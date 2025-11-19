using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "ScriptableObjects/SpellScriptableObject")]
public class SpellSO : ScriptableObject
{
    public string spellName;

    public CastType castType;

    public int manaCost;

    public int spellDamage;
    public float spellFireRate = 0.1f;
    public float explosionRadius = 0;

    public GameObject projectilePrefab;
}
public enum CastType
{
    SELF,
    TOUCH,
    RANGE
}
