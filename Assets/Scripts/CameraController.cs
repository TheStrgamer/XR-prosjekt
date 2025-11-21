using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;
    public float verticalSpeed = 5f;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalInput;

    [Header("Dig Settings")]
    public float digRadius = 3f;
    public float digStrength = 3f;

    public float coolDown = 0.2f;
    private float currentCoolDown = 0.2f;

    [Header("CritPoint Settings")]
    private bool pointActive = false;
    [SerializeField] private float critBoost = 1.25f;
    [SerializeField] private float critPointLifetime = 5f;
    private float currentCritLife;
    [SerializeField] private GameObject currentCritPoint;
    [SerializeField] private float maxDistToPoint = 0.4f;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();   
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        HandleMovement();
        HandleLook();
        HandleDig();

        currentCritLife -= Time.deltaTime;
        if (currentCritLife < 0)
        {
            currentCritPoint.SetActive(false);
            currentCritLife = 0;
            pointActive = false;
        }
    }

    private void HandleMovement()
    {
        Vector3 horizontalMove = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 verticalMove = Vector3.up * verticalInput;
        transform.position += (horizontalMove * moveSpeed + verticalMove * verticalSpeed) * Time.deltaTime;
    }
    private void HandleLook()
    {
        transform.Rotate(Vector3.up, lookInput.x * lookSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.left, lookInput.y * lookSpeed * Time.deltaTime, Space.Self);
    }
    public bool TryPlaceCritPoint(float maxDistance = 10f, int attempts = 10)
    {
        Debug.Log(maxDistance + " "+ attempts);

        for (int i = 0; i < attempts; i++)
        {
            Vector3 direction =
                cam.transform.forward +
                cam.transform.right * Random.Range(-0.25f, 0.25f) +
                cam.transform.up * Random.Range(-0.2f, 0.2f);

            direction.Normalize();

            if (Physics.Raycast(cam.transform.position, direction, out RaycastHit hit, maxDistance))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    currentCritPoint.transform.position = hit.point;
                    currentCritPoint.SetActive(true);
                    pointActive = true;
                    currentCritLife = critPointLifetime;
                    return true;
                }
            }
        }

        return false;
    }

    private void HandleDig()
    {
        currentCoolDown -= Time.deltaTime;
        if (Mouse.current.leftButton.isPressed && currentCoolDown <= 0)
        {
            currentCoolDown = coolDown;
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Collider[] colliders = Physics.OverlapSphere(hit.point, digRadius);
                foreach (Collider c in colliders)
                {
                    //Debug.Log(c.gameObject.name);
                    MarchingCubes mc = c.GetComponent<MarchingCubes>();
                    if (mc != null)
                    {
                        if (Vector3.Distance(c.transform.position, currentCritPoint.transform.position) < maxDistToPoint)
                        {
                            mc.Dig(hit.point, digRadius * critBoost, digStrength * critBoost);
                        }
                        else
                        {
                            mc.Dig(hit.point, digRadius, digStrength);
                        }
                        TryPlaceCritPoint(10,10);
                    }
                }
            }
        }
    }
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    public void OnUpDown(InputValue value) => verticalInput = value.Get<float>();
}
