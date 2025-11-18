using System;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public bool isAlive = true;

    public int maxHealth = 100;
    public int currentHealth;

    [SerializeField] PlayerUi uiScript;

    public static event Action<HealthManager> HasDied;

    private void Awake()
    {
        InitHealth();
    }

    void InitHealth()
    {
        currentHealth = maxHealth;

        uiScript.UpdateHealthUi(currentHealth, maxHealth);
    }

    //test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) TakeDamage(10);
    }

    public void TakeDamage(int dmg)
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
        uiScript.UpdateHealthUi(currentHealth, maxHealth);
    }

    public void Die()
    {
        isAlive = false;

        if (HasDied != null) HasDied(this);
    }

    public void Heal(int healAmount)
    {
        if (currentHealth + healAmount < maxHealth)
        {
            currentHealth += healAmount;
        }
        else
        {
            currentHealth = maxHealth;
        }
        uiScript.UpdateHealthUi(currentHealth, maxHealth);
    }
}
