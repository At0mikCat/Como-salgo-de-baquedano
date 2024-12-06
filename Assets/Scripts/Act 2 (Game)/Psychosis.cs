using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public enum PsychosisPhase { None, Phase1, Phase2, Phase3 }
public class Psychosis : MonoBehaviour
{
    public PsychosisPhase currentPhase = PsychosisPhase.None;

    [Header("Post-Processing Effects")]
    [SerializeField] private PostProcessVolume postProcessVolume;
    [SerializeField] private float offset = 1.5f;

    private DepthOfField depthOfField;
    private LensDistortion lensDistortion;
    private Grain grain;
    private Vignette vignette;
    private ColorGrading colorGrading;

    [Header("Enemy Setup")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Phase Object Parents")]
    [SerializeField] private Transform phase1Parent;
    [SerializeField] private Transform phase2Parent;
    [SerializeField] private Transform phase3Parent;

    public Transform player;

    [SerializeField] private Transform hallucinationParent;
    [SerializeField] private float minFlashDuration = 0.1f;
    [SerializeField] private float maxFlashDuration = 0.3f;

    [SerializeField] private float minIntervalPhase2 = 1f; 
    [SerializeField] private float maxIntervalPhase2 = 2f;

    [SerializeField] private float minIntervalPhase3 = 0.3f; 
    [SerializeField] private float maxIntervalPhase3 = 1f;

    private List<GameObject> hallucinationTexts = new List<GameObject>();

    private Coroutine hallucinationCoroutine; 

    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<GameObject> deactivatedObjects = new List<GameObject>();

    private List<GameObject> phase1Objects = new List<GameObject>();
    private List<GameObject> phase2Objects = new List<GameObject>();
    private List<GameObject> phase3Objects = new List<GameObject>();

    private void Awake()
    {
        if (hallucinationParent != null)
        {
            foreach (Transform child in hallucinationParent)
            {
                hallucinationTexts.Add(child.gameObject);
                child.gameObject.SetActive(false); 
            }
        }

        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGetSettings(out depthOfField);
            postProcessVolume.profile.TryGetSettings(out lensDistortion);
            postProcessVolume.profile.TryGetSettings(out grain);
            postProcessVolume.profile.TryGetSettings(out vignette);
            postProcessVolume.profile.TryGetSettings(out colorGrading);
        }

        FillPhaseObjects(phase1Parent, phase1Objects);
        FillPhaseObjects(phase2Parent, phase2Objects);
        FillPhaseObjects(phase3Parent, phase3Objects);
    }

    private void Update()
    {
        UpdateEnemiesLookAt();
    }

    public void TriggerPsychosis(int phase)
    {
        currentPhase = (PsychosisPhase)phase;

        if (hallucinationCoroutine != null)
        {
            StopCoroutine(hallucinationCoroutine);
            hallucinationCoroutine = null;
        }

        switch (currentPhase)
        {
            case PsychosisPhase.Phase1:
                StartCoroutine(TransitionEffects(0.4f, 0.3f, 15f, new Vector4(0.05f, -0.05f, 0.05f, 0f), new Vector4(0.1f, 0.0f, -0.05f, 0f), new Vector4(0.0f, 0.0f, 0.15f, 0f), 10f, 2.8f));
                ReplaceObjectsWithEnemies(phase1Objects);
                break;

            case PsychosisPhase.Phase2:
                StartCoroutine(TransitionEffects(0.4f, 0.6f, 15f, new Vector4(0.05f, -0.05f, 0.05f, 0f), new Vector4(0.1f, 0.0f, -0.05f, 0f), new Vector4(0.0f, 0.0f, 0.15f, 0f), 10f, 2.8f));
                ReplaceObjectsWithEnemies(phase2Objects);
                hallucinationCoroutine = StartCoroutine(FlashHallucinations());
                break;

            case PsychosisPhase.Phase3:
                StartCoroutine(TransitionEffects(0.7f, 0.65f, 30f, new Vector4(0.1f, -0.05f, 0.05f, 0f), new Vector4(0.2f, 0.1f, -0.05f, 0f), new Vector4(0.1f, 0.1f, 0.1f, 0f), 5f, 4f));
                ReplaceObjectsWithEnemies(phase3Objects);
                hallucinationCoroutine = StartCoroutine(FlashHallucinations());
                break;
        }
    }

