using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private GameObject camObj;
    private Camera cam;
    [SerializeField] private GameObject player;

    [SerializeField] private Transform orientation;

// ----------------------------------------------------------------

    [Header("Sensitivity")]
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;
    private float mouseX;
    private float mouseY;
    private float multiplier = 0.01f;
    private float xRotation;
    private float yRotation;

// ----------------------------------------------------------------

    private Rigidbody rb;
    private Camera UICamera;
    [Header("FOV")]
    [Range(0, 180)]
    public float standardFov;
    [SerializeField] private float sprintingFov;
    [SerializeField] private float jumpingFov;
    [SerializeField] private float fovLerp;
    [SerializeField] float scrollAmount;
    [SerializeField] float minFov;
    [SerializeField] float maxFov;
    private float projectedFov;
    private float lastFov;

// ----------------------------------------------------------------


    void Awake(){
        cam = camObj.GetComponent<Camera>();
        UICamera = cam.gameObject.transform.GetChild(0).GetComponent<Camera>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        projectedFov = cam.fieldOfView;

        rb = GetComponent<Rigidbody>();
    }

    void Update(){
        MyInput(); 
        FOV();

        camObj.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        player.transform.localRotation = Quaternion.Euler(0, camObj.transform.localRotation.eulerAngles.y, 0);
    }

    private void MyInput(){
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    private void FOV(){
        // for scrollwheel
        standardFov -= Input.GetAxis("Mouse ScrollWheel") * scrollAmount;
        standardFov = Mathf.Clamp(standardFov, minFov, maxFov);

        // for movement
        projectedFov = standardFov + ((Input.GetAxis("Sprint")) * sprintingFov) + (Mathf.Abs(rb.linearVelocity.y) * jumpingFov);
        if(lastFov != projectedFov){
            lastFov = cam.fieldOfView = UICamera.fieldOfView = Mathf.Lerp(cam.fieldOfView, projectedFov, fovLerp);
        }
    }
}