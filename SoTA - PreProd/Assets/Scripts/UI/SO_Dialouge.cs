using UnityEngine;

/// <summary>
/// Author: Sixten
/// 
/// Modified by:
/// 
/// </summary>

[CreateAssetMenu(fileName = "New_dialouge", menuName = "Dialouge")]
public class SO_Dialogue : ScriptableObject
{
    //IIRC this whole source file is from the tutorial but with minor (if any) changes
    // Written by myself though

    [System.Serializable] public class Info
    {
        [TextArea(4, 8)] public string dialouge;
    }

    public Info[] dialogueInfo;

}

