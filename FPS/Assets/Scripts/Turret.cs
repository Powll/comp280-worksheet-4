﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour, IDamageable
{
    [Header("Control settings")]
    public bool playerControlled = true;
    public Transform target;
    [Header("Turret components")]
    // Used for rotating along the X axis
    public Transform tPitch;
    // Used for rotating along the Y axis
    public Transform tYaw;
    // List of all barrels controlled by turret
    public List<Transform> tBarrels;

    [Header("Turret variables")]
    public float barrelRecoil = 1f;
    public float barrelRecoilPower = 0.1f;
    public float barrelCooldown = 0.2f;
    public float reloadTime = 2f;
    public int clipAmmo = 16;
    public int clipSize = 16;
    public int totalAmmo = 160;

    // IDamageable variables
    public int _damageTaken = 0;
    public int damageTaken {get{return _damageTaken;} set{_damageTaken = value;}}

    public int _damageThreshold = 100;
    public int damageThreshold {get{return _damageThreshold;} set{_damageThreshold = value;}}

    public float damagePercent{get{return (float)damageTaken / (float)damageThreshold;}}
    public float ammoPercent{get{return (float)clipAmmo / (float)clipSize;}}

    public float _healDelay = 1f;
    public float healDelay {get{return _healDelay;} set{_healDelay = value;}}

    [Header("Projectile settings")]
    public Transform projectileParent;
    public GameObject projectilePrefab;
    public GameObject shellPrefab;
    public float projectileLifespan = 1f;
    public int projectileDamage = 1;
    public float projectileSpeed = 1f;

    [Header("UI settings")]
    public Slider healthSlider;
    public Slider healCooldownSlider;
    public Slider ammoSlider;
    public Slider reloadTimerSlider;

    int cooldownSpeed = 10;

    bool firing = true;
    bool reloading = false;

    private void Start() {
        firing = false;
        StartCoroutine("FireSequence");
    }

    void IDamageable.HealDamage(int amount)
    {
        damageTaken -= amount;
    }

    IEnumerator Heal()
    {
        if(healCooldownSlider)
            for(int i = 1; i <= cooldownSpeed; i++)
            {
                healCooldownSlider.value = (float)i / (float)cooldownSpeed;   
                yield return new WaitForSeconds((float)healDelay / (float) cooldownSpeed);
            }

        while(damageTaken > 0)
        {
            GetComponent<IDamageable>().HealDamage(1);
            yield return new WaitForSeconds(0.5f);
        }
    }

    void IDamageable.OnReachedThreshold()
    {
        Destroy(gameObject);
    }

    void IDamageable.TakeDamage(int amount)
    {
        damageTaken += amount;

        if(damageTaken >= damageThreshold)
            GetComponent<IDamageable>().OnReachedThreshold();
        
        ResetHeal();
    }

    void ResetHeal()
    {
        StopCoroutine("Heal");
        StartCoroutine("Heal");
    }

    private void Update() {
        if(playerControlled)
        {
            if(Input.GetMouseButtonDown(0))
            {
                firing = true;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                firing = false;
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                if(!reloading)
                    StartCoroutine("ReloadSequence");
            }

            if(healthSlider)
            {
                healthSlider.value = 1.0f - damagePercent;
            }

            if(ammoSlider)
            {
                ammoSlider.value = ammoPercent;
            }

            Vector3 rot = new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);

            tPitch.localEulerAngles += new Vector3(-rot.x, 0, 0);
            tYaw.localEulerAngles += new Vector3(0, rot.y, 0);
        }
        else
        {
            if(target)
                {
                    firing = true;
                    AimTowards(target);
                }
            else
                firing = false;
        }
    }

    IEnumerator FireSequence()
    {
        int i = 0;
        while(true)
        {
            if(CanFire())
            {
                if(firing)
                {
                    StartCoroutine(MoveBarrel(tBarrels[i].GetChild(0).transform));
                    yield return new WaitForSeconds(barrelCooldown);
                    i = i + 1 >= tBarrels.Count ? 0 : i + 1; 
                }
                else
                    yield return new WaitForEndOfFrame();
            }
            else
            {
                if(!reloading)
                    StartCoroutine("ReloadSequence");
                else
                    yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator ReloadSequence()
    {
        reloading = true;

        if(reloadTimerSlider)
            for(int i = 1; i <= cooldownSpeed; i++)
            {
                reloadTimerSlider.value = (float)i / (float)cooldownSpeed;   
                yield return new WaitForSeconds((float)reloadTime / (float) cooldownSpeed);
            }

        reloading = false;

        if(totalAmmo >= clipSize)
        {
            clipAmmo = clipSize;
            totalAmmo -= clipSize;
        }
        else
        {
            clipAmmo = totalAmmo;
            totalAmmo = 0;
        }

        yield return null;
    }

    IEnumerator MoveBarrel(Transform barrel)
    {
        float startPos = barrel.localPosition.y;
        float endPos = startPos - barrelRecoil;

        // Move barrel back when firing
        FireBarrel(barrel);
        while(barrel.localPosition.y > endPos)
        {
            barrel.localPosition = Vector3.MoveTowards(barrel.localPosition, barrel.localPosition - new Vector3(0, barrelRecoil, 0), barrelRecoilPower);
            yield return new WaitForEndOfFrame();
        }

        GameObject o = Instantiate(shellPrefab, barrel.parent.GetChild(1).position, Quaternion.LookRotation(barrel.parent.GetChild(1).up, barrel.parent.GetChild(1).forward));
        o.GetComponent<Rigidbody>().AddForce(o.transform.up * 8, ForceMode.Impulse);

        // Then move it back after firing
        while(barrel.localPosition.y < startPos)
        {
            barrel.localPosition = Vector3.MoveTowards(barrel.localPosition, barrel.localPosition + new Vector3(0, barrelRecoil / 5, 0), barrelRecoilPower);
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    public bool CanFire()
    {
        return clipAmmo > 0 && !reloading;
    }

    public void FireBarrel(Transform barrel)
    {
        clipAmmo--;
        GameObject p = Instantiate(projectilePrefab, barrel.GetChild(0).transform.position, Quaternion.identity, projectileParent);
        p.GetComponent<Projectile>().Init(barrel.up, projectileDamage, projectileSpeed, projectileLifespan, transform);
        barrel.GetComponentInChildren<AudioSource>().Play();
        barrel.GetComponentInChildren<ParticleSystem>().Play();
    }

    // Rotates turret to face target and returns the angle difference between the target and the turret
    public float AimTowards(Transform target)
    {
        float angle = 0f;

        Vector3 dir = target.position - tYaw.position;

        Vector3 yawRot = Quaternion.LookRotation(dir).eulerAngles;
        Vector3 pitchRot = Quaternion.LookRotation(dir).eulerAngles;

        yawRot.x = tYaw.localEulerAngles.x;
        yawRot.z = 0;

        pitchRot.y = 0;
        pitchRot.z = 0;

        tYaw.localEulerAngles = yawRot;
        tPitch.localEulerAngles = pitchRot;

        return angle;
    }
}
