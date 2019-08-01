using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UnderwaterFog : MonoBehaviour {

    [SerializeField] private Material mat = null;
    [SerializeField] private Color fogColor = Color.white;
    [SerializeField] private float depthStart = 1f;
    [SerializeField] private float depthDistance = 25f;

    private bool bRender = false;


    // Start is called before the first frame update
    void Start() {
        if (!mat) { Debug.LogError("No material set."); }

        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        GetComponent<Camera>().renderingPath = RenderingPath.DeferredShading;

    }

    // Update is called once per frame
    void Update() {
        mat.SetColor("_FogColor", fogColor);
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
