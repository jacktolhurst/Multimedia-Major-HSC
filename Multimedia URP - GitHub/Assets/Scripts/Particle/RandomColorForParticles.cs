using UnityEngine;

public class RandomColorForParticles : MonoBehaviour
{
    private ParticleSystemRenderer particleRenderer;
    public Material tempMat;

    void Awake(){
        particleRenderer = GetComponent<ParticleSystemRenderer>();

        particleRenderer.enableGPUInstancing = true;
        particleRenderer.material = tempMat;
    }
}
