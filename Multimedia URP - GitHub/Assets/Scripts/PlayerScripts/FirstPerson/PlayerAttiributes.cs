using UnityEngine;

public class PlayerAttiributes : MonoBehaviour
{

    private ManagerScript managerScript;

    [SerializeField] private int maxHealth;
    [HideInInspector] public int currentHealth;

    void Awake(){
        GameObject[] allGameObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);;
        foreach(GameObject obj in allGameObjects){
            if(obj.GetComponent<ManagerScript>() != null){
                managerScript = obj.GetComponent<ManagerScript>();
                break;
            }
        }
        if(managerScript == null){
            Debug.LogError("No manager script found");
        }

        currentHealth = maxHealth;
    }

    void LateUpdate(){
        if(currentHealth <= 0){
            managerScript.RestartScene();
        }
    }
}
