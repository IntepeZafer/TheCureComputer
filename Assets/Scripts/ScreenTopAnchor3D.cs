using UnityEngine;

public class ScreenTopAnchor3D : MonoBehaviour
{
    [Header("Mesafe Ayarı")]
    public float kameraUzakligi = 50f; 

    [Header("Konum Ayarları")]
    
    [Range(0f, 1f)]
    public float yatayKonum = 0.5f; 

    [Range(0.8f, 1.2f)]
    public float dikeyKonum = 1.0f;

    private Camera anaKamera;

    void Start()
    {
        anaKamera = Camera.main;
    }

    void LateUpdate()
    {
        if (anaKamera == null) return;

        Vector3 hedefNokta = new Vector3(yatayKonum, dikeyKonum, kameraUzakligi);


        transform.position = anaKamera.ViewportToWorldPoint(hedefNokta);
        
 
        transform.LookAt(anaKamera.transform);
    }
}
