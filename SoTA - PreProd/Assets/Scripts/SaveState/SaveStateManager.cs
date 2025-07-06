using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author:Linus
/// 
/// Modified by:
/// 
/// </summary>
//A Singelton since it is a Manager
public class SaveStateManager : MonoBehaviour
{
    public static SaveStateManager Instance { get; private set; }

    [SerializeField] private bool debugMode = false;

    private List<SaveData> saves = new List<SaveData>();

    private GameObject player;
    private CameraPanObserver cameraScript;
    private GameObject[] buttons;
    private GameObject[] boulders;
    
    //Temporary fix I hope or more data added here and removed from other places
    private StarActions starActions;
    private PlayerStarActionController playerStarActionController;
    private PlayerHealth playerHealth;

    private bool referencesSet = false;
    //private bool saved = false;
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        SetSaveableObjectReferences();
        starActions = GameObject.FindGameObjectWithTag("Star").GetComponent<StarActions>();
        playerStarActionController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStarActionController>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    //For Debug Purposes  <<-- If it works, then we should remove?
    private void OnSave()
    {
        if (debugMode)
        {
            Save();
            Debug.Log("Saved");
        }
        
    }
    private void OnLoad()
    {
        if (debugMode)
        { 
            Load();
            Debug.Log("Loaded");
        }
       
    }

    private void OnReset()
    {
        if (debugMode)
        {
            LoadStartSave();
            Debug.Log("Reset");
        }

    }
    private void SetSaveableObjectReferences()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        buttons = GameObject.FindGameObjectsWithTag("Button");
        boulders = GameObject.FindGameObjectsWithTag("Boulder");
        cameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraPanObserver>();
        referencesSet = true;
    }
    
    public void Save()
    {

        if (referencesSet == false)
            SetSaveableObjectReferences();
        if (player.GetComponent<PlayerController>().IsGrounded()|| player.GetComponent<CharacterController>().isGrounded || saves.Count < 1)
        {
            if (playerHealth.IsDead)
            {
                return;
            }

            saves.Add(CreateSaveData());
        }
    }
    private SaveData CreateSaveData()
    {
        Vector3 playerPositions = player.transform.position;
        Vector3[] boulderPositions = GetBoulderPositions();
        bool[] buttonsActive = GetButtonsState();
        Vector3 cameraPosition = GetCameraPosition();

        SaveData saveData = new SaveData(playerPositions, boulderPositions, buttonsActive, cameraPosition);
        return saveData;
    }
    private bool[] GetButtonsState()
    {
        bool[] buttonsActive = new bool[buttons.Length];
        int index = 0;
        foreach (GameObject button in buttons)
        {
            ButtonScript buttonScript = button.GetComponent<ButtonScript>();
            buttonsActive[index++] = buttonScript.IsActive;
        }

        return buttonsActive;
    }
    private Vector3[] GetBoulderPositions()
    {
        Vector3[] bouldersPosition = new Vector3[boulders.Length];
        int index = 0;
        foreach (GameObject boulder in boulders)
        {
            bouldersPosition[index++] = boulder.transform.position;
        }
        return bouldersPosition;
    }
    private Vector3 GetCameraPosition()
    {
        return cameraScript.GetCameraSavePosition();
    }
    
    public void Load()
    {
        SaveData dataToLoad;
        dataToLoad = saves[saves.Count - 1];
        CheckFromSaveData(dataToLoad);
        starActions.Recall();
        starActions.StopAllCoroutines();
        playerStarActionController.InteruptGravityPullToDestination();
    }
    private void CheckFromSaveData(SaveData saveData)
    {
        SetFromButtonStates(saveData);
        if (CheckSafety(saveData))
        {
            SetFromSaveData(saveData);
        }
        else
        {
            if (saves.Count > 1)
            {
                saves.RemoveAt(saves.Count - 1);
                CheckFromSaveData(saves[saves.Count - 1]);
            }
            else
            {
                SetFromSaveData(saveData);
            }
        }
    }
    private void SetFromSaveData(SaveData saveData)
    {
        SetFromButtonStates(saveData);
        SetFromBoulderPositions(saveData);
        SetFromPlayerPosition(saveData);
        SetFromCameraPosition(saveData);
    }
    private void SetFromPlayerPosition(SaveData saveData)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        player.GetComponent<PlayerSegment>().ClearSegments();
        playerController.SetPlayerPosition(saveData.PlayerPosition);
        playerController.inputLocked = false;

        PlayerStarActionController playerStarActionController = player.GetComponent<PlayerStarActionController>();
        playerStarActionController.AllowStarOnPlayer();
    }
    private void SetFromBoulderPositions(SaveData saveData)
    {
        Vector3[] boulderPositions = saveData.BoulderPositions;
        int index = 0;
        foreach(GameObject boulder in boulders)
        {
            boulder.GetComponent<BoulderController>().Detach();
            boulder.GetComponent<BoulderPushController>().StopBoulderPush();
            boulder.transform.position = boulderPositions[index++];
        }
    }
    private void SetFromButtonStates(SaveData saveData)
    {
        bool[] buttonsActive = saveData.ButtonsActive;
        int index = 0;  
        foreach(GameObject button in buttons)
        {
            ButtonScript buttonScript = button.GetComponent<ButtonScript>();
            buttonScript.SetState(buttonsActive[index++]);
        }
    }
    private void SetFromCameraPosition(SaveData saveData)
    {
        cameraScript.SetCameraPosition(saveData.CameraPosition);
    }
    private bool CheckSafety(SaveData saveData)
    {
        RaycastHit[] hits;
        CharacterController playerController = player.GetComponent<CharacterController>();
        Vector3 p1 = saveData.PlayerPosition+ playerController.center;
        hits = Physics.SphereCastAll(p1, playerController.height / 2, Vector3.forward, 0f);
        foreach(RaycastHit hit in hits)
        {
            if (hit.collider.tag == "Spikes")
            {
                return false;
            }
        }
        return true;
        
    }
    private void LoadStartSave()
    {
        SaveData dataToLoad = saves[0];
        SetFromSaveData(dataToLoad);
        CameraTriggerScript.ReactivateLastTrigger();
        starActions.Recall();
        starActions.StopAllCoroutines();
    }
}
