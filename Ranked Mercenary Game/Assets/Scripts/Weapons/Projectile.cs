using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public float lifeTime = 5f;

    public SpellManager spellManager;
    public ParticleSystem ps;

    public GameObject destroyFX;

    Collider myCol;
    MeshRenderer mr;
    Rigidbody rb;

    private void Awake()
    {
        myCol = GetComponent<Collider>();
        mr = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();

        ps = GetComponentInChildren<ParticleSystem>();

        Invoke("DestroyAfterLifetime", lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (destroyFX != null) Instantiate(destroyFX, transform.position, Quaternion.identity);

        if (other.transform.TryGetComponent<HealthManager>(out HealthManager healthManager))
        {
            healthManager.TakeDamage(damage);            
        }

        if (spellManager != null) spellManager.SpellExplosion(transform.position);

        StartCoroutine(DestroyMe());
    }

    IEnumerator DestroyMe()
    {
        rb.isKinematic = true;
        myCol.enabled = false;
        mr.enabled = false;

        if (ps != null)
        {
            ps.Stop();
        }     
        
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }

    void DestroyAfterLifetime()
    {
        Destroy(gameObject);
    }
}
