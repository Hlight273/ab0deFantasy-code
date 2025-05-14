using System.Linq;
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
                if (material == null || !renderingData.cameraData.postProcessEnabled)
                    return;

                var camera = renderingData.cameraData.camera;
                var currentScene = camera.gameObject.scene;

                // 获取当前场景的Volume组件
                var volume = camera.GetComponent<Volume>();
                if (volume == null || !volume.isGlobal)
                {
                    volume = GameObject.FindObjectsOfType<Volume>()
                        .FirstOrDefault(v => v.gameObject.scene == currentScene && v.gameObject.activeInHierarchy);
                }

                if (volume == null || !volume.isActiveAndEnabled)
                    return;

                var profile = volume.profile;
                if (!profile.TryGet(out ClawVignette clawVignette) || !clawVignette.IsActive())
                    return;

                var cmd = CommandBufferPool.Get("Claw Vignette Effect");

                material.SetColor("_ClawColor", clawVignette.clawColor.value);
                material.SetFloat("_Intensity", clawVignette.intensity.value);
                material.SetFloat("_Size", clawVignette.size.value);
                material.SetVector("_FlowSpeed", clawVignette.flowSpeed.value);
                material.SetFloat("_FlowStrength", clawVignette.flowStrength.value);
                material.SetFloat("_CustomTime", Time.time);

                material.SetTexture("_ClawTex", clawVignette.clawTexture.value);

                var source = renderingData.cameraData.renderer.cameraColorTarget;
                Blit(cmd, source, tempTexture, material, 0);
                Blit(cmd, tempTexture, source);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void OnCameraCleanup(CommandBuffer cmd) { }

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
                Debug.LogError("无法找到 ClawVignette Shader!");
                return;
            }

            material = CoreUtils.CreateEngineMaterial(shader);
            pass = new ClawVignettePass(material);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (material == null || !renderingData.cameraData.postProcessEnabled)
                return;

            var camera = renderingData.cameraData.camera;
            var currentScene = camera.gameObject.scene;

            // 检查当前场景是否有激活的包含ClawVignette的Volume
            var volume = camera.GetComponent<Volume>();
            if (volume == null || !volume.isGlobal)
            {
                volume = GameObject.FindObjectsOfType<Volume>()
                    .FirstOrDefault(v => v.gameObject.scene == currentScene && v.gameObject.activeInHierarchy);
            }

            if (volume != null && volume.isActiveAndEnabled && volume.profile != null)
            {
                if (volume.profile.TryGet(out ClawVignette clawEffect) && clawEffect.IsActive())
                {
                    renderer.EnqueuePass(pass);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(material);
            pass?.Dispose();
        }
    }
}