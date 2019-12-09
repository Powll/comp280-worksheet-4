using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airship : DamageableEntity
{
    public float speed;

    private void Update() {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, speed);
    }
}
