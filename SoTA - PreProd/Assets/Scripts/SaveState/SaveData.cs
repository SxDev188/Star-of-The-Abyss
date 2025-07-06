using UnityEngine;
/// <summary>
/// Author:Linus
/// 
/// Modified by:
/// 
/// </summary>
public class SaveData 
{
    public bool[] ButtonsActive { get { return buttonsActive; } }
    public Vector3 PlayerPosition { get { return playerPosition; } }
    public Vector3[] BoulderPositions { get { return boulderPositions; } }
    public Vector3 CameraPosition { get { return cameraPosition; } }

    private bool[] buttonsActive;

    private Vector3 playerPosition;
    private Vector3[] boulderPositions;

    private Vector3 cameraPosition;

    public SaveData(Vector3 playerPosition, Vector3[] boulderPositions, bool[] buttonsActive, Vector3 cameraPosition) 
    { 
        this.playerPosition = playerPosition;
        this.boulderPositions = boulderPositions;
        this.buttonsActive = buttonsActive;
        this.cameraPosition = cameraPosition;
    }

}
