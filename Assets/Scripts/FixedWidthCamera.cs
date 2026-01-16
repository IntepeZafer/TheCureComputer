using UnityEngine;
using System.Collections;

public class RotasyonluGecis : MonoBehaviour
{
    [Header("--- 1. GEREKLİLER ---")]
    public Camera mainCamera;           // Sahnedeki Kamera
    public Transform cameraTarget;      // Omuzdaki "CameraPos" (Hedef)
    public GameObject menuCanvas;       // Menü Paneli
    public Animator characterAnimator;  // Karakter Animator

    [Header("--- 2. AYARLAR ---")]
    public float gecisSuresi = 3.0f;    // Kamera kaç saniyede dönsün?
    public float characterSpeed = 5.0f; // Karakter hızı
    public float mouseSpeed = 2.0f;     // Fare hassasiyeti

    // --- SİGORTA (Şalter) ---
    // Bu 'false' olduğu sürece klavye ve fare ÖLÜDÜR.
    private bool kontrollerAktif = false; 
    
    private float cameraPitch = 0f;

    void Start()
    {
        // 1. ŞALTERİ İNDİR (Her şeyi kilitle)
        kontrollerAktif = false;
        Time.timeScale = 1.0f;

        // 2. Mouse Sadece Menü İçin Serbest
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // BUTONA BU FONKSİYONU BAĞLA
    public void OyunuBaslat()
    {
        // Menüyü Yok Et
        if (menuCanvas != null) menuCanvas.SetActive(false);

        // Karakter Arkasını Dönsün
        if (characterAnimator != null) characterAnimator.SetTrigger("StartGame");

        // --- ADIM 1: KAMERAYI ÖZGÜRLEŞTİR ---
        // Kamerayı karakterden koparıyoruz ki karakter dönerken kamera savrulmasın.
        if (mainCamera != null) mainCamera.transform.SetParent(null);

        // Rotasyonu (Kavisli Hareketi) Başlat
        StartCoroutine(KavisliGecis());
    }

    IEnumerator KavisliGecis()
    {
        float gecenSure = 0f;
        
        Vector3 baslangicKonumu = mainCamera.transform.position;
        Quaternion baslangicRotasyonu = mainCamera.transform.rotation;

        while (gecenSure < gecisSuresi)
        {
            gecenSure += Time.deltaTime;
            float t = gecenSure / gecisSuresi;

            // Smooth (Yumuşak) hareket formülü
            t = t * t * (3f - 2f * t);

            if (cameraTarget != null)
            {
                // --- ADIM 2: KAVİSLİ HAREKET (SLERP) ---
                // Lerp düz gider, Slerp kavis çizer (Rotasyon gibi).
                // Kamera karakterin etrafından dolaşarak omza gider.
                mainCamera.transform.position = Vector3.Slerp(baslangicKonumu, cameraTarget.position, t);
                mainCamera.transform.rotation = Quaternion.Slerp(baslangicRotasyonu, cameraTarget.rotation, t);
            }

            // SİGORTA: Hareket bitene kadar burası 'false' kalmaya zorlanır.
            kontrollerAktif = false;

            yield return null;
        }

        // --- ADIM 3: VARIŞ VE KİLİT AÇMA ---
        if (cameraTarget != null)
        {
            // Tam hizala
            mainCamera.transform.position = cameraTarget.position;
            mainCamera.transform.rotation = cameraTarget.rotation;

            // Kamerayı tekrar karakterin içine sok (Parent yap)
            mainCamera.transform.SetParent(cameraTarget);

            // Açıyı al (Zıplama olmasın)
            cameraPitch = mainCamera.transform.localEulerAngles.x;
            if (cameraPitch > 180) cameraPitch -= 360;
        }

        // Fareyi Kilitle ve Gizle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // --- ŞALTERİ KALDIR ---
        // Artık kontroller çalışabilir!
        kontrollerAktif = true;
        Debug.Log("KAMERA YERLEŞTİ - KONTROLLER AÇILDI");
    }

    void Update()
    {
        // --- GÜVENLİK DUVARI ---
        // Eğer şalter kapalıysa, kod buradan geri döner.
        // Aşağıdaki kodların çalışması FİZİKEN İMKANSIZDIR.
        if (kontrollerAktif == false) return;

        // --- ARTIK OYUN KODLARI ÇALIŞABİLİR ---

        // 1. FARE
        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed;

        transform.Rotate(Vector3.up * mouseX);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -45f, 45f);
        mainCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

        // 2. KLAVYE
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        transform.position += move * characterSpeed * Time.deltaTime;
    }
}