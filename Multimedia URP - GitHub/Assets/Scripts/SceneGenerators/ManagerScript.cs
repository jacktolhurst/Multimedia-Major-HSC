using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class ManagerScript : MonoBehaviour
{
    [SerializeField] private int targetFrameRate;

    void Start(){
		Application.targetFrameRate = targetFrameRate;

        Renderer[] allRenderers = Resources.FindObjectsOfTypeAll<Renderer>()
            .Where(r => r.gameObject.scene.IsValid()).ToArray();

        foreach (Renderer rend in allRenderers)
        {
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    void Update(){
        if(Input.GetKey("p")){
            RestartScene();
        }

        if(Application.targetFrameRate != targetFrameRate){
            Application.targetFrameRate = targetFrameRate;
        }
    }

    public void RestartScene(){
        AudioManager.instance.StopAllSounds();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
