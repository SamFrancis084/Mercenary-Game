using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] List<WeaponData> heldWeapons = new List<WeaponData>();
    [SerializeField] int currentWeaponIndex = 0;
    [SerializeField] WeaponData currentWeapon;

    [SerializeField] Transform firePointT;

    [SerializeField] GunStats gunStats;
    float fireTimer;
    float maxRange = 200f;

    [Header("Effects")]
    [SerializeField] GameObject hitSmokeGo;
    [SerializeField] TMP_Text gunText;

    private void Start()
    {
        currentWeapon = heldWeapons[currentWeaponIndex];
        UpdateGS();
        fireTimer = gunStats.fireRate;
        UpdateText();
    }

    private void Update()
    {
        WeaponSwapper();

        fireTimer += Time.deltaTime;
        if (Input.GetMouseButton(0) && fireTimer > gunStats.fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    void WeaponSwapper()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            PreviousWeapon();
            UpdateGS();
            UpdateText();
        }
        else if (Input.mouseScrollDelta.y < 0)
        {

            NextWeapon();
            UpdateGS();
            UpdateText();
        }
    }

    void NextWeapon()
    {
        if (currentWeaponIndex + 1 < heldWeapons.Count)
        {
            currentWeaponIndex++;
        }
        else
        {
            //loop back to zero
            currentWeaponIndex = 0;
        }
        currentWeapon = heldWeapons[currentWeaponIndex];
    }
    void PreviousWeapon()
    {
        if (currentWeaponIndex - 1 > -1)
        {
            currentWeaponIndex--;
        }
        else
        {
            // loop other way
            currentWeaponIndex = heldWeapons.Count - 1;
        }
        currentWeapon = heldWeapons[currentWeaponIndex];
    }

    void UpdateGS()
    {
        if (currentWeapon == null) return;
        gunStats = currentWeapon.gunStats;
    }

    void UpdateText()
    {
        gunText.text = $"Current Weapon: {currentWeapon.name}";
    }

    void Shoot()
    {
        Debug.Log("shoot");
        Vector3 firePoint = firePointT.position;
        Vector3 aimDir = firePointT.forward;
        RaycastHit hit;

        if (Physics.Raycast(firePoint, aimDir, out hit, maxRange))
        {
            //particle effects on hit
            

            //calculate damage (linear falloff)
            float hitDist = Vector3.Distance(hit.point, firePoint);
            float dmgFalloff = (hitDist - gunStats.effectiveRange) / (gunStats.ineffectiveRange - gunStats.effectiveRange);
            dmgFalloff = Mathf.Clamp(dmgFalloff, 0f, 1f);
            int dmg = Mathf.RoundToInt(Mathf.Lerp(gunStats.damage, 0, dmgFalloff));

            gunText.text = $"Current Weapon: {currentWeapon.name} | damage: <color=\"red\">{dmg}</color> | distance: <color=\"yellow\">{hitDist}</color>";

            //damage npcs
            if (hit.transform.GetComponent<BaseNPC>() != null)
            {
                Vector3 dir = (hit.point - firePoint).normalized;
                hit.transform.GetComponent<BaseNPC>().TakeDamage(dmg, dir, gunStats.weaponForce, hit.point);
            }
            else
            {
                GameObject hitFx = Instantiate(hitSmokeGo, hit.point, Quaternion.identity);
            }

            //knock back rbs
            if (hit.transform.GetComponent<Rigidbody>() != null)
            {
                Rigidbody hitRb = hit.transform.GetComponent<Rigidbody>();
                Vector3 dir = (hit.point - firePoint).normalized;
                hitRb.AddForce(dir * gunStats.weaponForce, ForceMode.Impulse);
            }
        }
    }
}
