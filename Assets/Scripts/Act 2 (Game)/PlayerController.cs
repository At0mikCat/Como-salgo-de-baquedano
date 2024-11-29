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
    public Psychosis psychosis;
    public NavMeshAgent navMeshAgent;

    [Header("Audio")]
    public AudioSource terror0;
    public AudioSource terror1;
    public AudioSource terror2;

    [Header("External")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private Transform cameraTransform;
    private Vector3 checkpointPosition;

    [Header("Phase Timing")]
    [SerializeField] private float phase1Time = 30f;
    [SerializeField] private float phase2Time = 45f;
    [SerializeField] private float phase3Time = 60f;

    [SerializeField] private float duration = 300f;

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
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            transform.Rotate(Vector3.up * mouseX);
            cameraTransform.localEulerAngles = Vector3.right * cameraLimit;
            rb.velocity = transform.forward * movementSpeed * Input.GetAxis("Vertical")
                          + transform.right * movementSpeed * Input.GetAxis("Horizontal");

            timer += Time.deltaTime;

            //final del desmayo
            if(timer >= duration)
            {
                StartCoroutine(End3());
            }

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
        if (other.CompareTag("End1"))
        {
            //final de salida
            StartCoroutine(End1());
        }

        if (other.CompareTag("End2"))
        {
            //final de cabina
            StartCoroutine(End2());
        }

        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(Respawn());
        }

        if (other.CompareTag("StairsUp"))
        {
            movementSpeed = 8f;
        }

        if (other.CompareTag("StairsDown"))
        {
            movementSpeed = 4f;
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
            navMeshAgent.speed = 4.8f;
            navMeshAgent.stoppingDistance = 15f;
        }
        else if (other.CompareTag("Phase2") && currentPhase < 2)
        {
            terror0.Stop();
            terror1.Play();
            currentPhase = 2;
            psychosis.TriggerPsychosis(2);
            navMeshAgent.stoppingDistance = 12f;
            navMeshAgent.speed = 5.25f;
        }
        else if (other.CompareTag("Phase3") && currentPhase < 3)
        {
            terror1.Stop();
            terror2.Play();
            currentPhase = 3;
            psychosis.TriggerPsychosis(3);
            navMeshAgent.stoppingDistance = 8f;
            navMeshAgent.speed = 5.5f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("StairsUp"))
        {
            movementSpeed = 6.2f;
        }

        if (other.CompareTag("StairsDown"))
        {
            movementSpeed = 6.2f;
        }
    }

    IEnumerator End1()
    {
        yield return new WaitForSeconds(1f);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1.8f);
        SceneManager.LoadScene("End1");
    }

    IEnumerator End2()
    {
        yield return new WaitForSeconds(1f);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1.8f);
        SceneManager.LoadScene("End2");
    }

    IEnumerator End3()
    {
        yield return new WaitForSeconds(1f);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1.8f);
        SceneManager.LoadScene("End3");
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
