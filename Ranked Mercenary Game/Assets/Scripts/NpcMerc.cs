using UnityEngine;
using TMPro;

public class NpcMerc : BaseNPC
{
    public string mercName;
    public int mercRank;

    public Corp chosenCorp;

    //[SerializeField] MercenaryBase npcStats;

    TMP_Text nameText;
    SkinnedMeshRenderer smr;

    [Header("Chunks")]
    [SerializeField] bool willChunk = true;
    [SerializeField] GameObject chunkGo;

    private void Awake()
    {
        nameText = GetComponentInChildren<TMP_Text>();
        smr = GetComponentInChildren <SkinnedMeshRenderer>();

        base.initHealth();
    }

    private void Update()
    {
        base.PlayerDetector();
        base.MovementManager();
    }

    public void SetValues(Stats setStats)
    {
        if (nameText == null) nameText = GetComponentInChildren<TMP_Text>();

        base.myStats = setStats;

        mercName = setStats.mercName;
        mercRank = setStats.rank;

        nameText.text = mercName + "<br>Rank: " + mercRank; 

        gameObject.name = mercName;

        base.initHealth(); // init health after receiving max health
    }

    public void SetCorp(Corp corp)
    {
        chosenCorp = corp;
        smr.materials[1].color = corp.companyColor;
    }

}
