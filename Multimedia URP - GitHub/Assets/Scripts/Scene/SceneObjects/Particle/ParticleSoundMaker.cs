using UnityEngine;

public class ParticleSoundMaker : MonoBehaviour
{
    private ParticleSystem ps;

    [SerializeField] private AudioManager.AudioReferenceClass grabbingObjectSound;

    private int lastParticleCount;

    void Awake(){
        ps = GetComponent<ParticleSystem>();
    }

    void Update(){
        int currentCount = GetAliveParticles();

        if (currentCount > 0 && lastParticleCount == 0) {
            grabbingObjectSound.PlaySoundPosition(transform.position);
        }

        lastParticleCount = currentCount;

    }

    private int GetAliveParticles(){
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
        return ps.GetParticles(particles);
    }
}
