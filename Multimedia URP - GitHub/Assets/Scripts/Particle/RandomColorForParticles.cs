using UnityEngine;
using System.Collections.Generic; 

public class RandomColorForParticles : MonoBehaviour
{
    private ParticleSystemRenderer particleRenderer;

    [SerializeField] private List<Material> materials = new List<Material>();

    void Awake(){
        particleRenderer = GetComponent<ParticleSystemRenderer>();

        particleRenderer.enableGPUInstancing = true;
    }
}
