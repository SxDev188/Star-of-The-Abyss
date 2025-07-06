using UnityEngine;
using System.Collections;
/// <summary>
/// Author:Karin
/// 
/// Modified by:
/// 
/// </summary>
public class LampScript : MonoBehaviour, IActivatable

{
    [field: SerializeField] public bool IsLit { get; private set; } = false;
    
    private LightTracker tracker;
    private ParticleSystem lampParticles;

    void Start()
    {
        tracker = GameObject.Find("RadialColorManager").GetComponent<LightTracker>();
        tracker.RegisterLightSource(transform);
        lampParticles = GetComponentInChildren<ParticleSystem>();
        FindParticleColor();
    }

    public void Activate()
    {
        if (!IsLit)
        {
            TurnOnLamp();
        }
    }

    private void TurnOnLamp()
    {
        IsLit = true;
        tracker.RefreshLightSources();
        StartCoroutine(PlayAndStopParticleBurst());
    }

    public void Deactivate()
    {
        if (IsLit)
        {
            TurnOffLamp();
        }
    }

    private void TurnOffLamp()
    {
        IsLit = false;
        tracker.RefreshLightSources();
        StartCoroutine(PlayAndStopParticleBurst());
    }

    public void Interact()
    {
        if (!IsLit)
        {
            TurnOnLamp();
        } else if (IsLit)
        {
            TurnOffLamp();
        }
    }

    IEnumerator PlayAndStopParticleBurst()
    {
        lampParticles.Play();
        yield return new WaitForSeconds(0.1f); // wait for particles to spawn
        lampParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void FindParticleColor()
    {
        Transform lightSource = transform.Find("Lamp/Light_source");
        Transform lampParticlesTransform = transform.Find("LampParticles");

        if (lightSource != null && lampParticlesTransform != null)
        {
            MeshRenderer sourceRenderer = lightSource.GetComponent<MeshRenderer>();
            ParticleSystemRenderer psRenderer = lampParticlesTransform.GetComponent<ParticleSystemRenderer>();

            if (sourceRenderer != null && psRenderer != null)
            {
                psRenderer.material = sourceRenderer.sharedMaterial;
            }
            else
            {
                Debug.LogWarning("MeshRenderer or ParticleSystemRenderer not found.");
            }
        }
        else
        {
            Debug.LogError("Light_source or LampParticles not found in the hierarchy.");
        }
    }
}
