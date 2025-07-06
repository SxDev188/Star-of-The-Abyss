using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Author: Emil, Linus
/// 
/// Modified by: Gabbriel
/// 
/// </summary>
public class EndPointScript : MonoBehaviour
{
    [SerializeField] Transform nextSpawnPoint;
    [SerializeField] Vector2 cameraPanDirection;
    [SerializeField] bool isExit;
    [SerializeField] float exitWaitTimeInSeconds = 3;
    [SerializeField] CanvasGroup fadeToBlackCanvas;
    
    PlayerController playerController;
    CooldownTimer cooldownTimer;

    IEnumerator fadeToblackCoroutine;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        if (playerController == null)
        {
            Debug.LogWarning("PlayerController not found in the scene!");
        }

        cooldownTimer = new CooldownTimer(this);
    }

    IEnumerator FadeToBlack()
    {
        float increment = 0.01f;

        fadeToBlackCanvas.alpha = 0;
        fadeToBlackCanvas.gameObject.SetActive(true);

        while (true)
        {
            if ((fadeToBlackCanvas.alpha + increment) <= 1)
            {
                fadeToBlackCanvas.alpha += increment;
            } else
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        StopCoroutine(fadeToblackCoroutine);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().SetIsDead(true); //placeholder just so player cannot move
            AudioManager.Instance.SetBgMusicState(false); //if music is playing, turn it off (let fade out)
            AudioManager.Instance.SetAllAmbienceState(false); //if music is playing, turn it off (let fade out)

            cooldownTimer.Start(exitWaitTimeInSeconds, ExitBehavior); //ExitBehavior() method will run when timer is finished
            fadeToblackCoroutine = FadeToBlack();
            StartCoroutine(fadeToblackCoroutine);
        }
    }

    private void ExitBehavior()
    {
        LevelManager.Instance.LoadNextSceen();
        if (isExit)
        {
            LevelManager.Instance.LoadNextSceen();
            //#if UNITY_EDITOR
            //EditorApplication.isPlaying = false;
            //#endif
        }

        else if (nextSpawnPoint != null)
        {
            playerController.transform.position = nextSpawnPoint.position;
            CameraPanScript.Instance.PanCamera(cameraPanDirection);
        }
        else
        {
            Debug.LogWarning("Next spawn point not assigned!");
        }
    }
}
