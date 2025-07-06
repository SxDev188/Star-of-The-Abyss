using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author:Gabbriel
/// 
/// Modified by:
/// 
/// </summary>
public class SceneRestartManager : MonoBehaviour
{    
    public SceneRestarter sceneRestarter;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
            if (sceneRestarter != null)
            {
                sceneRestarter.RestartCurrentSceneInEditor();
            }
            else
            {
                Debug.LogError("RestartSceneInEditor script not assigned!");
            }
        }
    }
}
