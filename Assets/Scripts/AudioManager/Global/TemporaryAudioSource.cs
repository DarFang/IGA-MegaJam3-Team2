using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TemporaryAudioSource : MonoBehaviour {
    [SerializeField] private AudioSource audioSource;

    [Header("Distance Settings")]
    [SerializeField] private float baseMinDistance = 1f;
    [SerializeField] private float baseMaxDistance = 50f;
    [SerializeField] private float minPowerMultiplier = 0.5f;
    [SerializeField] private float maxPowerMultiplier = 3f;
    [SerializeField] private AnimationCurve powerToDstCurve = AnimationCurve.EaseInOut(0f, 0.5f, 1f, 3f);

    private Action<TemporaryAudioSource> returnToPoolCallback;

    public void Initialize(Action<TemporaryAudioSource> onComplete) {
        returnToPoolCallback = onComplete;
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        // ”становка 3D звукового профил€ по умолчанию
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    public void SetClip(AudioClip audioClip) {
        audioSource.clip = audioClip;
    }

    public void SetupSource(AudioClip clip, Vector3 position, float power = 1.0f, float spatialBlend = 1.0f) {
        transform.position = position;
        audioSource.clip = clip;
        audioSource.spatialBlend = spatialBlend;

        // –асчет рассто€ний в зависимости от силы звука
        CalculateDistancesBasedOnPower(power);

        // –асчет громкости в зависимости от силы
        audioSource.volume = Mathf.Clamp(power, 0.1f, 1.0f);
    }

    /// <summary>
    /// –ассчитывает минимальную и максимальную дистанцию звука в зависимости от силы
    /// </summary>
    /// <param name="power">—ила звука (от 0 до бесконечности, обычно 0-10)</param>
    private void CalculateDistancesBasedOnPower(float power) {
        // Ќормализуем значение силы дл€ использовани€ в кривой (0-1)
        float normalizedPower = Mathf.Clamp01(power / 10f);

        // ѕолучаем множитель из кривой
        float distanceMultiplier = powerToDstCurve.Evaluate(normalizedPower);

        // –асчет дистанций с использованием базовых значений и множител€
        float minDistance = baseMinDistance * Mathf.Lerp(minPowerMultiplier, 1f, normalizedPower);
        float maxDistance = baseMaxDistance * Mathf.Lerp(1f, maxPowerMultiplier, normalizedPower);

        // ѕримен€ем рассчитанные значени€ к аудиоисточнику
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
    }

    public void Play() {
        audioSource.Play();
        ReturnToPoolAfterPlay().Forget();
    }

    public void PlayOneShot(AudioClip clip, float volume = 1.0f) {
        audioSource.PlayOneShot(clip, volume);
        ReturnToPoolAfterPlay().Forget();
    }

    private async UniTaskVoid ReturnToPoolAfterPlay() {
        if (audioSource.clip != null) {
            float delay = audioSource.clip.length / audioSource.pitch;
            await UniTask.Delay(TimeSpan.FromSeconds(delay + 0.1f));
        } else {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
        if (this != null && returnToPoolCallback != null) {
            returnToPoolCallback(this);
        }
    }

    public void Stop() {
        audioSource.Stop();
    }

    public void SetVolume(float volume) {
        audioSource.volume = volume;
    }

    public void SetSpatialBlend(float blend) {
        audioSource.spatialBlend = blend;
    }

    public void SetDistanceSettings(float minDistance, float maxDistance) {
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
    }

    public void ConfigureDistanceCalculation(float newBaseMin, float newBaseMax,
                                           float newMinMultiplier, float newMaxMultiplier,
                                           AnimationCurve newCurve = null) {
        baseMinDistance = newBaseMin;
        baseMaxDistance = newBaseMax;
        minPowerMultiplier = newMinMultiplier;
        maxPowerMultiplier = newMaxMultiplier;

        if (newCurve != null) {
            powerToDstCurve = newCurve;
        }
    }

    public AudioSource GetAudioSource() => audioSource;
}
