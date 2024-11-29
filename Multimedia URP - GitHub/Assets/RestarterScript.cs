using UnityEngine;
using UnityEngine.SceneManagement;

public class RestarterScript : MonoBehaviour
{
    void Update(){
        if(Input.GetKey("p")){
            SceneManager.LoadScene("OfficeWorks");
        }
    }
}
