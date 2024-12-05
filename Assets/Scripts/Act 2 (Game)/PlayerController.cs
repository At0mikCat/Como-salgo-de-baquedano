using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private float currentMoveSpeed;

    [Header("Exotic references")]
    public Psychosis psychosis;
    public NavMeshAgent navMeshAgent;
    public GameObject trainLeft;

    [Header("Audio")]
    public AudioSource Latidos;
    public AudioClip Latidos0;
    public AudioClip Latidos1;
    public AudioClip Latidos2;
    public AudioClip Latidos3;

    public AudioSource Resp;
    public AudioClip Resp1;
    public AudioClip Resp2;
    public AudioClip Resp3;

    public AudioSource DialogosPJ;
    public AudioClip DialogoF0;
    public AudioClip DialogoF1;
    public AudioClip DialogoF2;
    public AudioClip DialogoF3;


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

    private bool isMovingToTarget = false;
    private Transform targetPosition; 
    [SerializeField] private float targetMoveSpeed = 3f; 

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleCameraMovement();

        if (isMovingToTarget && targetPosition != null)
        {
            MoveTowardsTarget();
        }
        else if (!isInCinematic)
        {
            HandlePlayerMovement(); 
        }

        HandlePhaseByTime(); 
    }

    private void HandleCameraMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
        cameraTransform.localEulerAngles = Vector3.right * cameraLimit;
    }

    private void HandlePlayerMovement()
    {
        rb.velocity = transform.forward * movementSpeed * Input.GetAxis("Vertical") + transform.right * movementSpeed * Input.GetAxis("Horizontal");
        timer += Time.deltaTime;

        if (timer >= duration)
        {
            StartCoroutine(End3());
        }
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, Time.deltaTime * targetMoveSpeed);

        if (Vector3.Distance(transform.position, targetPosition.position) < 0.1f)
        {
            isMovingToTarget = false;
            targetPosition = null;
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
            // Final de salida
            StartCoroutine(End1());
        }

        if (other.CompareTag("End2"))
        {
            // Final de cabina
            StartCoroutine(End2());
        }

        if (other.CompareTag("Enemy"))
        {
            //StartCoroutine(Respawn());
            SceneManager.LoadScene("End3");
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

        if (other.CompareTag("ActiveTrain"))
        {
            trainLeft.SetActive(true);
        }

        if (other.CompareTag("Phase1") && currentPhase < 1)
        {
            Latidos.clip = Latidos1;
            Resp.clip = Resp1;
            DialogosPJ.clip = DialogoF1;
            currentPhase = 1;
            psychosis.TriggerPsychosis(1);
            navMeshAgent.speed = 4.8f;
            navMeshAgent.stoppingDistance = 15f;
        }
        else if (other.CompareTag("Phase2") && currentPhase < 2)
        {
            Latidos.clip = Latidos2;
            Resp.clip = Resp2;
            DialogosPJ.clip = DialogoF2;
            currentPhase = 2;
            psychosis.TriggerPsychosis(2);
            navMeshAgent.stoppingDistance = 12f;
            navMeshAgent.speed = 5.2f;
        }
        else if (other.CompareTag("Phase3") && currentPhase < 3)
        {
            Latidos.clip = Latidos3;
            Resp.clip = Resp3;
            DialogosPJ.clip = DialogoF3;
            currentPhase = 3;
            psychosis.TriggerPsychosis(3);
        }

        if (other.CompareTag("TriggerLeft1") || other.CompareTag("TriggerLeft2") || other.CompareTag("TriggerLeft3") ||
        other.CompareTag("TriggerRight1") || other.CompareTag("TriggerRight2") || other.CompareTag("TriggerRight3") ||
        other.CompareTag("TriggerSolo1"))
        {
            targetPosition = other.transform.Find("Target");

            if (targetPosition != null)
            {
                if (other.CompareTag("TriggerSolo1"))
                {
                    targetMoveSpeed = 4f; 
                }
                else
                {
                    targetMoveSpeed = 3f; 
                }

                isMovingToTarget = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TriggerLeft1") || other.CompareTag("TriggerLeft2") || other.CompareTag("TriggerLeft3") ||
            other.CompareTag("TriggerRight1") || other.CompareTag("TriggerRight2") || other.CompareTag("TriggerRight3") ||
            other.CompareTag("TriggerSolo1"))
        {
            isMovingToTarget = false;
            targetPosition = null;
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
