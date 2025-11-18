using UnityEngine;
using TMPro;
public class PlayerScanner : MonoBehaviour
{
    [SerializeField] BaseNPC targetNpc;

    [SerializeField] TMP_Text targetText;
    [SerializeField] bool targetLocked = false;

    [SerializeField] float maxD = 30f;
    [SerializeField] LayerMask npcLayer;

    [SerializeField] float displayTime = 5f;
    float displayTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetLocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetLocked)
        {
            displayTimer += Time.deltaTime;
            if (displayTimer > displayTime)
            {
                HideStats();
                displayTimer = 0f;
                targetLocked = false;
            }
        }

        if (Input.GetMouseButtonDown(1)) // change to new input
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxD, npcLayer))
            {
                if (hit.transform.GetComponent<BaseNPC>() != null)
                {
                    targetNpc = hit.transform.GetComponent<BaseNPC>();
                    ShowTargetStats();
                    targetLocked = true;
                    displayTimer = 0f;
                }
            }
            else
            {
                HideStats();
                targetLocked = false;
            }
        }
    }

    void ShowTargetStats()
    {
        Stats targetStats = targetNpc.myStats;

        string displayString = "Name: " + targetStats.mercName + "<br>Rank: " + targetStats.rank + "<br>Power Level: " + targetStats.powerLevel;
        targetText.text = displayString;
    }

    void HideStats()
    {
        targetText.text = "";
    }
}
