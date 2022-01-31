using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float lookOffset, cameraAngle, defaultZoom, zoomMax, zoomMin, rotationSpeed;
    [SerializeField]
    private float internalMoveTargetSpeed = 8;
    [SerializeField]
    private float internalMoveSpeed = 4;

    private Vector3 cameraPosTarget;

    private Vector3 moveTarget;
    private Vector3 moveDirection;

    private bool rmbDown = false;
    [SerializeField]
    private float internalRotationSpeed = 4;
    private Quaternion rotationTarget;
    private Vector2 mouseDelta;

    private void Start()
    {
        transform.rotation = Quaternion.AngleAxis(cameraAngle, Vector3.right);
        rotationTarget = transform.rotation;

        cameraPosTarget = (Vector3.up * lookOffset) + (Quaternion.AngleAxis(cameraAngle, Vector3.right) * Vector3.back) * defaultZoom;
        transform.position = cameraPosTarget;
        moveTarget = cameraPosTarget;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        moveDirection = new Vector3(value.x, 0, value.y);

    }

    public void OnRotateToggle(InputAction.CallbackContext context)
    {
        rmbDown = context.ReadValue<float>() == 1;
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        mouseDelta = rmbDown ? context.ReadValue<Vector2>() : Vector2.zero;
    }

    private void FixedUpdate()
    {
        moveTarget += (transform.forward * moveDirection.z + transform.right * moveDirection.x) * Time.fixedDeltaTime * internalMoveTargetSpeed;
    }

    float polar = 0;
    float elevation = 0;
    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, moveTarget, Time.deltaTime * internalMoveSpeed);

        polar += mouseDelta.x;
        elevation -= mouseDelta.y;
        rotationTarget = Quaternion.AngleAxis(elevation / rotationSpeed, Vector3.right);
        rotationTarget = Quaternion.AngleAxis(polar / rotationSpeed, Vector3.up) * rotationTarget;
        //rotationTarget *= Quaternion.AngleAxis(mouseDelta.x * Time.deltaTime * rotationSpeed, Vector3.up);
        //rotationTarget *= Quaternion.AngleAxis(mouseDelta.y * Time.deltaTime * rotationSpeed, Vector3.left);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotationTarget, Time.deltaTime * internalRotationSpeed);
    }
}
