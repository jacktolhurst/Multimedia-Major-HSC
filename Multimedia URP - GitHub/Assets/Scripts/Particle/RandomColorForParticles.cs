using UnityEngine;
using System.Collections.Generic; 

public class RandomColorForParticles : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private ParticleSystemRenderer particleRenderer;

    [SerializeField] private List<Material> materials = new List<Material>();

    void Awake(){
        particleSystem = GetComponent<ParticleSystem>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();

        particleRenderer.enableGPUInstancing = true;
    }

    void Update(){
        if(particleSystem.particleCount == 0){
            particleRenderer.material = materials[Random.Range(0, materials.Count)];
        }
    }
}
