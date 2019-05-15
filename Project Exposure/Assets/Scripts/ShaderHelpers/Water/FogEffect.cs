using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class FogEffect : MonoBehaviour
{
    public Material mat;
    public Color _fogColor;
    public float _depthStart;
    public float _depthDistance;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetColor("_FogColor", _fogColor);
        mat.SetFloat("_DepthStart", _depthStart);
        mat.SetFloat("_DepthDistance", _depthDistance);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}
