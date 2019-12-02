using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
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

    [Header("Projectile settings")]
    public Transform projectileParent;
    public GameObject projectilePrefab;
    public float projectileLifespan = 1f;

    bool firing = true;

    private void Start() {
        firing = false;
        StartCoroutine("FireSequence");
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0))
        {
            firing = true;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            firing = false;
        }

        Vector3 rot = new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0);

        tPitch.localEulerAngles += new Vector3(-rot.x, 0, 0);
        tYaw.localEulerAngles += new Vector3(0, rot.y, 0);
    }

    IEnumerator FireSequence()
    {
        int i = 0;
        while(true)
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

        // Then move it back after firing
        while(barrel.localPosition.y < startPos)
        {
            barrel.localPosition = Vector3.MoveTowards(barrel.localPosition, barrel.localPosition + new Vector3(0, barrelRecoil / 5, 0), barrelRecoilPower);
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    public void FireBarrel(Transform barrel)
    {
        GameObject p = Instantiate(projectilePrefab, barrel.GetChild(0).transform.position, Quaternion.identity, projectileParent);
        p.GetComponent<Projectile>().Init(barrel.up, 1, 1, projectileLifespan);
    }
}
