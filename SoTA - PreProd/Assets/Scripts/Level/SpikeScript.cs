using UnityEngine;
using System.Collections;
/// <summary>
/// Author:Emil
/// 
/// Modified by: Karin, Gabbriel
/// 
/// </summary>
public class SpikeScript : MonoBehaviour, IActivatable
{
    [SerializeField] bool startsAsActive = true;
    public bool StartsAsActive { get { return startsAsActive; } } //is used to determine polarity of spikes for SFX

    private ParticleSystem spikeParticles;

    private void Start()
    {
        spikeParticles = GetComponentInChildren<ParticleSystem>();
        FindParticleColor();

        if (startsAsActive == true)
        {
            ResetRotation();
        }
        else if (startsAsActive == false)
        {
            Flip();
        }
    }
    public void Activate()
    {
        if (startsAsActive == true)
        {
            StartCoroutine(PlayAndStopParticleBurst());
            Flip();
        }
        else if (startsAsActive == false)
        {
            StartCoroutine(PlayAndStopParticleBurst());
            ResetRotation();
        }
    }

    public void Deactivate()
    {
        if (startsAsActive == true)
        {
            StartCoroutine(PlayAndStopParticleBurst());
            ResetRotation();
        }
        else if (startsAsActive == false)
        {
            StartCoroutine(PlayAndStopParticleBurst());
            Flip();
        }
    }

    private void Flip()
    {
        transform.Rotate(180f, 0f, 0f);
    }

    private void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
    }

    IEnumerator PlayAndStopParticleBurst()
    {
        spikeParticles.Play();
        yield return new WaitForSeconds(0.1f); // wait for particles to spawn
        spikeParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void FindParticleColor()
    {
        Transform spikes = transform.Find("Spikes_new");
        Transform spikeParticlesTransform = transform.Find("SpikesParticles");

        if (spikes != null && spikeParticlesTransform != null)
        {
            MeshRenderer sourceRenderer = spikes.GetComponent<MeshRenderer>();
            ParticleSystemRenderer psRenderer = spikeParticlesTransform.GetComponent<ParticleSystemRenderer>();

            if (sourceRenderer != null && psRenderer != null)
            {
                Material[] materials = sourceRenderer.sharedMaterials;

                if (materials.Length > 1)
                {
                    psRenderer.material = materials[1];
                }
                else
                {
                    Debug.LogWarning("Spikes_new does not have a second material.");
                }
            }
            else
            {
                Debug.LogWarning("MeshRenderer or ParticleSystemRenderer not found.");
            }
        }
        else
        {
            Debug.LogError("Spikes_new or SpikesParticles not found in the hierarchy.");
        }
    }
}
