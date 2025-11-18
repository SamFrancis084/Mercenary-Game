using System;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public bool isAlive = true;

    public int maxHealth = 100;
    public int currentHealth;

    public BaseNPC npcScript; // if applicable

    private void Awake()
    {
        InitHealth();
    }

    public virtual void InitHealth()
    {
        currentHealth = maxHealth;        
    }

    public virtual void InitNpcHealth(BaseNPC npc)
    {
        npcScript = npc;
        maxHealth = npc.myStats.maxHealth;
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage, float force = 0f, Vector3 hitPoint = new Vector3(), Vector3 dir = new Vector3())
    {
        if (currentHealth - damage > 0)
        {
            currentHealth -= damage;
        }
        else
        {
            currentHealth = 0;
            Die(force, dir);
        }
    }

    public virtual void Die(float force = 0, Vector3 dir = new Vector3())
    {
        isAlive = false;
    }

    public virtual void Heal(int healAmount)
    {
        if (currentHealth + healAmount < maxHealth)
        {
            currentHealth += healAmount;
        }
        else
        {
            currentHealth = maxHealth;
        }
    }
}
