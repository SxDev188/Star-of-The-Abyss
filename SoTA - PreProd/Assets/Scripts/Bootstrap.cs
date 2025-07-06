using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Author: Sixten
/// 
/// Modified by: Sixten
/// 
/// The bootstrapper just initalizes components (UI in our case lol) that we want to bring around in the whole game
/// Not used as extensively as I would've wanted, but it was implemented a bit late in development
/// 
/// </summary>

public class Bootstrap : MonoBehaviour
{

    public GameObject[] persistentPrefabs; 
    public string firstSceneName = "MainMenu";

    void Awake()
    {
        foreach (var prefab in persistentPrefabs)
        {
            GameObject obj = Instantiate(prefab);
            DontDestroyOnLoad(obj);
        }

        SceneManager.LoadScene(firstSceneName);
    }
}
