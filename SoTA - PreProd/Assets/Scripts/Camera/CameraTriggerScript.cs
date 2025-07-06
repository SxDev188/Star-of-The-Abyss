using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author:Karin, Linus
/// 
/// Modified by:
/// 
/// </summary>
public class CameraTriggerScript : MonoBehaviour
{
    public Vector2 panDirection;
    public List<Vector2> requiredEntryDirections;
    private static CameraTriggerScript lastActivatedTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && IsMovingMostlyInRequiredDirection(player.GetLastMoveDirection()))
            {
                if (lastActivatedTrigger != null && lastActivatedTrigger != this)
                {
                    lastActivatedTrigger.ReactivateTrigger();
                }

                CameraPanScript.Instance.PanCamera(panDirection);
                lastActivatedTrigger = this;

                DeactivateTrigger();
            }
        }
    }

    private bool IsMovingMostlyInRequiredDirection(Vector2 moveDirection)
    {
        foreach (var requiredDirection in requiredEntryDirections)
        {
            float dotProduct = Vector2.Dot(moveDirection.normalized, requiredDirection.normalized);
            if (dotProduct > 0.7f)  
            {
                return true; 
            }
        }
        return false; 
    }

    private void DeactivateTrigger()
    {
        GetComponent<Collider>().enabled = false;
    }

    public void ReactivateTrigger()
    {
        GetComponent<Collider>().enabled = true;
    }

    public static void ReactivateLastTrigger()
    {
        if (lastActivatedTrigger != null)
        {
            lastActivatedTrigger.ReactivateTrigger();
            lastActivatedTrigger = null;
        }
    }
}