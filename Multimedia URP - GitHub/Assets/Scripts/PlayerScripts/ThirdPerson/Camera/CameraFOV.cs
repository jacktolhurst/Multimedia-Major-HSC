using UnityEngine;
using Cinemachine;

public class CameraFOV : MonoBehaviour
{
    // * camera
    public CinemachineFreeLook cam;

    // * floats and ints
    float fov = 40;
    public float scrollAmout;
    public float minFov;
    public float maxFov;

    void Update(){
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        fov -= scroll * scrollAmout;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        cam.m_Lens.FieldOfView = fov;
    }

}
