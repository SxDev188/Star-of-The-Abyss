using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Author: Sixten
/// 
/// Modified by:
/// 
/// </summary>

public class GraphicsMenu : MonoBehaviour
{
    //IIRC this whole source file is from the tutorial but with minor (if any) changes
    // Written by myself though

    [SerializeField] private TMP_Dropdown resolutionDropDown;

    void Start() 
    {
        PopulateResDropDown();
    }

    public void ToggleFullScreen() 
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void ChangeResolution(int resIndex)
    {
        Screen.SetResolution(Screen.resolutions[resIndex].width, Screen.resolutions[resIndex].height, Screen.fullScreen);
    }

    public void PopulateResDropDown()
    {
        int currentResIndex = 0;
        List<string> options = new List<string>();

        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            options.Add(Screen.resolutions[i].width + "x" + Screen.resolutions[i].height);
            if (options[i] == Screen.currentResolution.width + "x" + Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropDown.ClearOptions();
        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currentResIndex;
        resolutionDropDown.RefreshShownValue();

    }
}
