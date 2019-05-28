using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class FogEffect : MonoBehaviour
{
    public Material mat;
    public Color _fogColor;
    public Color _silhouetteColor;
    public float _depthStart;
    public float _depthDistance;
    [Range(0, 1)]
    public float _depthFadeDistance = 0.5f;
    [Range(0, 1)]
    public float _fogBeforeFadeMultiplier = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetColor("_FogColor", _fogColor);
        mat.SetColor("_SilhouetteColor", _silhouetteColor);
        mat.SetFloat("_DepthStart", _depthStart);
        mat.SetFloat("_DepthDistance", _depthDistance);
        mat.SetFloat("_FadeDistance", _depthFadeDistance);
        mat.SetFloat("_FogBeforeFadeMultiplier", _fogBeforeFadeMultiplier);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}
