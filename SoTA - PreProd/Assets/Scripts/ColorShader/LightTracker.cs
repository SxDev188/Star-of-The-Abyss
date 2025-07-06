using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Linq;
using System.Collections.Generic;
/// <summary>
/// Author:Karin
/// 
/// Modified by:
/// 
/// </summary>
public class LightTracker : MonoBehaviour
{
    // COMPONENTS
    private Transform star;
    private RadialColorRenderFeature feature;
    private List<Transform> lightSources = new List<Transform>();

    // TWEAKABLE VARIABLES
    [SerializeField] private ScriptableRendererData rendererData;

    [SerializeField] private float smoothSpeed = 20f;

    [SerializeField] private float effectToggle = 1f;
    [SerializeField] private float effectRadius = 150f;
    [SerializeField] private float effectRadiusSmoothing = 10f;

    // STORING/VALUE VARIABLES
    private Vector4 smoothedStarPosition;

    void Start()
    {
        if (star == null)
        {
            GameObject starObject = GameObject.FindWithTag("Star");
            if (starObject != null)
                star = starObject.transform;
        }

        if (rendererData == null || star == null)
        {
            Debug.LogError("LightTracker: Missing star or renderer data!");
            return;
        }

        feature = rendererData.rendererFeatures
            .OfType<RadialColorRenderFeature>()
            .FirstOrDefault();

        if (feature != null)
        {
            Debug.Log("Star assigned to RadialColorRenderFeature.");
        }
        else
        {
            Debug.LogError("RadialColorRenderFeature not found in Renderer Data!");
        }
    }

    public void RegisterLightSource(Transform lightSource)
    {
        lightSources.Add(lightSource);
        Update();
    }

    public void RefreshLightSources()
    {
        List<Vector4> lightSourcePositions = new List<Vector4>();

        foreach (Transform t in lightSources)
        {
            LampScript lamp = t.GetComponent<LampScript>();
            if (lamp != null && lamp.IsLit)
            {
                Vector4 pos = Camera.main.WorldToViewportPoint(t.position + 1.5f * Vector3.down);
                lightSourcePositions.Add(pos);
            }
        }

        // Ensure exactly MAX_LIGHT_SOURCE_NUM elements (10) so the shader doesn't read junk data
        while (lightSourcePositions.Count < 10)
        {
            lightSourcePositions.Add(new Vector4(-1, -1, 0, 0)); // Placeholder for inactive lights
        }

        feature.SetLightPositions(lightSourcePositions);
    }

    private float WorldDistanceToViewportDistance(float worldDistance)
    {
        Camera cam = Camera.main;
        Vector3 worldOrigin = star.position;
        Vector3 worldOffset = worldOrigin + new Vector3(worldDistance, 0, 0); // 1 unit in X direction

        Vector3 viewportOrigin = cam.WorldToViewportPoint(worldOrigin);
        Vector3 viewportOffset = cam.WorldToViewportPoint(worldOffset);

        return Mathf.Abs(viewportOffset.x - viewportOrigin.x);
    }

    void Update()
    {
        if (star == null || feature == null) return;

        // Checks Star position and sends its position further
        Vector3 starViewportPos = Camera.main.WorldToViewportPoint(star.position + 0.25f * Vector3.down);
        // 0.25f * Vector3.down is position adjustment for the radius center
        Vector4 targetStarPosition = new Vector4(starViewportPos.x, starViewportPos.y, 0, 0);
        // smoothedStarPosition is currently needed to prevent "shaking" when Star is held by player
        smoothedStarPosition = Vector4.Lerp(smoothedStarPosition, targetStarPosition, Time.deltaTime * smoothSpeed);
        feature.SetStarPosition(smoothedStarPosition);

        float viewportRadius = WorldDistanceToViewportDistance(effectRadius);
        float viewportSmoothing = WorldDistanceToViewportDistance(effectRadiusSmoothing);

        feature.SetLightEffectRadius(viewportRadius);
        feature.SetLightEffectRadiusSmoothing(viewportSmoothing);
        feature.SetEffectToggle(effectToggle);

        List<Vector4> lightSourcePositions = new List<Vector4>();
        foreach (Transform t in lightSources)
        {
            LampScript lamp = t.GetComponent<LampScript>();

            if (lamp != null && lamp.IsLit)
            {
                Vector4 pos = Camera.main.WorldToViewportPoint(t.position);
                lightSourcePositions.Add(pos);
            }
        }

        feature.SetLightPositions(lightSourcePositions);
    }
}