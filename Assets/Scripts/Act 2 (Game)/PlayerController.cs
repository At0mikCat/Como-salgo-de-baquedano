using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("PlayerController")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;
    private float cameraLimit = 0f;
    private Rigidbody rb;
    private Animator animator;
    bool isInCinematic = false;

    [Header("Exotic references")]
    public NavMeshAgent navMeshAgent;
    public Psychosis psychosis;

    [Header("External")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private Transform cameraTransform;
    private Vector3 checkpointPosition;

    [Header("Phase Timing")]
    [SerializeField] private float phase1Time = 30f;
    [SerializeField] private float phase2Time = 45f;
    [SerializeField] private float phase3Time = 60f;

    [SerializeField] private float timer = 0f;
    private int currentPhase = 0; 

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isInCinematic)
        {
            // Movimiento del jugador
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            transform.Rotate(Vector3.up * mouseX);
            cameraTransform.localEulerAngles = Vector3.right * cameraLimit;
            rb.velocity = transform.forward * movementSpeed * Input.GetAxis("Vertical")
                          + transform.right * movementSpeed * Input.GetAxis("Horizontal");

            timer += Time.deltaTime;

            HandlePhaseByTime();
        }
    }

    private void HandlePhaseByTime()
    {
        if (currentPhase < 1 && timer >= phase1Time)
        {
            currentPhase = 1;
            psychosis.TriggerPsychosis(1);
        }
        else if (currentPhase < 2 && timer >= phase2Time)
        {
            currentPhase = 2;
            psychosis.TriggerPsychosis(2);
        }
        else if (currentPhase < 3 && timer >= phase3Time)
        {
            currentPhase = 3;
            psychosis.TriggerPsychosis(3);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Win"))
        {
            isInCinematic = true;
            StartCoroutine(TestWin());
        }

        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(Respawn());
        }

        if (other.CompareTag("Checkpoint"))
        {
            checkpointPosition = other.transform.position;
        }

        if (other.CompareTag("Intro"))
        {
            isInCinematic = true;
            StartCoroutine(Intro());
        }

        if (other.CompareTag("Phase1") && currentPhase < 1)
        {
            currentPhase = 1;
            psychosis.TriggerPsychosis(1);
        }
        else if (other.CompareTag("Phase2") && currentPhase < 2)
        {
            currentPhase = 2;
            psychosis.TriggerPsychosis(2);
        }
        else if (other.CompareTag("Phase3") && currentPhase < 3)
        {
            currentPhase = 3;
            psychosis.TriggerPsychosis(3);
        }
    }

    IEnumerator TestWin()
    {
        yield return new WaitForSeconds(3f);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1.8f);
        SceneManager.LoadScene("Main Menu");
    }

    IEnumerator Respawn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1.8f);
        gameObject.transform.position = checkpointPosition;

        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }
    }

    IEnumerator Intro()
    {
        animator.enabled = true;
        animator.SetTrigger("Intro");
        yield return new WaitForSeconds(2.2f);
        animator.enabled = false;
        isInCinematic = false;
    }

}
