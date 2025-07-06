using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Author:Karin
/// 
/// Modified by:
/// 
/// </summary>
public class RadialColorRenderFeature : ScriptableRendererFeature
{
    class RadialColorRenderPass : ScriptableRenderPass
    {
        private Material material;
        private RenderTargetHandle tempTexture;
        private Vector4 starPosition = new Vector4(0, 0, 0, 0); // Default position, this is why the color starts in bottom left corner
        private const int MAX_LIGHT_SOURCE_NUM = 10; // Needs to be known at compile time
        private List<Vector4> lightPositions = new List<Vector4>(MAX_LIGHT_SOURCE_NUM);
        private float effectRadius;
        private float effectRadiusSmoothing;
        private float effectToggle;
        private int activeLightCount = 0;
        private float healthBlackout = 0.0f;

        public RadialColorRenderPass(Material material)
        {
            this.material = material;
            tempTexture.Init("_TemporaryColorTexture");
        }

        public void SetStarPosition(Vector3 screenPos)
        {
            starPosition = new Vector4(screenPos.x, screenPos.y, 0, 0);
        }

        public void SetHealthBlackout(float value)
        {
            healthBlackout = value;
        }

        public void SetLightPositions(List<Vector4> lights)
        {
            lightPositions.Clear();

            // Ensure we maintain a fixed-size array by padding with zeroed vectors because otherwise the shader messes up positions when lights are turned on or off
            for (int i = 0; i < MAX_LIGHT_SOURCE_NUM; i++)
            {
                if (i < lights.Count)
                    lightPositions.Add(lights[i]);
                else
                    lightPositions.Add(Vector4.zero); // Fill remaining slots with (0,0,0,0)
            }
        }

        public void SetActiveLightCount(int count)
        {
            activeLightCount = count;
        }

        public void SetLightEffectRadius(float effectArea)
        {
            this.effectRadius = effectArea;
        }

        public void SetLightEffectRadiusSmoothing(float effectAreaSmoothing)
        {
            this.effectRadiusSmoothing = effectAreaSmoothing;
        }

        public void SetEffectToggle(float effectToggle)
        {
            this.effectToggle = effectToggle;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(tempTexture.id, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("Radial Color Effect");

            RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            // Pass the health blackout value to the shader
            material.SetFloat("_HealthBlackout", healthBlackout);

            // Pass star and additional light source positions to shader
            material.SetVector("_StarPosition", starPosition);
            material.SetVector("_ScreenResolution", new Vector2(Screen.width, Screen.height));
            material.SetVectorArray("_LightPositions", lightPositions);
            material.SetInt("_ActiveLightCount", activeLightCount);

            // Pass effect radius to shader
            material.SetFloat("_EffectRadius", effectRadius);
            material.SetFloat("_EffectRadiusSmoothing", effectRadiusSmoothing);
            material.SetFloat("_EnableEffect", effectToggle);

            // Apply effect
            cmd.Blit(source, tempTexture.Identifier(), material);
            cmd.Blit(tempTexture.Identifier(), source, material, 0);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    private RadialColorRenderPass pass;
    private Material material;
    private static readonly string ShaderName = "Unlit/RadialColorMaskURP";

    public override void Create()
    {
        Shader shader = Shader.Find(ShaderName);
        if (shader == null)
        {
            Debug.LogError("RadialColorRenderFeature: Shader is missing!");
            return;
        }

        material = CoreUtils.CreateEngineMaterial(shader);
        pass = new RadialColorRenderPass(material)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (pass != null)
        {
            renderer.EnqueuePass(pass);
        }
    }

    public void SetHealthBlackout(float value)
    {
        if (pass != null)
        {
            pass.SetHealthBlackout(value); // Correctly calls SetHealthBlackout of RadialColorRenderPass
        }
    }

    public void SetStarPosition(Vector3 starScreenPos)
    {
        if (pass != null)
        {
            pass.SetStarPosition(starScreenPos);
        }
    }

    public void SetLightPositions(List<Vector4> lights)
    {
        if(pass != null)
        {
            pass.SetLightPositions(lights);
            pass.SetActiveLightCount(lights.Count);
        }
    }

    public void SetLightEffectRadius(float effectRadius)
    {
        if (pass != null)
        {
            pass.SetLightEffectRadius(effectRadius);
        }
    }

    public void SetLightEffectRadiusSmoothing(float effectRadiusSmoothing)
    {
        if (pass != null)
        {
            pass.SetLightEffectRadiusSmoothing(effectRadiusSmoothing);
        }
    }

    public void SetEffectToggle(float effectToggle)
    {
        if (pass != null)
        {
            pass.SetEffectToggle(effectToggle);
        }
    }
}