using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Transform cameraTransform;

    bool canSprint = true;

    private float cameraLimit = 0f;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        cameraLimit = Mathf.Clamp(cameraLimit - mouseY, -90f, 90f); 
        cameraTransform.localEulerAngles = Vector3.right * cameraLimit;

        Vector3 moveDirection = (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")).normalized;
        transform.Translate(moveDirection * movementSpeed * Time.deltaTime, Space.World);

        //if ((Si la stamina es 0))
        //{
            // canSprint = false;
            //StartCoroutine(RecoverStamina());
        //}
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift) && canSprint)
        {
            movementSpeed = 10f;
            //Disminuye la "stamina"
        }
        else
        {
            movementSpeed = 5f;
        }
    }
}
