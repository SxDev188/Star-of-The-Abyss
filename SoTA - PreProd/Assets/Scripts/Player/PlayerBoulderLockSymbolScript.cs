using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// Author: Gabbriel
/// 
/// Modified by:
/// 
/// </summary>
public class PlayerBoulderLockSymbolScript : MonoBehaviour
{
    PlayerController playerController;
    [SerializeField] GameObject lockSymbol;
    [SerializeField] Vector3 offsetToBoulder = new Vector3(0.75f, 0.75f, 0);
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        if (playerController.IsAttachedToBoulder && BoulderController.GetCurrentlyActiveBoulder() != null)
        {
            lockSymbol.SetActive(true);
            lockSymbol.transform.position = BoulderController.GetCurrentlyActiveBoulder().transform.position + offsetToBoulder;
            lockSymbol.transform.rotation = BoulderController.GetCurrentlyActiveBoulder().transform.rotation;
        }
        else if(lockSymbol.activeInHierarchy)
        {
            lockSymbol.SetActive(false);
        }
    }
}