    private IEnumerator FlashHallucinations()
    {
        while (currentPhase == PsychosisPhase.Phase2 || currentPhase == PsychosisPhase.Phase3)
        {
            if (hallucinationTexts.Count > 0)
            {
                int randomIndex = Random.Range(0, hallucinationTexts.Count);
                GameObject hallucinationText = hallucinationTexts[randomIndex];

                if (hallucinationText != null)
                {
                    hallucinationText.SetActive(true); 
                    yield return new WaitForSeconds(Random.Range(minFlashDuration, maxFlashDuration));
                    hallucinationText.SetActive(false); 
                }
            }

            float minInterval = currentPhase == PsychosisPhase.Phase2 ? minIntervalPhase2 : minIntervalPhase3;
            float maxInterval = currentPhase == PsychosisPhase.Phase2 ? maxIntervalPhase2 : maxIntervalPhase3;

            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        }

        foreach (var text in hallucinationTexts)
        {
            if (text != null)
                text.SetActive(false);
        }
    }

    private IEnumerator TransitionEffects(float targetGrain, float targetVignette, float targetLensDistortion, Vector4 targetLift, Vector4 targetGamma, Vector4 targetGain, float targetFocusDistance, float targetAperture)
    {
        float duration = 2f;
        float elapsed = 0f;

        float initialGrain = grain?.intensity.value ?? 0f;
        float initialVignette = vignette?.intensity.value ?? 0f;
        float initialLensDistortion = lensDistortion?.intensity.value ?? 0f;
        Vector4 initialLift = colorGrading?.lift.value ?? Vector4.zero;
        Vector4 initialGamma = colorGrading?.gamma.value ?? Vector4.zero;
        Vector4 initialGain = colorGrading?.gain.value ?? Vector4.zero;

        float initialFocusDistance = depthOfField?.focusDistance.value ?? 0f;
        float initialAperture = depthOfField?.aperture.value ?? 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (grain != null) grain.intensity.value = Mathf.Lerp(initialGrain, targetGrain, t);
            if (vignette != null) vignette.intensity.value = Mathf.Lerp(initialVignette, targetVignette, t);
            if (lensDistortion != null) lensDistortion.intensity.value = Mathf.Lerp(initialLensDistortion, targetLensDistortion, t);
            if (colorGrading != null)
            {
                colorGrading.lift.value = Vector4.Lerp(initialLift, targetLift, t);
                colorGrading.gamma.value = Vector4.Lerp(initialGamma, targetGamma, t);
                colorGrading.gain.value = Vector4.Lerp(initialGain, targetGain, t);
            }
            if (depthOfField != null)
            {
                depthOfField.focusDistance.value = Mathf.Lerp(initialFocusDistance, targetFocusDistance, t);
                depthOfField.aperture.value = Mathf.Lerp(initialAperture, targetAperture, t);
            }

            yield return null;
        }

        if (grain != null) grain.intensity.value = targetGrain;
        if (vignette != null) vignette.intensity.value = targetVignette;
        if (lensDistortion != null) lensDistortion.intensity.value = targetLensDistortion;
        if (colorGrading != null)
        {
            colorGrading.lift.value = targetLift;
            colorGrading.gamma.value = targetGamma;
            colorGrading.gain.value = targetGain;
        }
        if (depthOfField != null)
        {
            depthOfField.focusDistance.value = targetFocusDistance;
            depthOfField.aperture.value = targetAperture;
        }
    }


    private void UpdateEnemiesLookAt()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy == null) continue;

            Vector3 direction = (player.position - enemy.transform.position).normalized;
            direction.y = 0;

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void ReplaceObjectsWithEnemies(List<GameObject> objects)
    {
        foreach (var obj in objects)
        {
            if (obj == null) continue;

            obj.SetActive(false);
            deactivatedObjects.Add(obj);

            Vector3 position = obj.transform.position;
            Quaternion rotation = obj.transform.rotation;

            GameObject enemy = Instantiate(enemyPrefab, position, rotation);
            enemy.transform.position = position + Vector3.down * offset;

            activeEnemies.Add(enemy);
        }
    }

    private void ClearEnemy()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null) Destroy(enemy);
        }
        activeEnemies.Clear();

        foreach (var obj in deactivatedObjects)
        {
            if (obj != null) obj.SetActive(true);
        }
        deactivatedObjects.Clear();
    }

    private void FillPhaseObjects(Transform parent, List<GameObject> phaseObjects)
    {
        if (parent != null)
        {
            phaseObjects.Clear();
            foreach (Transform child in parent)
            {
                phaseObjects.Add(child.gameObject);
            }
        }
    }
}
