using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airship : DamageableEntity
{
    [Header("Airship settings")]
    public float speed;
    [Header("Compass display settings")]
    public GameObject iconPrefab;
    public Color iconColor;
    GameObject iconObject;
    CompassManager compassManager;
    Target target;

    private void Start() {
        target = new Target();
        target.displayColor = iconColor;
        iconObject = Instantiate(iconPrefab, FindObjectOfType<Canvas>().transform);
        RectTransform icon = iconObject.GetComponent<RectTransform>();
        target.displayIcon = icon;
        target.targetTransform = transform;
        compassManager = FindObjectOfType<CompassManager>();
        compassManager.targets.Add(target);
    }

    private void Update() {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, speed);
    }

    private void OnDestroy() {
        compassManager.targets.Remove(target);
        Destroy(iconObject);
    }
}
