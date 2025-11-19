using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public CasterType casterType = CasterType.PLAYER;

    //test with one spell first
    public SpellSO currentSpell;

    public Transform firePoint;
    public float spellForce = 40f;
    public LayerMask explodeLayer;

    [Header("Keybinds")] //change to work with new input system
    public KeyCode castKey = KeyCode.Mouse0;

    public int maxMana = 100;
    public int currentMana;
    public float rechargeSpeed = 1f;
    float tempManaRechargeAmount = 0f;

    public float rechargeWaitTime = 2f;
    float rechargeTimer = 0f;

    float fireTimer = 0;

    [Header("FX")]
    [SerializeField] GameObject explosionGo;
    [SerializeField] bool useCameraShake = false;
    [SerializeField] float maxExplosionRangeForCamShake = 30f;

    [Header("SFX")] // change clip based on spell
    [SerializeField] AudioClip spellClip;
    [SerializeField] AudioClip explosionClip;
    [SerializeField] AudioSource audioSource;

    [Header("Other Scritps")]
    [SerializeField] CameraShake camShake;
    HealthManager healthManager;
    [SerializeField] PlayerUi playerUi;
    Camera cam;

    private void Awake()
    {
        if (casterType == CasterType.PLAYER)
        {
            playerUi.UpdateManaUi(currentMana, maxMana);
            cam = Camera.main;
        }
        currentMana = maxMana;
        healthManager = GetComponentInParent<HealthManager>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (casterType == CasterType.PLAYER)  InputManager();
        RechargeMana();
    }

    void InputManager()
    {
        fireTimer -= Time.deltaTime;

        if (Input.GetKey(castKey) && fireTimer <= 0f) // change to use cast speed
        {
            rechargeTimer = 0f;
            Cast();
            fireTimer = currentSpell.spellFireRate;
        }
    }

    public void Cast()
    {
        if (currentMana - currentSpell.manaCost <= 0)
        {
            Debug.Log("not enough mana you absolute fool");
            return;
        }
        currentMana -= currentSpell.manaCost;

        if (casterType == CasterType.PLAYER)
        {
            if (useCameraShake) StartCoroutine(camShake.Shake(0.1f, 0.1f)); // test cam shake
            playerUi.UpdateManaUi(currentMana, maxMana);
        }       
        

        PlaySpellAudio();

        CastType spellType = currentSpell.castType;
        switch (spellType)
        {
            case CastType.SELF:
                SelfCast();
                    break;
            case CastType.TOUCH:
                TouchCast();
                break;
            case CastType.RANGE:
                RangeCast();
                break;
        }
    }

    void SelfCast()
    {
        if (currentSpell.spellDamage > 0)
        {
            healthManager.TakeDamage(currentSpell.spellDamage);
        }
        else
        {
            healthManager.Heal(Mathf.Abs(currentSpell.spellDamage));
        }

        if (currentSpell.explosionRadius > 0) SpellExplosion(transform.position);
    }
    void TouchCast()
    {
        RaycastHit hit;

        if (casterType == CasterType.PLAYER)
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 1f))
            {
                if (hit.transform.TryGetComponent<HealthManager>(out HealthManager targetHM))
                {
                    if (currentSpell.spellDamage > 0) targetHM.TakeDamage(currentSpell.spellDamage);
                    else targetHM.Heal(Mathf.Abs(currentSpell.spellDamage));
                }
                else if (hit.transform.parent.TryGetComponent<HealthManager>(out HealthManager parentHM))
                {
                    if (currentSpell.spellDamage > 0) parentHM.TakeDamage(currentSpell.spellDamage);
                    else parentHM.Heal(currentSpell.spellDamage);
                }

                if (currentSpell.explosionRadius > 0) SpellExplosion(hit.point);
            }
        }
        else
        {
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, 1f))
            {
                if (hit.transform.TryGetComponent<HealthManager>(out HealthManager targetHM))
                {
                    if (currentSpell.spellDamage > 0) targetHM.TakeDamage(currentSpell.spellDamage);
                    else targetHM.Heal(Mathf.Abs(currentSpell.spellDamage));
                }
                else if (hit.transform.parent.TryGetComponent<HealthManager>(out HealthManager parentHM))
                {
                    if (currentSpell.spellDamage > 0) parentHM.TakeDamage(currentSpell.spellDamage);
                    else parentHM.Heal(currentSpell.spellDamage);
                }

                if (currentSpell.explosionRadius > 0) SpellExplosion(hit.point);
            }
        }

        

    }
    void RangeCast()
    {
        GameObject newProjectile = Instantiate(currentSpell.projectilePrefab, firePoint.position, firePoint.rotation);
        newProjectile.name = currentSpell.spellName + "_inst";

        Projectile proj = newProjectile.GetComponent<Projectile>();
        proj.damage = currentSpell.spellDamage;
        proj.spellManager = this;

        newProjectile.GetComponent<Rigidbody>().AddForce(firePoint.forward * spellForce, ForceMode.Impulse);
    }

    public void SpellExplosion(Vector3 explosionPoint)
    {
        if (currentSpell.explosionRadius <= 0) return;

        if (casterType == CasterType.PLAYER && useCameraShake)
        {
            float explosionMag = Mathf.Lerp(1f, 0f, (transform.position - explosionPoint).magnitude / maxExplosionRangeForCamShake);
            StartCoroutine(camShake.Shake(0.5f * explosionMag, explosionMag));
        }

        //fx to see radius
        GameObject explosion = Instantiate(explosionGo, explosionPoint, Quaternion.identity);
        explosion.transform.localScale *= currentSpell.explosionRadius * 2;

        bool willHeal = currentSpell.spellDamage < 0;
        Collider[] cols = Physics.OverlapSphere(explosionPoint, currentSpell.explosionRadius, explodeLayer);
        foreach (Collider col in cols)
        {
            if (col.transform.TryGetComponent<HealthManager>(out HealthManager colHM))
            {
                if (willHeal) colHM.Heal(Mathf.Abs(currentSpell.spellDamage));
                else colHM.TakeDamage(currentSpell.spellDamage, 50, explosionPoint, (col.transform.position - explosionPoint).normalized);

                Debug.Log($"{col.gameObject.name} was hit {Vector3.Distance(col.transform.position, explosionPoint)} units from my centre");
            }
        }

        AudioTools.PlayClipAtPoint(explosionClip, explosionPoint, 1f, Random.Range(0.8f, 1.1f), 0.3f);
    }

    void RechargeMana()
    {
        // if not in combat
        rechargeTimer += Time.deltaTime;
        if (rechargeTimer < rechargeWaitTime) return;

        if (currentMana < maxMana)
        {
            tempManaRechargeAmount += Time.deltaTime * rechargeSpeed;
            if (tempManaRechargeAmount > 1)
            {
                currentMana++;
                tempManaRechargeAmount = 0f;
                if (casterType == CasterType.PLAYER) playerUi.UpdateManaUi(currentMana, maxMana);
            }
        }
        else
        {
            currentMana = maxMana;            
        }
    }

    void PlaySpellAudio()
    {
        float randomPitch = Random.Range(0.8f, 1.2f);
        audioSource.pitch = randomPitch;
        audioSource.PlayOneShot(spellClip);
    }
}

public enum CasterType
{
    PLAYER,
    NPC
}
