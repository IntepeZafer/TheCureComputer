using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Gerekli Bileşenler")]
    public Animator playerAnimator; // Karakterin Animator'ı
    public Transform player;        // Karakterin kendisi (Transform)
    
    [Header("Kamera Hedef Ayarları")]
    public Camera mainCamera;
    
    [Tooltip("Kamera oyuncunun neresinde dursun? (Sağ/Sol, Yukarı, İleri/Geri)")]
    public Vector3 targetLocalPosition = new Vector3(0.6f, 1.8f, -2.5f); // Omuz hizası
    
    [Tooltip("Kamera kaç derece aşağı/yukarı baksın? (X=15 hafif aşağı bakar)")]
    public Vector3 targetLocalRotation = new Vector3(10f, 0f, 0f); // Hafif aşağı eğim
    
    [Header("Süre")]
    public float transitionDuration = 1.5f; // Geçiş kaç saniye sürsün?

    [Header("Diğerleri")]
    public GameObject menuCanvas;
    public MonoBehaviour playerMovementScript;

    public void StartGameSequence()
    {
        // 1. Menüyü kapat
        if (menuCanvas != null) menuCanvas.SetActive(false);

        // 2. Karakter animasyonunu başlat (Dönsün)
        if (playerAnimator != null) playerAnimator.SetTrigger("StartGame");

        // 3. Kamerayı karakterden kopar (Bağımsız olsun ki karakter dönerken saçmalamasın)
        mainCamera.transform.SetParent(null); 

        // 4. Hareketi başlat
        StartCoroutine(MoveAndRotateCamera());
    }

    IEnumerator MoveAndRotateCamera()
    {
        float elapsed = 0f;

        // BAŞLANGIÇ DEĞERLERİ
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            
            // Daha sinematik, yumuşak hızlanma/yavaşlama (SmoothStep)
            t = t * t * (3f - 2f * t); 

            // --- A. POZİSYON HESABI ---
            // Oyuncunun o anki konumuna göre hedef noktayı buluyoruz
            Vector3 targetWorldPos = player.TransformPoint(targetLocalPosition);
            mainCamera.transform.position = Vector3.Lerp(startPos, targetWorldPos, t);

            // --- B. ROTASYON HESABI ---
            // Oyuncunun o anki yönüne, senin istediğin eğimi (targetLocalRotation) ekliyoruz.
            // Örn: Oyuncu nereye bakarsa baksın, kamera onun baktığı yerin 10 derece altına bakar.
            Quaternion finalTargetRot = player.rotation * Quaternion.Euler(targetLocalRotation);
            
            // Yumuşakça o açıya dön
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, finalTargetRot, t);

            yield return null;
        }

        // --- BİTİŞ ---
        // 1. Tam hizala
        mainCamera.transform.position = player.TransformPoint(targetLocalPosition);
        mainCamera.transform.rotation = player.rotation * Quaternion.Euler(targetLocalRotation);

        // 2. Kamerayı karakterin çocuğu yap (Artık karakterle beraber gezecek)
        mainCamera.transform.SetParent(player);

        // 3. Kontrolleri aç
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;
    }

    public void QuitGame() { Application.Quit(); }
}