using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using DG.Tweening;

public class ManagerScript : MonoBehaviour
{
    [Range(0,4)]
    [SerializeField] private float baseVolume;

    [SerializeField] private int targetFrameRate;

    void Awake(){
        DOTween.SetTweensCapacity(750, 50);
    }

    void Start(){
		Application.targetFrameRate = targetFrameRate;
        TurnOffShadows();
    }

    void Update(){
        if(Input.GetKeyUp("p")){
            RestartScene();
        }

        AudioManager.instance.ChangeAllVolume(baseVolume);

        if(Application.targetFrameRate != targetFrameRate){
            Application.targetFrameRate = targetFrameRate;
        }
    }

    public void RestartScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void TurnOffShadows(){ 
        Renderer[] allRenderers = Resources.FindObjectsOfTypeAll<Renderer>()
            .Where(r => r.gameObject.scene.IsValid()).ToArray();

        foreach (Renderer rend in allRenderers){
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
