using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineRenderFeature : ScriptableRendererFeature
{
    class OutlinePass : ScriptableRenderPass
    {
        // 定义一个材质来渲染描边
        Material outlineMaterial;

        public OutlinePass()
        {
            // 在构造函数中创建描边材质
            outlineMaterial = new Material(Shader.Find("Outline/UniversalOutline"));
            this.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Outline Pass");

            // 设置视图和投影矩阵
            cmd.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix, renderingData.cameraData.camera.projectionMatrix);

            // 遍历所有渲染对象，应用描边材质
            var cullResults = renderingData.cullResults;
            var drawingSettings = new DrawingSettings(new ShaderTagId("Outline"), new SortingSettings(renderingData.cameraData.camera) { criteria = SortingCriteria.CommonOpaque });
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            // 修正这一行，直接传入 cullResults
            context.DrawRenderers(cullResults, ref drawingSettings, ref filteringSettings);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    OutlinePass outlinePass;

    public override void Create()
    {
        outlinePass = new OutlinePass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(outlinePass);
    }
}