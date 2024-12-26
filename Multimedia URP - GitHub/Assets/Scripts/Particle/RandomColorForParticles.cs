using UnityEngine;
using System.Collections.Generic; 

public class RandomColorForParticles : MonoBehaviour
{
    private ParticleSystem particleSystemMain;
    private ParticleSystemRenderer particleRenderer;

    [SerializeField] private List<Material> materials = new List<Material>();

    void Awake(){
        particleSystemMain = GetComponent<ParticleSystem>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();

        particleRenderer.enableGPUInstancing = true;
    }

    void Update(){
        if(particleSystemMain.particleCount == 0){
            particleRenderer.material = materials[Random.Range(0, materials.Count)];
        }
    }
}
