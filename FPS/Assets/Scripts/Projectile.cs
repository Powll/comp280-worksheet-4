using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector3 dir;
    int damage;
    float speed;
    Transform parent;

    public ParticleSystem hitParticles;

    private void Update() 
    {
        transform.LookAt(transform.position + dir);
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed);    
    }

    public void Init(Vector3 direction, int damage, float speed, float lifeSpan, Transform parent)
    {
        dir = direction;
        this.damage = damage;
        this.speed = speed;
        this.parent = parent;
        transform.LookAt(transform.position + dir);
        GetComponentInChildren<MeshRenderer>().enabled = true;
        Invoke("DestroyProjectile", lifeSpan);
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.GetComponent<IDamageable>() != null)
        {
            List<Transform> transforms = new List<Transform>();
            transforms.AddRange(other.gameObject.GetComponentsInParent<Transform>());

            if(!transforms.Contains(parent))
            {
                other.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
                if(hitParticles)
                {
                    GameObject o = Instantiate(hitParticles.gameObject, other.GetContact(0).point, Quaternion.identity);
                }
                DestroyProjectile();
            }
        }    
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
