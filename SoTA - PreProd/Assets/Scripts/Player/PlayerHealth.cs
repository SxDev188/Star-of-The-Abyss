using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author: Linus, Emil
/// 
/// Modified by:Gabbriel, Karin
/// 
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float timeWithoutStar = 10.0f;
    [SerializeField] private float timeToResetLife = 0.5f;
    [SerializeField] private float deathCooldownDuration = 1f;
    private float currentHealth;
    private float startingHealth = 1.0f;
    private bool isHealthDrainPaused = false; //Used to stop player from dying when reading lore tiles
    private StarActions starActions;
    private PlayerController playerController;
    private EventInstance deathSFX;
    private EventInstance lowHealthWarningSFX;
    private ParticleSystem respawnParticles;
    public bool IsDead { get; private set; } = false;
    public float CurrentHealth { get { return currentHealth; } }

    CooldownTimer deathCooldownTimer;
    

    // Start is called before the first frame update
    private void Start()
    {
        GameObject star = GameObject.FindGameObjectWithTag("Star");
        starActions = star.GetComponent<StarActions>();
        playerController = GetComponent<PlayerController>();
        currentHealth = startingHealth;
        deathSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.DeathSFX);
        lowHealthWarningSFX = AudioManager.Instance.CreateInstance(FMODEvents.Instance.LowHealthWarningSFX);
        respawnParticles = GetComponentInChildren<ParticleSystem>();

        deathCooldownTimer = new CooldownTimer(this); //sends in the monobehaviour object that will run the timers coroutine
    }

    // Update is called once per frame
    private void Update()
    {
        ManagePlayerHealth();

        PlayLowHealthWarningSound();

        //Sets global PlayerHealth parameter in FMOD --> controls effects on the master audio channel
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("PlayerHealth", currentHealth);
    }
    private void ManagePlayerHealth()
    {
        if (starActions.IsOnPlayer)
        {
            if(currentHealth < startingHealth)
            {
                RestoreHealth();
            }
        }
        else
        {
            DrainHealth();
        }
    }

    private void DrainHealth()
    {
        if(isHealthDrainPaused) //Used to stop player from dying when reading lore tiles
        {
            return;
        }

        currentHealth -= (Time.deltaTime / timeWithoutStar);
        if (currentHealth < 0.0f)
        {
            Death();
        }
    }

    private void RestoreHealth()
    {
        currentHealth += (Time.deltaTime / timeToResetLife);
        if (currentHealth > startingHealth)
            currentHealth = startingHealth;
    }

    public void Death()
    {
        if (!IsDead) //to avoid triggering the sound effect and animation multiple times if player is gravity pulling through more than 1 set of spikes
        {
            IsDead = true;
            deathSFX.start();
            starActions.IsOnPlayer = false;
            playerController.SetDeathAnimationTrue();

            deathCooldownTimer.Start(deathCooldownDuration, Respawn); //automatically runs the Respawn() method when timer is finished
        }
    }

    public void Respawn()
    {
        SaveStateManager.Instance.Load();
        currentHealth = startingHealth;

        playerController.SetDeathAnimationFalse();
        StartCoroutine(PlayAndStopParticleBurst());

        IsDead = false;
    }

    void PlayLowHealthWarningSound()
    {
        if (currentHealth < 0.47f && currentHealth > 0 && !starActions.IsOnPlayer)
        {
            PLAYBACK_STATE playbackState;
            lowHealthWarningSFX.getPlaybackState(out playbackState);

            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                lowHealthWarningSFX.start();
            }
        }
        else
        {
            PLAYBACK_STATE playbackState;
            lowHealthWarningSFX.getPlaybackState(out playbackState);

            if (playbackState.Equals(PLAYBACK_STATE.PLAYING))
            {
                lowHealthWarningSFX.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }
    }

    IEnumerator PlayAndStopParticleBurst()
    {
        respawnParticles.Play();
        yield return new WaitForSeconds(0.1f); // wait for particles to spawn
        respawnParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    public void PauseHealthDrain() //Used to stop player from dying when reading lore tiles
    {
        isHealthDrainPaused = true;
    }
    
    public void ResumeHealthDrain() //Used to stop player from dying when reading lore tiles
    {
        isHealthDrainPaused = false;
    }

    public void SetIsDead(bool isDead)
    {
        IsDead = isDead;
    }
}
