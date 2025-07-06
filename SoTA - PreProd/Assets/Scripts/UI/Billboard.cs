using UnityEngine;

/// <summary>
/// Author: Sixten
/// 
/// Modified by:
/// 
/// </summary>

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            // Rotate the object to always face the camera
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
                             cam.transform.rotation * Vector3.up);
        }
    }
}
