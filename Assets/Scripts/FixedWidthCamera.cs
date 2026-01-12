using UnityEngine;

[ExecuteInEditMode]
public class FixedWidthCamera : MonoBehaviour
{
    public float targetAspectRatio = 16f / 9f;
    public float fieldOfView = 60f;
    private Camera cam;
    void Start()
    {
        cam = GetComponent<Camera>();
        UpdateCamera();
    }
    void Update()
    {
        UpdateCamera();
    }
    void UpdateCamera()
    {
        if(cam == null)
        {
            cam = GetComponent<Camera>();
        }
        float currentAspectRatio = (float)Screen.width / (float)Screen.height;
        float horizontalFov = CalcVerticalFov(fieldOfView , 1 / targetAspectRatio);
        float newVerticalFov = CalcVerticalFov(horizontalFov, currentAspectRatio);
        cam.fieldOfView = newVerticalFov;
    }
    private float CalcVerticalFov(float hFovInDeg, float aspectRatio)
    {
        float hFovInRads = hFovInDeg * Mathf.Deg2Rad;
        float vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2) / aspectRatio);
        return vFovInRads * Mathf.Rad2Deg;
    }
}
