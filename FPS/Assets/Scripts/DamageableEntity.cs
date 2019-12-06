using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableEntity : MonoBehaviour, IDamageable
{
    // IDamageable variables
    public int _damageTaken = 0;
    public int damageTaken {get{return _damageTaken;} set{_damageTaken = value;}}

    public int _damageThreshold = 100;
    public int damageThreshold {get{return _damageThreshold;} set{_damageThreshold = value;}}

    public float damagePercent{get{return (float)damageTaken / (float)damageThreshold;}}

    public float _healDelay = 1f;
    public float healDelay {get{return _healDelay;} set{_healDelay = value;}}

    void IDamageable.HealDamage(int amount)
    {
        damageTaken -= amount;
    }

    IEnumerator Heal()
    {
        yield return new WaitForSeconds(healDelay);

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
}
