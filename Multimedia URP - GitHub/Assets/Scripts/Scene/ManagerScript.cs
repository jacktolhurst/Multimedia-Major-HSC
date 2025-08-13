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
        // SetLightLayerForAll();
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
    }

    private void TurnOffShadows(){ 
        Renderer[] allRenderers = Resources.FindObjectsOfTypeAll<Renderer>()
            .Where(r => r.gameObject.scene.IsValid()).ToArray();

        foreach (Renderer rend in allRenderers){
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    private void SetLightLayerForAll()
    {
        uint lightLayer1Mask = 1u << 1; // Layer 1 mask
        int count = 0;

        foreach (GameObject rootObj in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            count += SetLightLayerRecursive(rootObj, lightLayer1Mask);
        }
    }

    private int SetLightLayerRecursive(GameObject obj, uint lightLayerMask)
    {
        int count = 0;

        // If it has a Renderer, set the mask
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.renderingLayerMask = lightLayerMask;
            count++;
        }

        // Recurse through children
        foreach (Transform child in obj.transform)
        {
            count += SetLightLayerRecursive(child.gameObject, lightLayerMask);
        }

        return count;
    }
}
