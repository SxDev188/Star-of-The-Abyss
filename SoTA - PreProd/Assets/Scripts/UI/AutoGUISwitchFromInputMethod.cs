using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// Author:Gabbriel
/// 
/// Modified by:
/// </summary>
public class AutoGUISwitchFromInputMethod : MonoBehaviour
{
    [SerializeField] GameObject keyboard;
    [SerializeField] GameObject controller;

    void Update()
    {
        if (UIScript.IsUsingController)
        {
            if(!controller.gameObject.activeInHierarchy) //we are using controller but the controller text is diabled
            {
                controller.gameObject.SetActive(true);
                keyboard.gameObject.SetActive(false);
            }
        } else
        {
            if (!keyboard.gameObject.activeInHierarchy) //we are using keyboard but the keyboard text is diabled
            {
                keyboard.gameObject.SetActive(true);
                controller.gameObject.SetActive(false);
            }
        }
    }
}
