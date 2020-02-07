using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox : MonoBehaviour {
    public static bool debug = true;
    private bool offScreen = true;
    private Camera cam;
    private float xMax, xMin, yMax, yMin;
    private float boxHeight = 20f, boxWidth = 60f;
    private Mesh mesh;
    private static Texture2D whiteTexture;
    private GUIStyle style = new GUIStyle();
    private IEnumerator coroutine;


    private void Start() {
        cam = Camera.main;
        MeshFilter mf = GetComponent<MeshFilter>();
        if(mf == null) {
            mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }
        else {
            mesh = mf.mesh;
        }

        whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();

        style.alignment = TextAnchor.MiddleCenter;

        coroutine = CalculateBorders();
        StartCoroutine(coroutine);
    }

    private void OnGUI() {
        if (debug && !offScreen) {
            DrawBorders();
        }
    }

    private void DrawBorders() {
        DrawScreenRect(new Rect((int)xMin, (int)(Screen.height - yMin), 
        (int)(xMax - xMin), (int)(-yMax + yMin)), new Color(0, 0, 1, 0.2f));

        DrawCorners(new Rect((int)xMax, (int)(Screen.height - yMax - boxHeight),
            boxWidth, boxHeight), new Rect((int)xMin - boxWidth, 
            (int)(Screen.height - yMin), boxWidth, boxHeight));
    }

    private IEnumerator CalculateBorders() {
        while (true) {
            yield return new WaitForSeconds(0.01f);

            Matrix4x4 localToWorld = transform.localToWorldMatrix;
            xMax = 0;
            xMin = int.MaxValue;
            yMax = 0;
            yMin = int.MaxValue;

            for (int i = 0; i < mesh.vertexCount; i++) {
                Vector2 v = cam.WorldToScreenPoint(localToWorld.MultiplyPoint3x4(mesh.vertices[i]));
                if (v.x < xMin) {
                    xMin = v.x;
                }
                else if (v.x > xMax) {
                    xMax = v.x;
                }
                if (v.y < yMin) {
                    yMin = v.y;
                }
                else if (v.y > yMax) {
                    yMax = v.y;
                }
            }

            if(xMax < 1f || yMax < 1f || xMin + 1 > Screen.width || 
                yMin + 1 > Screen.height) { offScreen = true; }
            else { offScreen = false; }

            xMax = Mathf.Clamp(xMax, 0, Screen.width);
            xMin = Mathf.Clamp(xMin, 0, Screen.width);
            yMax = Mathf.Clamp(yMax, 0, Screen.height);
            yMin = Mathf.Clamp(yMin, 0, Screen.height);
        }
    }

    public void DrawScreenRect(Rect rect, Color color) {
        GUI.color = color;
        GUI.DrawTexture(rect, whiteTexture);
        GUI.color = Color.white;
    }

    public void DrawCorners(Rect topRight, Rect bottomLeft) {
        GUI.DrawTexture(topRight, whiteTexture);
        GUI.DrawTexture(bottomLeft, whiteTexture);
        GUI.Box(topRight, "" + (int)xMax + ", " + (int)yMax, style);
        GUI.Box(bottomLeft, "" + (int)xMin + ", " + (int)yMin, style);
    }
}
