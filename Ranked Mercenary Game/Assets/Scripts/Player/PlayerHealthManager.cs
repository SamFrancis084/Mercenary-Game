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
    public override void TakeDamage(int damage, float force = 0f, Vector3 hitPoint = new Vector3(), Vector3 dir = new Vector3())
    {
        base.TakeDamage(damage);
        uiScript.UpdateHealthUi(currentHealth, maxHealth);
    }
    public override void Die(float force = 0, Vector3 dir = new Vector3())
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
