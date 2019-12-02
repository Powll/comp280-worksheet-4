using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector3 dir;
    int damage;
    float speed;

    private void Update() 
    {
        transform.LookAt(transform.position + dir);
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed);    
    }

    public void Init(Vector3 direction, int damage, float speed, float lifeSpan)
    {
        dir = direction;
        this.damage = damage;
        this.speed = speed;
        transform.LookAt(transform.position + dir);
        GetComponentInChildren<MeshRenderer>().enabled = true;
        Invoke("DestroyProjectile", lifeSpan);
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
