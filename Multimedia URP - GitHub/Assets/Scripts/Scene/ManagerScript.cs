using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using DG.Tweening;

public class ManagerScript : MonoBehaviour
{
    public static ManagerScript instance;

    [Range(0,4)]
    [SerializeField] private float baseVolume;

    [SerializeField] private int targetFrameRate;

    void Awake(){
        if(instance != null){
            Debug.LogWarning("Two Manager instances");
        }
        instance = this;

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

    public void LoadNextScene(){
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);

        print(OrbData.objs.Count);
    }

    private void TurnOffShadows(){ 
        Renderer[] allRenderers = Resources.FindObjectsOfTypeAll<Renderer>()
            .Where(r => r.gameObject.scene.IsValid()).ToArray();

        foreach (Renderer rend in allRenderers){
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
