using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;

[System.Serializable]
[PostProcess(typeof(OverlayEffectRenderer), PostProcessEvent.BeforeStack, "Overlay Effect", false)]
public sealed class OverlayEffect : PostProcessEffectSettings
{
    // Effect settings go here
    public ColorParameter _outlineColor = new ColorParameter() { value = new Color(0, 1, 0, 1) };
    public ColorParameter _fillColor = new ColorParameter() { value = new Color(0, 1, 0, 1) };
}

public sealed class OverlayEffectRenderer : PostProcessEffectRenderer<OverlayEffect>
{
    private Shader _shader;
    private Material _material;
    public override void Init()
    {
        base.Init();
        _shader = Shader.Find("Hidden/OverlayShader");
        _material = new Material(_shader);
    }

    public override void Render(PostProcessRenderContext context)
    {
        var cmd = context.command;
        var sheet = context.propertySheets.Get(_shader);

        context.camera.depthTextureMode = DepthTextureMode.Depth;


        // Copy the existing image. We will be drawing on top of it.
        cmd.BlitFullscreenTriangle(context.source, context.destination);
        // Overlay effect code goes here

        /*
        _material.DisableKeyword("Pattern_Diamond");
        _material.EnableKeyword("Pattern_Rect");

        // Request an 8 bit single channel texture without depth buffer.
        RenderTexture overlayIDTexture = RenderTexture.GetTemporary(context.camera.pixelWidth, context.camera.pixelHeight, 0, RenderTextureFormat.R8);
        // Bind the temporary render texture, but keep using the camera's depth buffer
        cmd.SetRenderTarget(overlayIDTexture, BuiltinRenderTextureType.Depth);
        // Always clear temporary RenderTextures before use, their content is random.
        cmd.ClearRenderTarget(false, true, Color.clear, 1.0f);

        // [Pass 2]
        // Bind the camera render target

        cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.Depth);
        cmd.SetGlobalTexture("_OverlayIDTexture", overlayIDTexture);


        cmd.SetGlobalVector("_OutlineColor", settings._outlineColor);
        cmd.SetGlobalVector("_FillColor", settings._fillColor);



        // Don't forget to release the temporary render texture
        RenderTexture.ReleaseTemporary(overlayIDTexture);
        */
    }

}