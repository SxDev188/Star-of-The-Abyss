using UnityEngine;
/// <summary>
/// Author:Emil
/// 
/// Modified by:
/// 
/// </summary>
public class SpawnPointScript : MonoBehaviour
{
    PlayerController playerController;
    Vector3 spawmPointPosition;
    void Start()
    {
        spawmPointPosition = transform.position + new Vector3(0, 0f, 0);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        Spawn();
        SaveStateManager.Instance.Save();
    }

    public void Spawn()
    {
        playerController.SetPlayerPosition(spawmPointPosition);
    }
}
