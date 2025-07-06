using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
/// <summary>
/// Author: Karin
/// 
/// Modified by:
/// 
/// </summary>
public class PlayerHealthEffect : MonoBehaviour
{
    // Assigning this through script was complicated due to Unity protection of the render features...
    [SerializeField] private RadialColorRenderFeature radialColorRenderFeature;
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (radialColorRenderFeature == null)
        {
            Debug.LogError("RadialColorRenderFeature is not assigned in the Inspector.");
        }
    }

    private void Update()
    {
        float health = playerHealth.CurrentHealth; // 1.0 to 0.0
        float blackAmount = 1.0f - health;         // 0.0 (healthy) to 1.0 (dead/black)

        // Update the radial color effect
        if (radialColorRenderFeature != null)
        {
            radialColorRenderFeature.SetHealthBlackout(blackAmount);
        }
    }
}