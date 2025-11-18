using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CorpSystem : MonoBehaviour
{
    public List<Corp> corps = new List<Corp>(); // make each corp in editor -> save to json file

    [SerializeField] int minStartValue = 10000;
    [SerializeField] int maxStartValue = 1000000;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetCompanyValues();
    }

    void SetCompanyValues()
    {
        foreach (Corp corp in corps)
        {
            corp.currentValue = Random.Range(minStartValue, maxStartValue);
        }
    }

}
[System.Serializable]
public class Corp
{
    public string companyName;
    public int currentValue;
    [Header("Visual")]
    public Color companyColor;
    [TextArea(3, 3)]
    public string companyMotto;
}
