using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private GameObject cam;
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

    [Header("FOV")]
    [Range(0, 180)]
    [SerializeField] private float standardFov;
    [SerializeField] private float sprintingFov;
    [SerializeField] private float jumpingFov;

// ----------------------------------------------------------------


    void Awake(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update(){
        MyInput(); 
        FOV();

        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        player.transform.localRotation = Quaternion.Euler(0, cam.transform.localRotation.eulerAngles.y, 0);
    }

    private void MyInput(){
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    private void FOV(){
        cam.GetComponent<Camera>().fieldOfView = standardFov + (Input.GetAxis("Sprint")) * sprintingFov + Mathf.Abs(GetComponent<Rigidbody>().linearVelocity.y) * jumpingFov;
    }

    void OnDrawGizmos(){

    }
}