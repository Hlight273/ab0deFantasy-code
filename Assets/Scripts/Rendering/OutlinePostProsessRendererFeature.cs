using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace HFantasy
{
    public class OutlinePostProsessRendererFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class OutlineSettings
        {
            public float outlineWidth = 1f;
            public Color outlineColor = Color.black;
        }

        public OutlineSettings settings = new OutlineSettings();

        class OutlinePass : ScriptableRenderPass
        {
            private Material outlineMaterial;
            private RenderTargetHandle tempRT;
            private ScriptableRenderer renderer;
            private OutlineSettings settings;

            public OutlinePass(OutlineSettings settings)
            {
                this.settings = settings;
                outlineMaterial = new Material(Shader.Find("Hidden/PostProcess/SelectionOutline"));
                tempRT.Init("_TempRT");
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
                ConfigureInput(ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Depth);
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.renderer = renderer;
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                // 确保临时RT有正确的格式
                RenderTextureDescriptor desc = cameraTextureDescriptor;
                desc.depthBufferBits = 0;
                cmd.GetTemporaryRT(tempRT.id, desc);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!renderingData.cameraData.postProcessEnabled) return;

                var cmd = CommandBufferPool.Get("Outline Pass");

                var source = renderer.cameraColorTarget;

                // 设置描边参数
                outlineMaterial.SetFloat("_OutlineWidth", settings.outlineWidth);
                outlineMaterial.SetColor("_OutlineColor", settings.outlineColor);

                cmd.Blit(source, tempRT.Identifier());
                cmd.Blit(tempRT.Identifier(), source, outlineMaterial);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(tempRT.id);
            }
        }

        OutlinePass outlinePass;

        public override void Create()
        {
            outlinePass = new OutlinePass(settings);
        }



        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!renderingData.cameraData.postProcessEnabled) return;

            outlinePass.Setup(renderer);
            renderer.EnqueuePass(outlinePass);
        }
    }
}