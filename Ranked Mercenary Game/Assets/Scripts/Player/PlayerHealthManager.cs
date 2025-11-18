using System;
using UnityEngine;

public class PlayerHealthManager : HealthManager
{

    [SerializeField] PlayerUi uiScript;
    public static event Action<HealthManager> HasDied;

    public override void InitHealth()
    {
        base.InitHealth();
        uiScript.UpdateHealthUi(currentHealth, maxHealth);
    }
    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        uiScript.UpdateHealthUi(currentHealth, maxHealth);
    }
    public override void Die()
    {
        base.Die();
        uiScript.UpdateHealthUi(currentHealth, maxHealth);

        if (HasDied != null) HasDied(this);
    }
    public override void Heal(int healAmount)
    {
        base.Heal(healAmount);
        uiScript.UpdateHealthUi(currentHealth, maxHealth);
    }
}
