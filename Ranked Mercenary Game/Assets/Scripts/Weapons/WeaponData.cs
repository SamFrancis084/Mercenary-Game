using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Preset", menuName = "ScriptableObjects/WeaponScriptableObject", order = 2)]
public class WeaponData : ScriptableObject
{
    public string weaponName;

    public GameObject weaponGo;

    public GunStats gunStats;
}
[System.Serializable]
public class GunStats
{
    public int damage;
    public float weaponForce; // for moving rbs
    public int clipSize;
    public float fireRate;
    public float reloadTime;

    public float effectiveRange; // max damage
    public float ineffectiveRange; // >= no damage
}