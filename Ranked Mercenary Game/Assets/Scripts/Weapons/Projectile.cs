using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public float lifeTime = 5f;

    private void Awake()
    {
        Invoke("DestroyMe", lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<HealthManager>(out HealthManager healthManager))
        {
            healthManager.TakeDamage(damage);            
        }
        else if (other.gameObject.GetComponentInParent<HealthManager>())
        {
            other.gameObject.GetComponentInParent<HealthManager>().TakeDamage(damage);
        }

        DestroyMe();
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }
}
