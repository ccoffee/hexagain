using UnityEngine;
using UnityEngine.Rendering;

public class DepthTextureSetup : MonoBehaviour
{
    public Material postProcessingMaterial;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;
    }
}