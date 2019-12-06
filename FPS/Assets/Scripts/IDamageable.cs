using UnityEngine;

public interface IDamageable
{
    // Current damage target has taken
    int damageTaken {get; set;}
    // Maximum damage target can take
    int damageThreshold {get; set;}
    // Should return the amount of damage target has taken as a value between 0 and 1
    float damagePercent{get;}
    // Amount of time before target starts healing damage
    float healDelay{get; set;}

    // Function for taking a set amount of damage
    void TakeDamage(int amount);
    // What to do when target reaches damage threshold
    void OnReachedThreshold();
    // Function for healing set amount of damage
    void HealDamage(int amount);
}
