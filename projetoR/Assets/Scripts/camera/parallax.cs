using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{
    public Transform background;
    public float parallaxScale;
    public float smoothing;

    public Transform cam;
    private Vector3 previewCamPosition;

    void Start()
    {
        cam = Camera.main.transform;
        previewCamPosition = cam.position;
    }

    void Update()
    {
        float parallaxEffect = (previewCamPosition.x - cam.position.x) * parallaxScale;
        float backgroundTargetX = background.position.x + parallaxEffect;

        Vector3 backgroundPosition = new Vector3(backgroundTargetX, background.position.y, background.localPosition.x);

        background.position = Vector3.Lerp(background.position, backgroundPosition, smoothing * Time.deltaTime);

        previewCamPosition = cam.position;
    }
}
