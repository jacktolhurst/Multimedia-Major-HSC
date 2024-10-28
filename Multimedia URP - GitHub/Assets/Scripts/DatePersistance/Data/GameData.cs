using UnityEngine;

[System. Serializable]
public class GameData
{
    public int number;
    public Vector3 playerPosition;

    public GameData(){
        this.number = 0;
        this.playerPosition = Vector3.zero;
    }
}
