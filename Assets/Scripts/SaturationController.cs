using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
// URP kütüphanesini daha güvenli bir şekilde çağırıyoruz
using UnityEngine.Rendering.Universal;

public class SaturationController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float targetSaturation = 10f;
    [SerializeField] private float transitionDuration = 2.0f;

    [Header("References")]
    [SerializeField] private Volume globalVolume;

    private ColorAdjustments _colorAdjustments;
    private Coroutine _transitionCoroutine;

    private void Awake()
    {
        InitializeColorAdjustments();
    }

    private void InitializeColorAdjustments()
    {
        if (globalVolume == null)
        {
            Debug.LogError($"[SaturationController] {gameObject.name} üzerinde Global Volume referansı eksik!");
            return;
        }

        // Profile üzerinden güvenli erişim
        if (globalVolume.profile.TryGet(out ColorAdjustments ca))
        {
            _colorAdjustments = ca;
        }
        else
        {
            Debug.LogWarning("[SaturationController] Volume Profile içinde ColorAdjustments bulunamadı.");
        }
    }

    public void StartSaturationTransition()
    {
        if (_colorAdjustments == null) return;

        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);

        _transitionCoroutine = StartCoroutine(IncreaseSaturationRoutine());
    }

    private IEnumerator IncreaseSaturationRoutine()
    {
        float elapsedTime = 0f;
        float startValue = _colorAdjustments.saturation.value;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / transitionDuration;
            
            // SmoothStep kullanarak daha yumuşak bir geçiş sağlıyoruz (Profesyonel dokunuş)
            float smoothTime = Mathf.SmoothStep(0, 1, normalizedTime);
            
            _colorAdjustments.saturation.value = Mathf.Lerp(startValue, targetSaturation, smoothTime);

            yield return null;
        }

        _colorAdjustments.saturation.value = targetSaturation;
        _transitionCoroutine = null;
    }
}