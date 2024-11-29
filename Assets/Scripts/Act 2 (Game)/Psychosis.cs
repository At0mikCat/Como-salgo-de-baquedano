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

    [Header("Player Setup")]
    public Transform player;

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

        switch (currentPhase)
        {
            case PsychosisPhase.Phase1:
                StartCoroutine(ApplyPhase1());
                ReplaceObjectsWithEnemies(phase1Objects);
                break;
            case PsychosisPhase.Phase2:
                StartCoroutine(ApplyPhase2());
                ReplaceObjectsWithEnemies(phase2Objects);
                break;
            case PsychosisPhase.Phase3:
                StartCoroutine(ApplyPhase3());
                ReplaceObjectsWithEnemies(phase3Objects);
                break;
        }
    }

    private IEnumerator ApplyPhase1()
    {
        if (grain != null) grain.intensity.value = 0.3f;
        if (vignette != null) vignette.intensity.value = 0.2f;
        if (lensDistortion != null) lensDistortion.intensity.value = 10f;

        yield return new WaitForSeconds(5f);
    }

    private IEnumerator ApplyPhase2()
    {
        if (grain != null) grain.intensity.value = 0.5f;
        if (vignette != null) vignette.intensity.value = 0.4f;
        if (lensDistortion != null) lensDistortion.intensity.value = 20f;
        if (colorGrading != null) colorGrading.saturation.value = -50f;

        yield return new WaitForSeconds(5f);
    }

    private IEnumerator ApplyPhase3()
    {
        if (grain != null) grain.intensity.value = 1f;
        if (vignette != null) vignette.intensity.value = 0.6f;
        if (lensDistortion != null) lensDistortion.intensity.value = 40f;
        if (colorGrading != null) colorGrading.saturation.value = -100f;
        if (depthOfField != null) depthOfField.focusDistance.value = 1f;

        yield return new WaitForSeconds(5f);
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

    public void ResetEffects()
    {
        if (grain != null) grain.intensity.value = 0f;
        if (vignette != null) vignette.intensity.value = 0f;
        if (lensDistortion != null) lensDistortion.intensity.value = 0f;
        if (colorGrading != null) colorGrading.saturation.value = 0f;
        if (depthOfField != null) depthOfField.focusDistance.value = 10f;

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
