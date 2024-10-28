using UnityEngine;

public class NumberSaving : MonoBehaviour, IDataPersistence
{
    public int number = 0;

    private void Update(){
        if(Input.GetKeyDown("m")){
            number++;
        }
    }

    public void LoadData(GameData data){
        this.number = data.number;
    }

    public void SaveData(ref GameData data){
        data.number = this.number;
    }
}
