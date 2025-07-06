using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// Author:Gabbriel
/// 
/// Modified by:
/// 
/// </summary>
public class SceneRestarter : MonoBehaviour
{
    public void RestartCurrentSceneInEditor()
    {
#if UNITY_EDITOR
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
#else
        Debug.LogWarning("RestartSceneInEditor called, but it will only work in the Unity Editor.");
#endif
    }
}