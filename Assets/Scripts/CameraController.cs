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
    float polar = 0;
    float elevation = 0;

    private bool orbiting = false;
    private GameObject orbitTarget;
    private Coroutine orbitCoroutine;
    private Vector2 mousePos;

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

    public void OnCameraSelect(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            RaycastHit hit;
            Ray ray = GetComponent<Camera>().ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.GetComponent<Node>() != null && orbitTarget != hit.transform.gameObject)
                {
                    orbiting = true;
                    orbitTarget = hit.transform.gameObject;

                    if (orbitCoroutine != null)
                        StopCoroutine(orbitCoroutine);
                    orbitCoroutine = StartCoroutine(PositionOrbit());
                }
            }
            else
            {
                if (orbitCoroutine != null)
                    StopCoroutine(orbitCoroutine);

                moveTarget = transform.position;
                rotationTarget = Quaternion.LookRotation(transform.forward, Vector3.up);

                orbiting = false;
                orbitTarget = null;
            }
        }
    }

    public void OnMouseMove(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector2>();
    }


    private void FixedUpdate()
    {
        if (!orbiting)
            moveTarget += (transform.forward * moveDirection.z + transform.right * moveDirection.x) * Time.fixedDeltaTime * internalMoveTargetSpeed;
    }

    private void LateUpdate()
    {
        if (!orbiting)
        {
            transform.position = Vector3.Lerp(transform.position, moveTarget, Time.deltaTime * internalMoveSpeed);

            polar += mouseDelta.x;
            elevation -= mouseDelta.y;
            rotationTarget = Quaternion.AngleAxis(elevation / rotationSpeed, Vector3.right);
            rotationTarget = Quaternion.AngleAxis(polar / rotationSpeed, Vector3.up) * rotationTarget;

            transform.rotation = Quaternion.Slerp(transform.rotation, rotationTarget, Time.deltaTime * internalRotationSpeed);
        }
    }

    private IEnumerator PositionOrbit()
    {
        Vector3 cameraForwardDist = transform.forward * -5;
        while (true)
        {
            
            Vector3 orbitTargetPoint = new Vector3(orbitTarget.transform.position.x + cameraForwardDist.x, 0.0f, orbitTarget.transform.position.z + cameraForwardDist.z);
            Vector3 fromToAngles = Quaternion.FromToRotation(transform.forward, orbitTarget.transform.forward).eulerAngles;

            if ((transform.position - orbitTargetPoint).sqrMagnitude == 0.0f && (fromToAngles.x <= 2f || fromToAngles.x >= 358f) && (fromToAngles.z <= 2f || fromToAngles.z >= 358f))
            {
                print("In orbital position");
                break;
            }

            transform.position = Vector3.MoveTowards(transform.position, orbitTargetPoint, Time.deltaTime * 4);

            rotationTarget = Quaternion.LookRotation(orbitTarget.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotationTarget, Time.deltaTime * 2);

            yield return new WaitForFixedUpdate();
        }

        for (float f = 0.0f; f <= 1.0f; f += 0.1f)
        {
            rotationTarget = Quaternion.LookRotation(orbitTarget.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotationTarget, f);
            yield return new WaitForFixedUpdate();
        }

        while (true)
        {
            transform.RotateAround(orbitTarget.transform.position, Vector3.up, -20 * Time.deltaTime * 2);
            yield return new WaitForFixedUpdate();
        }
    }
}
