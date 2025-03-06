using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class ManagerScript : MonoBehaviour
{
    [SerializeField] private float baseVolume;
    [SerializeField] private float newVolume;

    [SerializeField] private int targetFrameRate;

    [Header("Audio")]
    [SerializeField] private string grabObjectName;
    private FMODEvents.SoundEventClass grabObjectSound;

    void Start(){
		Application.targetFrameRate = targetFrameRate;

        grabObjectSound = AudioManager.instance.GetSoundEventClass(grabObjectName);
    }

    void Update(){
        if(Input.GetKey("p")){
            RestartScene();
        }
        if(Input.GetKeyUp("l")){
            grabObjectSound.ChangeVolume(newVolume);
        }
        if(Input.GetKeyUp("k")){
            grabObjectSound.PlaySound(transform.position);
        }
        AudioManager.instance.AllSoundsVolume(baseVolume);

        if(Application.targetFrameRate != targetFrameRate){
            Application.targetFrameRate = targetFrameRate;
        }
    }

    public void RestartScene(){
        AudioManager.instance.StopAllSounds();
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
