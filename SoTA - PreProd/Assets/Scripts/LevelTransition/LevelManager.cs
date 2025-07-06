using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Author:Linus
/// 
/// Modified by:
/// 
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    private Scene activeScene;
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        // Singleton moment

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        activeScene = SceneManager.GetActiveScene();
    }
    public void LoadNextSceen()
    {
        int nextBuildIndex = activeScene.buildIndex + 1;
        LoadSceenByIndex(nextBuildIndex);
    }
    public void LoadSceenByIndex(int index)
    {
        if (SceneManager.sceneCountInBuildSettings > index && index >= 0)
        {
            SceneManager.LoadScene(index);
        }
        else
        {
            Application.Quit();
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #endif
        }
    }
}
