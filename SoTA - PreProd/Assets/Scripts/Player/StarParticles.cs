using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author:Karin
/// 
/// Modified by: Gabbriel
/// 
/// </summary>
public class StarParticles : MonoBehaviour
{
    //COMPONENTS ============================================ //
    [SerializeField] PlayerStarActionController playerStarActionController;
    [SerializeField] Transform playerTransform;
    [SerializeField] StarActions starActions;
    private LineRenderer gravityPullLine;
    private ParticleSystem gravityPullParticles;
    private ParticleSystem recallParticles;
    private ParticleSystem recallParticlesBurst;
    private ParticleSystem trailParticles;

    // STORING/VALUE VARIABLES ============================== //
    private float gravityPullRange;
    private float recallRange;
    private bool isBeingGravityPulled;
    private bool isOnPlayer;
    private bool wasOnPlayer;

    private EventInstance starShimmerSFX;

    // ENGINE METHODS ====================================== // 

    void Start()
    {
        playerStarActionController = FindObjectOfType<PlayerStarActionController>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        starActions = GetComponent<StarActions>();

        isBeingGravityPulled = playerStarActionController.IsBeingGravityPulled;
        isOnPlayer = starActions.IsOnPlayer;
        gravityPullRange = playerStarActionController.GravityPullRange;
        recallRange = playerStarActionController.RecallRange;

        FindParticleSystems();

        FindAndInitializeLineRenderer();

        starShimmerSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.StarShimmerSFX);
    }

    private void FindAndInitializeLineRenderer()
    {
        gravityPullLine = transform.Find("GravityLine").GetComponent<LineRenderer>();
        gravityPullLine.positionCount = 2; // Two points: player and star
        gravityPullLine.enabled = false; // Start disabled
        gravityPullLine.numCapVertices = 10; // Extra vertices for smoother look
        gravityPullLine.startWidth = 1f; // Start width
        gravityPullLine.endWidth = 1f; // Adjust the end width as needed
    }

    private void FindParticleSystems()
    {
        var particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            if (ps.gameObject.name.Equals("GravityParticles", System.StringComparison.OrdinalIgnoreCase))
                gravityPullParticles = ps;
            else if (ps.gameObject.name.Equals("RecallParticles", System.StringComparison.OrdinalIgnoreCase))
                recallParticles = ps;
            else if (ps.gameObject.name.Equals("TrailParticles", System.StringComparison.OrdinalIgnoreCase))
                trailParticles = ps;
            else if (ps.gameObject.name.Equals("RecallBurst", System.StringComparison.OrdinalIgnoreCase))
                recallParticlesBurst = ps;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        isOnPlayer = starActions.IsOnPlayer;

        ApplyGravityParticles(distanceToPlayer);
        ApplyRecallParticles(distanceToPlayer);
        ParticleTrailWhenThrown();
        GravityPullBeam(distanceToPlayer);
        RecallParticleBurst();

        UpdateStarShimmerSFX(distanceToPlayer);
    }

    private void RecallParticleBurst()
    {
        if (!wasOnPlayer && isOnPlayer)
        {
            StartCoroutine(PlayAndStopBurst());
        }

        wasOnPlayer = isOnPlayer;

        IEnumerator PlayAndStopBurst()
        {
            recallParticlesBurst.Play();
            yield return new WaitForSeconds(0.1f); // wait for particles to spawn
            recallParticlesBurst.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void ApplyGravityParticles(float distanceToPlayer)
    {
        if (!starActions.IsOnPlayer) // Applies the particle effects only if the Star isn't held by player
        {
            if (distanceToPlayer <= gravityPullRange)
            {
                if (gravityPullParticles && !gravityPullParticles.isPlaying)
                    gravityPullParticles.Play();
            }
            else
            {
                if (gravityPullParticles && gravityPullParticles.isPlaying)
                    gravityPullParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }           
        }
        else // If player is holding Star, the gravityPull particles get stopped and cleared but recallParticles continue
        {
            gravityPullParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void ApplyRecallParticles(float distanceToPlayer)
    {
        if (!starActions.IsOnPlayer) // Applies the particle effects only if the Star isn't held by player
        {
            if (distanceToPlayer <= recallRange)
            {
                if (recallParticles && !recallParticles.isPlaying)
                    recallParticles.Play();
            }
            else
            {
                if (recallParticles && recallParticles.isPlaying)
                    recallParticles.Stop();
            }
        }      
    }

    private void ParticleTrailWhenThrown()
    {
        if (starActions.IsTraveling) // Checks if the star has been thrown, and then starts trail system and stops recall system for a neater look
        {
            trailParticles.Play();
            recallParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        else
        {
            trailParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void GravityPullBeam(float distanceToPlayer)
    {
        isBeingGravityPulled = playerStarActionController.IsBeingGravityPulled;

        if (isBeingGravityPulled)
        {
            if (!gravityPullLine.enabled)
            {
                gravityPullLine.enabled = true;
            }

            // Set the positions for the line renderer that displays the "gravity beam"
            gravityPullLine.SetPosition(0, playerTransform.position); // Start position (Player)
            gravityPullLine.SetPosition(1, transform.position); // End position (Star)

            // Adjust width based on distance to star
            float lineWidth = Mathf.Lerp(0.3f, 0.2f, distanceToPlayer / 20f); // Controls shrinking speed of beam
            gravityPullLine.startWidth = 0.5f; // Start width
            gravityPullLine.endWidth = lineWidth; // End width
        }
        else
        {
            if (gravityPullLine.enabled)
            {
                gravityPullLine.enabled = false; // Disable the line renderer when not being pulled
            }
        }
    }

    void UpdateStarShimmerSFX(float distanceToPlayer)
    {
        PLAYBACK_STATE state;
        starShimmerSFX.getPlaybackState(out state);

        if (distanceToPlayer <= recallRange)
        {
            if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
            {
                starShimmerSFX.start();
            }
        }
        else
        {
            if (state == PLAYBACK_STATE.PLAYING)
            {
                starShimmerSFX.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }
    }
}
