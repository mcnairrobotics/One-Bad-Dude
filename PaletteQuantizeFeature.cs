using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PaletteQuantizeFeature : ScriptableRendererFeature
{
    class PaletteQuantizePass : ScriptableRenderPass
    {
        private Material material;
        private RTHandle tempTexture;

        public PaletteQuantizePass(Material mat)
        {
            material = mat;
            renderPassEvent = RenderPassEvent.AfterRendering;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(
                ref tempTexture,
                desc,
                name: "_TempPaletteQuantize"
            );
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("Palette Quantize");

            RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            // Source → Temp
            Blitter.BlitCameraTexture(cmd, source, tempTexture, material, 0);
            // Temp → Source
            Blitter.BlitCameraTexture(cmd, tempTexture, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            tempTexture?.Release();
        }
    }

    public Shader shader;
    public int colorCount = 16;

    private Material material;
    private PaletteQuantizePass pass;

    public override void Create()
    {
        if (shader == null)
            return;

        material = CoreUtils.CreateEngineMaterial(shader);
        material.SetInt("_Colors", colorCount);

        pass = new PaletteQuantizePass(material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null)
            return;

        renderer.EnqueuePass(pass);
    }
}
