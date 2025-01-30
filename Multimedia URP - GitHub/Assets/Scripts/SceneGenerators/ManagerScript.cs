using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScript : MonoBehaviour
{
    [SerializeField] private int targetFrameRate;

    void Start(){
        QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = targetFrameRate;
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
        SceneManager.LoadScene("OfficeWorks");
    }
}
