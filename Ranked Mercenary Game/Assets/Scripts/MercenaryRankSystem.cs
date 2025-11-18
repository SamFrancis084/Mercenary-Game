using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MercenaryRankSystem : MonoBehaviour
{
    [Header("Placed Mercs")]
    public GameObject mercPrefab;
    [SerializeField] float spawnRadius = 50f;

    public List<Stats> mercDataList = new List<Stats>(); //DATA List

    [SerializeField] int mercsToGenerate = 20;

    [Header("Randomised Values")]
    //[SerializeField] List<CompanyTemplate> possibleCorps;
    [SerializeField] RandomNames randomNames;
    [SerializeField] RandomStats randomStats;

    [Header("Other Scripts")]
    [SerializeField] CorpSystem corpSystem;
    Camera cam;

    private void Start()
    {
        cam = Camera.main;

        InitMercs();
    }

    void InitMercs()
    {
        //add player to list to be ranked
        for(int i = 0; i < mercsToGenerate; i++) // generate data
        {
            GenerateNewMercData();
        }

        RankMercs();

        //place mercs on screen
        GenerateMercGOs();
        
    }

    void GenerateNewMercData()
    {
        Stats newMercData = new Stats();

        //generate random values
        string mercName = randomNames.possibleFirstNames[UnityEngine.Random.Range(0, randomNames.possibleCallsigns.Length)]
            + " [" + randomNames.possibleCallsigns[UnityEngine.Random.Range(0, randomNames.possibleCallsigns.Length)] + "] "
            + randomNames.possibleLastNames[UnityEngine.Random.Range(0, randomNames.possibleLastNames.Length)];

        //find better way to randomise/ set misc values
        int maxHealth = UnityEngine.Random.Range(randomStats.minMaxHealth, randomStats.maxMaxHealth);
        int damage    = UnityEngine.Random.Range(randomStats.minDamage, randomStats.maxDamage);
        int accuracy  = UnityEngine.Random.Range(randomStats.minAccuracy, randomStats.maxAccuracy);
        int speed     = UnityEngine.Random.Range(randomStats.minSpeed, randomStats.maxSpeed);

        int mercPL = maxHealth + damage + accuracy + speed;

        //set values
        newMercData.mercName = mercName;
        newMercData.powerLevel = mercPL;
        newMercData.rank = 0;

        newMercData.maxHealth = maxHealth;
        newMercData.damage = damage;
        newMercData.accuracy = accuracy;
        newMercData.speed = speed;

        //add to list
        mercDataList.Add(newMercData);        
    }

    void RankMercs()
    {
        mercDataList = mercDataList.OrderByDescending(x => x.powerLevel).ToList();

        for (int i = 0; i < mercDataList.Count; i++)
        {
            mercDataList[i].rank = i + 1;
        }
    }
    
    void GenerateMercGOs()
    {
        foreach (var merc in mercDataList)
        {
            Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(-spawnRadius, spawnRadius), 0f, UnityEngine.Random.Range(-spawnRadius, spawnRadius));

            GameObject newMerc = Instantiate(mercPrefab, spawnPos, Quaternion.identity);
            newMerc.transform.parent = transform;

            NpcMerc npcScript = newMerc.GetComponent<NpcMerc>();
            npcScript.SetValues(merc);
            if (corpSystem.corps.Count > 0)
            {
                Corp assignedCorp = corpSystem.corps[UnityEngine.Random.Range(0, corpSystem.corps.Count)];
                npcScript.SetCorp(assignedCorp);
            }
        }
    }

}
[System.Serializable]
public class RandomStats
{
    public int minMaxHealth = 5;
    public int maxMaxHealth = 500;
    public int minDamage = 1;
    public int maxDamage = 100;
    public int minAccuracy = 1;
    public int maxAccuracy = 100;
    public int minSpeed = 1;
    public int maxSpeed = 100;
}
[System.Serializable]
public class RandomNames
{
    public string[] possibleCallsigns;
    public string[] possibleFirstNames;
    public string[] possibleLastNames;
}