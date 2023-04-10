using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Camera cam;
    [SerializeField] private List<Transform> grassColliders;
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private float fadeWhileDash;
    [SerializeField] private float magnetBackToPlayer;
    [SerializeField] private float distFromPlayer;
    [SerializeField] private float gamepadSensitivityX;
    [SerializeField] private float gamepadSensitivityY;
    [SerializeField] private float mouseSensitivityX;
    [SerializeField] private float mouseSensitivityY;
    [SerializeField] private float minYCameraRotation;
    [SerializeField] private float maxYCameraRotation;
    [SerializeField] private float FOV;
    [SerializeField] private float sphereCastRadius;

    private Vector2 rotationVector;
    private Vector2 rotationVectorRange;
    private Vector3 smoothVelocity;
    private Vector3 rotation;
    private RaycastHit hit;
    private float smoothTime = 0.12f;
    private float yaw;
    private float pitch;
    private bool mouseActive = false;
    private bool gamepadActive = true;
    private int gamepadSensitivityScale = 35;
    private float currentDistFromPlayer;
    private bool inDash = false;
    public bool isEating = false;

    public bool InDash { get => inDash; set => inDash = value; }
    public bool IsEating { get => isEating; set => isEating = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            yaw = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>().CurrentRotation.y;
            pitch = 0;
            this.transform.position = player.transform.position - transform.forward * distFromPlayer;
        }

        rotationVectorRange = new Vector2(minYCameraRotation, maxYCameraRotation);
        cam.fieldOfView = FOV;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gamepadSensitivityX *= gamepadSensitivityScale;
        gamepadSensitivityY *= gamepadSensitivityScale;
        currentDistFromPlayer = distFromPlayer;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (gamepadActive)
        {
            yaw += rotationVector.x * gamepadSensitivityX * Time.deltaTime;
            pitch += rotationVector.y * gamepadSensitivityY * Time.deltaTime;
        }
        else if (mouseActive)
        {
            yaw += rotationVector.x * mouseSensitivityX * Time.deltaTime;
            pitch -= rotationVector.y * mouseSensitivityY * Time.deltaTime;
        }

        pitch = Mathf.Clamp(pitch, rotationVectorRange.x, rotationVectorRange.y);

        rotation = Vector3.SmoothDamp(rotation, new Vector3(pitch, yaw), ref smoothVelocity, smoothTime);
        this.transform.eulerAngles = rotation;

        if (inDash)
        {
            if (!Physics.SphereCast(player.transform.position, sphereCastRadius, -this.transform.forward, out hit, Vector3.Distance(player.transform.position, this.transform.position) + (fadeWhileDash * 2), collisionLayer))
                currentDistFromPlayer += fadeWhileDash;            
        }

        else
            currentDistFromPlayer -= magnetBackToPlayer;

        if (currentDistFromPlayer < distFromPlayer)
            currentDistFromPlayer = distFromPlayer;

        transform.position = player.transform.position - transform.forward * currentDistFromPlayer;
        
        if (Physics.Raycast(player.transform.position, -this.transform.forward, out hit, distFromPlayer + 2, collisionLayer))
        {
            if (pitch < 0 && hit.collider.tag == "Ground")
            {
                transform.position = player.transform.position - transform.forward * (hit.distance - 0.75f);
                return;
            }
            else
            {
                if (Physics.SphereCast(player.transform.position, sphereCastRadius, -this.transform.forward, out hit, Vector3.Distance(player.transform.position, this.transform.position), collisionLayer))
                {
                    if (pitch < 0)
                    {
                        transform.position = player.transform.position - transform.forward * (hit.distance - 0.75f);
                        return;
                    }

                    else
                    {
                        transform.position = player.transform.position - transform.forward * (hit.distance - 0.25f);
                        return;
                    }
                }
            }
        }

        transform.position = player.transform.position - transform.forward * currentDistFromPlayer;

        float j = 0.1f;
        for (int i = 0; i < grassColliders.Count; i++, j += 0.1f)
        {
            grassColliders[i].position = Vector3.Lerp(player.transform.position, this.transform.position, j);
        }

    }

    public void Look(InputAction.CallbackContext context)
    {
        rotationVector = context.ReadValue<Vector2>();
        if (context.control.device is Gamepad)
        {
            gamepadActive = true;
            mouseActive = false;
        }
        else
        {
            gamepadActive = false;
            mouseActive = true;
        }
    }
}
