using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineRenderFeature : ScriptableRendererFeature
{
    class OutlinePass : ScriptableRenderPass
    {
        // ����һ����������Ⱦ���
        Material outlineMaterial;

        public OutlinePass()
        {
            // �ڹ��캯���д�����߲���
            outlineMaterial = new Material(Shader.Find("Outline/UniversalOutline"));
            this.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Outline Pass");

            // ������ͼ��ͶӰ����
            cmd.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix, renderingData.cameraData.camera.projectionMatrix);

            // ����������Ⱦ����Ӧ����߲���
            var cullResults = renderingData.cullResults;
            var drawingSettings = new DrawingSettings(new ShaderTagId("Outline"), new SortingSettings(renderingData.cameraData.camera) { criteria = SortingCriteria.CommonOpaque });
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            // ������һ�У�ֱ�Ӵ��� cullResults
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