using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;
    private float cameraLimit = 0f;

    [SerializeField] private Transform cameraTransform;
    private Rigidbody rb;
    private Animator animator;

    bool isInCinematic = false;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(!isInCinematic)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            cameraLimit = Mathf.Clamp(cameraLimit - mouseY, -90f, 90f);
            cameraTransform.localEulerAngles = Vector3.right * cameraLimit;

            rb.velocity = transform.forward * movementSpeed * Input.GetAxis("Vertical") + transform.right * movementSpeed * Input.GetAxis("Horizontal");
        }
    }

    IEnumerator TestCinematic1()
    {
        yield return new WaitForSeconds(4f);
        animator.SetBool("LookUp", false);
        isInCinematic = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CinematicTrigger1"))
        {
            isInCinematic = true;
            animator.SetBool("LookUp", true);
            StartCoroutine(TestCinematic1());
        }
    }
}
