using UnityEngine;

public class FogParticleStarter : MonoBehaviour
{
    [SerializeField] private float skipTime;
    void Awake(){
        foreach(Transform child in transform){
            ParticleSystem childParticle = child.gameObject.GetComponent<ParticleSystem>();
            childParticle.Simulate(skipTime);
            childParticle.Play();
        }
    }
}
