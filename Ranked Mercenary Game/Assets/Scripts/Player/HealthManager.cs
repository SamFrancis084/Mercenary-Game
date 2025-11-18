using System;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public bool isAlive = true;

    public int maxHealth = 100;
    public int currentHealth;


    private void Awake()
    {
        InitHealth();
    }

    public virtual void InitHealth()
    {
        currentHealth = maxHealth;        
    }

    //test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) TakeDamage(10);
    }

    public virtual void TakeDamage(int dmg)
    {
        if (currentHealth - dmg > 0)
        {
            currentHealth -= dmg;
        }
        else
        {
            currentHealth = 0;
            Die();
        }
    }

    public virtual void Die()
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
