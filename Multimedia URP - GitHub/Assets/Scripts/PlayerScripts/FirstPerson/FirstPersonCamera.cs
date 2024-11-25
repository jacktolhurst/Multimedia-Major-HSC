using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private Transform cam;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform player;

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


    void Awake(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update(){
        MyInput(); 

        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        player.localRotation = Quaternion.Euler(0, cam.transform.localRotation.eulerAngles.y, 0);
    }

    private void MyInput(){
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
}