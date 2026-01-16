using UnityEngine;
using System.Collections;

public class CameraTransitionController : MonoBehaviour
{
    [Header("--- 1. REFERANSLAR ---")]
    public Camera mainCamera;           
    public Transform playerTransform;   
    public Transform focusTarget;       
    public GameObject menuCanvas;       
    public Animator characterAnimator;  

    [Header("--- 2. BAĞIMSIZ KAMERA AYARLARI ---")]
    public Vector3 cameraOffset = new Vector3(0f, 2.5f, -4.0f); 
    
    [Tooltip("Kamera takibinin yumuşaklığı (Titremeyi engeller)")]
    public float positionSmoothTime = 0.05f; 

    [Header("--- 3. HAREKET AYARLARI ---")]
    public float transitionDuration = 2.5f; 
    public float movementSpeed = 4.0f;      
    public float lookSensitivity = 2.0f;    
    
    [Header("--- 4. AÇI KISITLAMALARI (GERİ GELDİ) ---")]
    [Range(-60, 60)] public float minVerticalAngle = -30f;   // Aşağı bakma sınırı
    [Range(-60, 60)] public float maxVerticalAngle = 35f;    // Yukarı bakma sınırı
    [Range(-90, 90)] public float minHorizontalAngle = -60f; // Sola bakma sınırı
    [Range(-90, 90)] public float maxHorizontalAngle = 60f;  // Sağa bakma sınırı

    // --- GİZLİ DEĞİŞKENLER ---
    private bool isControlActive = false;   
    private float currentPitch = 0f;        
    private float currentYaw = 0f;          
    private Vector3 currentVelocity = Vector3.zero; 

    void Start()
    {
        isControlActive = false;
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (mainCamera != null) mainCamera.transform.SetParent(null);
    }

    public void BeginGameplaySequence()
    {
        if (menuCanvas != null) menuCanvas.SetActive(false);
        if (characterAnimator != null) characterAnimator.SetTrigger("StartGame");
        StartCoroutine(ExecuteTransition());
    }

    IEnumerator ExecuteTransition()
    {
        float timer = 0f;
        Vector3 camStartPos = mainCamera.transform.position;
        Quaternion camStartRot = mainCamera.transform.rotation;
        
        Quaternion charStartRot = transform.rotation;
        Quaternion charTargetRot = Quaternion.Euler(0f, 0f, 0f);

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / transitionDuration;
            progress = progress * progress * (3f - 2f * progress);

            // Intro Hareketi
            Vector3 targetPos = playerTransform.position + playerTransform.TransformDirection(cameraOffset);
            mainCamera.transform.position = Vector3.Lerp(camStartPos, targetPos, progress);

            if (focusTarget != null)
            {
                Quaternion lookRot = Quaternion.LookRotation(focusTarget.position - mainCamera.transform.position);
                mainCamera.transform.rotation = Quaternion.Slerp(camStartRot, lookRot, progress);
            }

            transform.rotation = Quaternion.Slerp(charStartRot, charTargetRot, progress);

            yield return null;
        }

        // --- OYUN BAŞLADI ---
        Vector3 currentAngles = mainCamera.transform.eulerAngles;
        currentPitch = currentAngles.x;
        if (currentPitch > 180) currentPitch -= 360;
        
        currentYaw = currentAngles.y; 
        
        // Başlangıçta açıyı limitlerin içinde tut ki küt diye dönmesin
        currentYaw = Mathf.Clamp(currentYaw, minHorizontalAngle, maxHorizontalAngle);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isControlActive = true; 
    }

    void Update()
    {
        if (!isControlActive) return;

        HandleCharacterMovement();
    }

    void LateUpdate()
    {
        if (!isControlActive || mainCamera == null || playerTransform == null) return;

        // 1. MOUSE İLE AÇI BELİRLE
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        currentYaw += mouseX;
        currentPitch -= mouseY;
        
        // --- KISITLAMALAR BURADA DEVREYE GİRİYOR ---
        // Oyuncu kafasını belirli açılardan fazla çeviremez
        currentYaw = Mathf.Clamp(currentYaw, minHorizontalAngle, maxHorizontalAngle);
        currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);

        // 2. ROTASYONU HESAPLA 
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        // 3. POZİSYONU HESAPLA
        Vector3 targetPosition = playerTransform.position + (rotation * cameraOffset);

        // 4. YUMUŞAT VE UYGULA
        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, targetPosition, ref currentVelocity, positionSmoothTime);
        mainCamera.transform.rotation = rotation;
    }

    void HandleCharacterMovement()
    {
        bool isPressingW = Input.GetKey(KeyCode.W);

        if (isPressingW)
        {
            Vector3 moveDirection = Vector3.forward; 

            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 10.0f))
            {
                if (hit.transform.CompareTag("Yol"))
                {
                    moveDirection = hit.transform.forward;
                }
            }

            moveDirection.y = 0;
            moveDirection.Normalize();

            transform.position += moveDirection * movementSpeed * Time.deltaTime;

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }

            if (characterAnimator != null) characterAnimator.SetBool("isWalking", true);
        }
        else
        {
            if (characterAnimator != null) characterAnimator.SetBool("isWalking", false);
        }
    }
}