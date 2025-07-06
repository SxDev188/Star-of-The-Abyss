using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// Author:Gabbriel
/// 
/// Modified by:
/// 
/// </summary>
public class CooldownTimer
{
    public bool IsRunning { get; private set; }
    private float cooldownDuration;
    Action onCooldownFinished; //An action (method) that is run when the cooldown is finished
    MonoBehaviour coroutineRunner; //The object running the coroutine

    public CooldownTimer(MonoBehaviour coroutineRunner)
    {
        this.coroutineRunner = coroutineRunner;
    }

    public void Start(float duration, Action onCooldownFinished)
    {
        if (!IsRunning && coroutineRunner != null)
        {
            this.cooldownDuration = duration;
            this.onCooldownFinished = onCooldownFinished;

            IsRunning = true;
            coroutineRunner.StartCoroutine(CooldownCoroutine());
        }
    }

    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(cooldownDuration);
        IsRunning = false;
        onCooldownFinished?.Invoke();
    }

    
}