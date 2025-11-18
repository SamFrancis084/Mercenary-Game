using UnityEngine;

public class NpcHealthManager : HealthManager
{
    [Header("GIBS")]
    public GameObject gibGo;
    public int dmgToGib = 20;

    [Header("Effects")]
    public GameObject hurtGo;
    public GameObject deadGo;

    public RagdollController rdScript;
    Animator myAnimator;

    private void Awake()
    {
        myAnimator = GetComponentInChildren<Animator>();
    }

    public override void TakeDamage(int damage, float force = 0f, Vector3 hitPoint = new Vector3(), Vector3 dir = new Vector3())
    {
        if (hurtGo != null) Instantiate(hurtGo, hitPoint, Quaternion.identity);

        if (currentHealth - damage > 0)
        {
            currentHealth -= damage;
        }
        else if (currentHealth - damage < -dmgToGib)
        {
            //gib
            GameObject gibs = Instantiate(gibGo, transform.position, transform.rotation);
            gibs.GetComponent<NpcChunks>().MoveChunks(dir);

            if (npcScript != null) npcScript.isDead = true;
            gameObject.SetActive(false);
        }
        else
        {
            if (deadGo != null) Instantiate(deadGo, hitPoint, Quaternion.identity);

            currentHealth = 0;
            Die(force, dir);
        }
    }

    public override void Die(float force = 0, Vector3 dir = new Vector3())
    {
        isAlive = false;

        if (npcScript != null) npcScript.isDead = true;
        if (myAnimator != null) myAnimator.enabled = false;

        if (rdScript != null) rdScript.EnableRagdoll(dir, force);
    }
}
