using UnityEngine;

[CreateAssetMenu(fileName = "Npc Preset", menuName = "ScriptableObjects/NpcPresetScriptableObject", order = 1)]
public class NpcPreset : ScriptableObject
{
    public Stats npcStats;
    public int currentHealth;
    public bool isDead = false;
}
