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

    [Header("Hallucination Texts")]
    [SerializeField] private List<GameObject> hallucinationTexts; 
    [SerializeField] private float minFlashDuration = 0.1f; 
    [SerializeField] private float maxFlashDuration = 0.5f; 

    [SerializeField] private float minIntervalPhase2 = 2f; 
    [SerializeField] private float maxIntervalPhase2 = 4f; 
    [SerializeField] private float minIntervalPhase3 = 0.5f; 
    [SerializeField] private float maxIntervalPhase3 = 2f; 

    private Coroutine hallucinationCoroutine; 

    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<GameObject> deactivatedObjects = new List<GameObject>();

    private List<GameObject> phase1Objects = new List<GameObject>();
    private List<GameObject> phase2Objects = new List<GameObject>();
    private List<GameObject> phase3Objects = new List<GameObject>();

    private void Awake()
    {
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
                StartCoroutine(TransitionEffects(0.3f, 0.2f, 10f, Vector4.zero));
                ReplaceObjectsWithEnemies(phase1Objects);
                break;

            case PsychosisPhase.Phase2:
                StartCoroutine(TransitionEffects(0.5f, 0.4f, 20f, new Vector4(-0.2f, -0.2f, -0.2f, 0f)));
                ReplaceObjectsWithEnemies(phase2Objects);
                hallucinationCoroutine = StartCoroutine(FlashHallucinations());
                break;

            case PsychosisPhase.Phase3:
                StartCoroutine(TransitionEffects(1f, 0.6f, 40f, new Vector4(0.5f, 0.1f, 0.3f, 0f)));
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
    private IEnumerator TransitionEffects(float targetGrain, float targetVignette, float targetLensDistortion, Vector4 targetLift)
    {
        float duration = 2f; 
        float elapsed = 0f;

        float initialGrain = grain?.intensity.value ?? 0f;
        float initialVignette = vignette?.intensity.value ?? 0f;
        float initialLensDistortion = lensDistortion?.intensity.value ?? 0f;
        Vector4 initialLift = colorGrading?.lift.value ?? Vector4.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (grain != null) grain.intensity.value = Mathf.Lerp(initialGrain, targetGrain, t);
            if (vignette != null) vignette.intensity.value = Mathf.Lerp(initialVignette, targetVignette, t);
            if (lensDistortion != null) lensDistortion.intensity.value = Mathf.Lerp(initialLensDistortion, targetLensDistortion, t);
            if (colorGrading != null) colorGrading.lift.value = Vector4.Lerp(initialLift, targetLift, t);

            yield return null;
        }

        if (grain != null) grain.intensity.value = targetGrain;
        if (vignette != null) vignette.intensity.value = targetVignette;
        if (lensDistortion != null) lensDistortion.intensity.value = targetLensDistortion;
        if (colorGrading != null) colorGrading.lift.value = targetLift;
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

    public void ResetEffects()
    {
        StartCoroutine(TransitionEffects(0f, 0f, 0f, Vector4.zero));

        if (hallucinationCoroutine != null)
        {
            StopCoroutine(hallucinationCoroutine);
            hallucinationCoroutine = null;
        }

        ClearEnemy();
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
