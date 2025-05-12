using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HFantasy.Script.PostProcessing
{
    public class ClawVignetteRenderer : ScriptableRendererFeature
    {
        class ClawVignettePass : ScriptableRenderPass
        {
            private Material material;
            private ClawVignette clawVignette;
            private RTHandle tempTexture;

             public ClawVignettePass(Material material)
            {
                this.material = material;
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
                profilingSampler = new ProfilingSampler("Claw Vignette");
            }

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                var descriptor = renderingData.cameraData.cameraTargetDescriptor;
                descriptor.depthBufferBits = 0;

                RenderingUtils.ReAllocateIfNeeded(ref tempTexture, descriptor, name: "_TempClawVignette");
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (material == null)
                    return;

                var stack = VolumeManager.instance.stack;
                clawVignette = stack.GetComponent<ClawVignette>();
                
                if (clawVignette == null || !clawVignette.IsActive())
                {
                    return;
                }

                var cmd = CommandBufferPool.Get("Claw Vignette Effect");

                  material.SetColor("_ClawColor", clawVignette.clawColor.value);
                material.SetFloat("_Intensity", clawVignette.intensity.value);
                material.SetFloat("_Size", clawVignette.size.value);
                material.SetVector("_FlowSpeed", clawVignette.flowSpeed.value);
                material.SetFloat("_FlowStrength", clawVignette.flowStrength.value);
                material.SetFloat("_CustomTime", Time.time);

                material.SetTexture("_ClawTex", null);
                if (clawVignette.clawTexture.value != null)
                {
                    material.SetTexture("_ClawTex", clawVignette.clawTexture.value);
                }

                var source = renderingData.cameraData.renderer.cameraColorTarget;
                var dest = renderingData.cameraData.renderer.cameraColorTarget;

                Blit(cmd, source, dest, material, 0);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void OnCameraCleanup(CommandBuffer cmd)
            {
            }

            public void Dispose()
            {
                tempTexture?.Release();
            }
        }

        private ClawVignettePass pass;
        private Material material;

        public override void Create()
        {
            var shader = Shader.Find("PostProcessing/ClawVignette");
            if (shader == null)
            {
                Debug.LogError("Œﬁ∑®’“µΩ ClawVignette Shader!");
                return;
            }

            material = CoreUtils.CreateEngineMaterial(shader);
            pass = new ClawVignettePass(material);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (material != null)
            {
                renderer.EnqueuePass(pass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(material);
            pass?.Dispose();
        }
    }
}
