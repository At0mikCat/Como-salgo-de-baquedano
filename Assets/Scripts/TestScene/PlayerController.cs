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
    private Rigidbody rb;
    private Animator animator;
    bool isInCinematic = false;

    [Header("External")]
    [SerializeField] private float fadeDuration = 1.5f;
    private float cameraLimit = 0f;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private Transform cameraTransform;
    private Vector3 checkpointPosition;
    public NavMeshAgent navMeshAgent;

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

            rb.velocity = transform.forward * movementSpeed * Input.GetAxis("Vertical") + transform.right * movementSpeed * Input.GetAxis("Horizontal");
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

        if(other.CompareTag("Checkpoint"))
        {
            checkpointPosition = other.transform.position;
        }

        if(other.CompareTag("Intro"))
        {
            isInCinematic = true;
            StartCoroutine(Intro());
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
