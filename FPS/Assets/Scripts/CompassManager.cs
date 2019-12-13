using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Target
{
    public Transform targetTransform;
    public Color displayColor;
    public RectTransform displayIcon;
}

public class CompassManager : MonoBehaviour
{
    // Compass settings
    [Header("Compass settings")]
    public bool scaleIconsWithDistance = true;
    public float maxIconSize = 4f;
    // Minimum distance from the screen for icon placement
    public float screenIconOffset = 5;

    // Distance above targets icon should be
    public float iconOffset = 4f;
    // List of all targets we should keep track of
    public List<Target> targets;
    [SerializeField] private Canvas canvas;

    private void Start() {
        canvas = FindObjectOfType<Canvas>();
    }

    // Every update, we iterate through all the targets in the scene and display them on the canvas accordingly
    private void Update() 
    {
        foreach(Target t in targets)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(t.targetTransform.position + new Vector3(0, 1, 0) * iconOffset);

            pos.x = Mathf.Clamp(pos.x, screenIconOffset, canvas.pixelRect.width - screenIconOffset);
            pos.y = Mathf.Clamp(pos.y, screenIconOffset, canvas.pixelRect.height - screenIconOffset);

            t.displayIcon.position = pos;

            t.displayIcon.GetComponent<Image>().color = t.displayColor;

            if(scaleIconsWithDistance)
                t.displayIcon.localScale = Vector3.one * Mathf.Clamp(Vector3.Magnitude(Camera.main.transform.position - t.targetTransform.position), 1f, maxIconSize);

            Image img = t.displayIcon.GetComponent<Image>();

            if(img)
            {
                if(img.fillMethod == Image.FillMethod.Radial360)
                    if(t.targetTransform.GetComponent<DamageableEntity>())
                        img.fillAmount = 1f - t.targetTransform.GetComponent<DamageableEntity>().damagePercent;
            }
        }
    }
}
