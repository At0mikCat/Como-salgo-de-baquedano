using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Transform cameraTransform;
    private Rigidbody rb;

    //bool canSprint = true;

    private float cameraLimit = 0f;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        cameraLimit = Mathf.Clamp(cameraLimit - mouseY, -90f, 90f); 
        cameraTransform.localEulerAngles = Vector3.right * cameraLimit;

        rb.velocity = transform.forward * movementSpeed * Input.GetAxis("Vertical") + transform.right * movementSpeed * Input.GetAxis("Horizontal");
    }

    //private void FixedUpdate()
    //{
    //    if (Input.GetKey(KeyCode.LeftShift) && canSprint)
    //    {
    //        movementSpeed = 6f;
    //        //Disminuye la "stamina"
    //    }
    //    else
    //    {
    //        movementSpeed = 3.5f;
    //    }
    //}
}
