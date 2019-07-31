using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UnderwaterDistortion : MonoBehaviour {

    [SerializeField] private Material mat = null;
    [SerializeField] [Range(0.001f, 0.1f)] private float pixelOffset = 0.001f;
    [SerializeField] [Range(0.1f, 20f)] private float noiseScale = 1f;
    [SerializeField] [Range(0.1f, 20f)] private float noiseFrequency = 1f;
    [SerializeField] [Range(0.1f, 30f)] private float noiseSpeed = 1f;
    [SerializeField] private float depthStart = 1f;
    [SerializeField] private float depthDistance = 10f;

    private bool bRender = false;

    // Start is called before the first frame update
    void Start() {
        if (!mat) { Debug.LogError("No material set."); }
    }

    // Update is called once per frame
    void Update() {
        mat.SetFloat("_NoiseFrequency", noiseFrequency);
        mat.SetFloat("_NoiseSpeed", noiseSpeed);
        mat.SetFloat("_NoiseScale", noiseScale);
        mat.SetFloat("_PixelOffset", pixelOffset);
        mat.SetFloat("_DepthStart", depthStart);
        mat.SetFloat("_DepthDistance", depthDistance);

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (bRender) {
            Graphics.Blit(source, destination, mat);
        }
        else {
            Graphics.Blit(source, destination);
        }
    }

    public void ShouldRender(bool b) {
        bRender = b;
    }
}
