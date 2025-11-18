using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class MissionSimulator : MonoBehaviour
{
    [Header("Corporation")]
    [SerializeField] CorpSystem corpSystem;
    [SerializeField] List<Corp> corpPool;
    public Corp employerCorp;
    public Corp targetCorp;
    string employerCorpHex;
    string targetCorpHex;

    [Header("Combatants")]
    [SerializeField] MercenaryRankSystem mercSys;
    [SerializeField] List<Stats> mercPool;
    bool setMercPool = false;
    public List<BaseNPC> hiredTeam; // team 1 
    public List<BaseNPC> enemyTeam; // team 2

    [Header("Preset Npcs")]
    [SerializeField] List<NpcPreset> npcPresets;

    [Header("Fight Params")]
    [SerializeField] bool isFighting = false;
    [SerializeField] bool teamOneGoesFirst; // determine which team attacks first
    public int numberOfHiredFighters = 10;
    public int numberOfEnemyFighters = 10;
    public int rewardAmount = 1000;
    [Range(0f, 1f)] public float rewardSplit = 0.5f; // how much goes to the merc vs the employer
    [SerializeField] float fightDelay = 1f;

    [Header("UI")]
    [SerializeField] TMP_Text fightText;
    [SerializeField] ScrollRect scrollView;

    private void Update()
    {
        if (!setMercPool && mercSys.mercDataList.Count > 0)
        {
            mercPool.AddRange(mercSys.mercDataList);
            corpPool.AddRange(corpSystem.corps);

            setMercPool = true;
        }

        //test
        if (Input.GetKeyDown(KeyCode.Return) && !isFighting)
        {
            TestFight();
            isFighting = true;
        }
    }

    void TestFight()
    {
        employerCorp = corpPool[Random.Range(0, corpPool.Count)];
        corpPool.Remove(employerCorp);
        targetCorp = corpPool[Random.Range(0, corpPool.Count)];
        corpPool.Remove(targetCorp);

        employerCorpHex = ColorUtility.ToHtmlStringRGB(employerCorp.companyColor);
        targetCorpHex = ColorUtility.ToHtmlStringRGB(targetCorp.companyColor);

        Stats testMercStats = mercPool[Random.Range(0, mercPool.Count)];
        mercPool.Remove(testMercStats);
        BaseNPC testMerc = new GameObject(testMercStats.mercName).AddComponent<BaseNPC>();
        testMerc.myStats = testMercStats;
        hiredTeam.Add(testMerc);

        Stats testEnemyStats = mercPool[Random.Range(0, mercPool.Count)];
        mercPool.Remove(testEnemyStats);
        BaseNPC testEnemy = new GameObject(testEnemyStats.mercName).AddComponent<BaseNPC>();
        testEnemy.myStats = testEnemyStats;
        enemyTeam.Add(testEnemy);

        InitBattle();
    }

    public void InitBattle()
    {
        //initialise hired merc(s)
        for (int i = 0; i < hiredTeam.Count; i++)
        {
            hiredTeam[i].initHealth();
        }

        //generate preset npcs
        for (int i = 0; i < numberOfHiredFighters; i++)
        {
            NpcPreset newNpc = npcPresets[Random.Range(0, npcPresets.Count)]; // randomly pick preset (unweighted)

            Stats npcPresetStats = newNpc.npcStats;
            BaseNPC fighterNpc = new GameObject("FighterNpc_" + i).AddComponent<BaseNPC>();
            fighterNpc.myStats = npcPresetStats;
            fighterNpc.initHealth();

            hiredTeam.Add(fighterNpc);
        }

        //initialise defending merc(s)
        if (enemyTeam.Count > 0)
        {
            for (int i = 0; i < enemyTeam.Count; i++)
            {
                enemyTeam[i].initHealth();
            }
        }        

        //generate preset npcs
        for (int i = 0; i < numberOfEnemyFighters; i++)
        {
            NpcPreset newNpc = npcPresets[Random.Range(0, npcPresets.Count)]; // randomly pick preset (unweighted)

            Stats npcPresetStats = newNpc.npcStats;
            BaseNPC fighterNpc = new GameObject("FighterNpc_" + i).AddComponent<BaseNPC>();
            fighterNpc.myStats = npcPresetStats;
            fighterNpc.initHealth();

            enemyTeam.Add(fighterNpc);
        }

        fightText.text = $"Starting fight between <color=#{employerCorpHex}> {employerCorp.companyName} </color> VS <color=#{targetCorpHex}> {targetCorp.companyName} </color>";

        //determine fight order
        teamOneGoesFirst = Random.Range(0f, 1f) > 0.5f ? true : false;

        if (teamOneGoesFirst)
        {
            StartCoroutine(hiredTeamsTurn());
        }
        else
        {
            StartCoroutine(enemyTeamsTurn());
        }        
    }

    IEnumerator hiredTeamsTurn()
    {
        //1.choose fighter from team
        BaseNPC currentFighter = hiredTeam[Random.Range(0, hiredTeam.Count)];

        //2.choose target from opposing team
        BaseNPC targetFighter = enemyTeam[Random.Range(0, enemyTeam.Count)];

        //3.attack
        int dmg = currentFighter.myStats.damage;
        //randomise dmg
        float randomDamage = (float)dmg * Random.Range(0.1f, 1.5f);
        dmg = Mathf.RoundToInt(randomDamage);
        targetFighter.TakeDamage(dmg);

        //4.report damage
        if (targetFighter.isDead)
        {
            fightText.text += $"<br><color=#{employerCorpHex}>{currentFighter.myStats.mercName}</color> <color=\"red\">killed</color> <color=#{targetCorpHex}>{targetFighter.myStats.mercName}</color>! {dmg}";
            scrollView.verticalNormalizedPosition = 0f;
            enemyTeam.Remove(targetFighter); // remove from fight

            //take out of ranking list
            if (mercSys.mercDataList.Contains(targetFighter.myStats))
            {
                mercSys.mercDataList.Remove(targetFighter.myStats);
            }

            if (enemyTeam.Count <= 0)
            {
                EndFight(true);
                yield break;
            }
        }
        else
        {
            fightText.text += $"<br> <color=#{employerCorpHex}>{currentFighter.myStats.mercName}</color> attacked <color=#{targetCorpHex}>{targetFighter.myStats.mercName}</color> for {dmg} points of damage!";
            scrollView.verticalNormalizedPosition = 0f;
        }
        yield return new WaitForSeconds(fightDelay);
        StartCoroutine(enemyTeamsTurn());

    }
    IEnumerator enemyTeamsTurn()
    {
        //1.choose fighter from team
        BaseNPC currentFighter = enemyTeam[Random.Range(0, enemyTeam.Count)];

        //2.choose target from opposing team
        BaseNPC targetFighter = hiredTeam[Random.Range(0, hiredTeam.Count)];

        //3.attack
        int dmg = currentFighter.myStats.damage;

        //randomise dmg
        float randomDamage = (float)dmg * Random.Range(0.1f, 1.5f);
        dmg = Mathf.RoundToInt(randomDamage);

        targetFighter.TakeDamage(dmg);

        //4.report damage
        if (targetFighter.isDead)
        {
            fightText.text += $"<br><color=#{targetCorpHex}>{currentFighter.myStats.mercName}</color> <color=\"red\">killed</color> <color=#{employerCorpHex}>{targetFighter.myStats.mercName}</color>! {dmg}";
            scrollView.verticalNormalizedPosition = 0f;
            hiredTeam.Remove(targetFighter); // remove from fight

            //take out of ranking list
            if (mercSys.mercDataList.Contains(targetFighter.myStats))
            {
                mercSys.mercDataList.Remove(targetFighter.myStats);
            }

            // enemy company takes defeated mercs money
            targetCorp.currentValue += targetFighter.myStats.currentMoney;

            if (hiredTeam.Count <= 0)
            {
                EndFight(false);
                yield break;
            }
        }
        else
        {
            fightText.text += $"<br> <color=#{targetCorpHex}>{currentFighter.myStats.mercName}</color> attacked <color=#{employerCorpHex}>{targetFighter.myStats.mercName}</color> for {dmg} points of damage!";
            scrollView.verticalNormalizedPosition = 0f;
        }
        yield return new WaitForSeconds(fightDelay);
        StartCoroutine(hiredTeamsTurn());
    }

    void EndFight(bool teamOneWon)
    {
        if (teamOneWon)
        {
            fightText.text += $"<br> <color=#{employerCorp}>{employerCorp.companyName}</color>'s team have won the battle";
            scrollView.verticalNormalizedPosition = 0f;
            //update stats
            float mercsMoney = (float)rewardAmount * rewardSplit;
            for (int i = 0; i < hiredTeam.Count; i++)
            {
                hiredTeam[i].myStats.currentMoney += Mathf.RoundToInt(mercsMoney) / hiredTeam.Count;

                if (mercSys.mercDataList.Contains(hiredTeam[i].myStats))
                {
                    mercSys.mercDataList[mercSys.mercDataList.IndexOf(hiredTeam[i].myStats)] = hiredTeam[i].myStats;

                    mercPool.Add(hiredTeam[i].myStats);
                }
            }

            rewardAmount -= Mathf.RoundToInt(mercsMoney);
            employerCorp.currentValue += rewardAmount;
        }
        else
        {
            fightText.text += $"<br> <color=#{targetCorpHex}>{targetCorp.companyName}</color>'s team have won the battle";
            scrollView.verticalNormalizedPosition = 0f;

            float mercsMoney = (float)rewardAmount * rewardSplit;

            for (int i = 0; i < enemyTeam.Count; i++)
            {
                enemyTeam[i].myStats.currentMoney += Mathf.RoundToInt(mercsMoney) / enemyTeam.Count;

                if (mercSys.mercDataList.Contains(enemyTeam[i].myStats))
                {
                    mercSys.mercDataList[mercSys.mercDataList.IndexOf(enemyTeam[i].myStats)] = enemyTeam[i].myStats;

                    mercPool.Add(enemyTeam[i].myStats);
                }
            }
        }

        //clear teams
        hiredTeam.Clear();
        enemyTeam.Clear();

        corpPool.Add(employerCorp);
        corpPool.Add(targetCorp);

        Canvas.ForceUpdateCanvases();
        scrollView.verticalNormalizedPosition = 0f;

        isFighting = false;
    }
}
