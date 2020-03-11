using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox : MonoBehaviour {
    private BoxManager boxManager;

    public bool offScreen = true;
    public float xMax, xMin, yMax, yMin;
    private Mesh mesh;
    private IEnumerator coroutine;
    private Rect bounding = new Rect();
    private Rect trCoords = new Rect();
    private Rect blCoords = new Rect();
    private Rect nameBox = new Rect();

    private void Awake() {
        boxManager = BoxManager.getInstance();
        boxManager.boundingBoxes.Add(this);
        GetMesh();
        coroutine = CalculateBorders();
        StartCoroutine(coroutine);
    }

    private void GetMesh() {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) {
            mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        }
        else {
            mesh = mf.mesh;
        }
    }

    private void OnGUI() {
        if (boxManager.debug && !offScreen) {
            DrawBorders();
        }
    }

    //Draws all the bounding box info
    private void DrawBorders() {
        //Draw bounding box
        bounding.width = (int)(xMax - xMin) + boxManager.borderSize * 2;
        bounding.height = (int)(-yMax + yMin) - boxManager.borderSize * 2;
        bounding.x = (int)xMin - boxManager.borderSize;
        bounding.y = (int)(Screen.height - yMin) + boxManager.borderSize;
        DrawScreenRect(bounding, boxManager.boxColor);

        //Draw coordinate info
        trCoords.x = (int)xMax + boxManager.borderSize;
        trCoords.y = (int)(Screen.height - yMax - boxManager.boxHeight - boxManager.borderSize);
        trCoords.width = boxManager.boxWidth;
        trCoords.height = boxManager.boxHeight;
        blCoords.x = (int)xMin - boxManager.boxWidth - boxManager.borderSize - 1;
        blCoords.y = (int)(Screen.height - yMin + boxManager.borderSize + 1);
        blCoords.width = boxManager.boxWidth;
        blCoords.height = boxManager.boxHeight;
        DrawCorners(trCoords, blCoords);

        //Drawing name info
        if (boxManager.displayNames) {
            nameBox.x = (int)xMin - boxManager.borderSize;
            nameBox.y = (int)(Screen.height - yMax - boxManager.boxHeight - boxManager.borderSize);
            nameBox.width = (int)(xMax - xMin) + boxManager.borderSize * 2;
            nameBox.height = boxManager.boxHeight;
            DrawName(nameBox, "" + this.gameObject.name);
        }
    }

    //Calculates the bounding box in screen units
    private IEnumerator CalculateBorders() {
        while (true) {
            yield return new WaitForSeconds(0.001f);
            Matrix4x4 localToWorld = transform.localToWorldMatrix;
            xMax = 0;
            xMin = int.MaxValue;
            yMax = 0;
            yMin = int.MaxValue;

            for (int i = 0; i < mesh.vertexCount; i++) {
                Vector2 v = boxManager.camera.WorldToScreenPoint(localToWorld.MultiplyPoint3x4(mesh.vertices[i]));
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

    //Draws the bounding box rectangle
    public void DrawScreenRect(Rect rect, Color color) {
        GUI.color = color;
        if (Event.current.type == EventType.Repaint) {
            boxManager.boxStyle.Draw(rect, false, false, false, false);
        }
    }

    //DrawCorners displays the coordinate info for the bounding box
    public void DrawCorners(Rect topRight, Rect bottomLeft) {
        if (Event.current.type == EventType.Repaint) {
            //Print top right coords info
            boxManager.textStyle.alignment = TextAnchor.LowerLeft;
            String tr = "⌞(" + (int)xMax + ", " + (int)yMax + ")";
            if (boxManager.outline) { 
                DrawOutline(topRight, tr, boxManager.outlineThickness, boxManager.textStyle); 
            }
            boxManager.textStyle.Draw(topRight, tr, false, false, false, false);

            //Print bottom left coords info
            String bl = "(" + (int)xMin + ", " + (int)yMin + ")⌝";
            boxManager.textStyle.alignment = TextAnchor.UpperRight;
            if (boxManager.outline) {
                DrawOutline(bottomLeft, bl, boxManager.outlineThickness, boxManager.textStyle);
            }
            boxManager.textStyle.Draw(bottomLeft, bl, false, false, false, false);
        }
    }

    //DrawName displays the name info for the bounding box
    public void DrawName(Rect nameRect, string name) {
        if (Event.current.type == EventType.Repaint) {
            boxManager.textStyle.alignment = TextAnchor.LowerLeft;
            if (boxManager.outline) {
                DrawOutline(nameRect, name, boxManager.outlineThickness, boxManager.textStyle);
            }
            boxManager.textStyle.Draw(nameRect, name, false, false, false, false);
        }
    }

    //Stolen from my good friends on stack exchange
    private void DrawOutline(Rect r, string t, int strength, GUIStyle style) {
        GUI.color = new Color(0, 0, 0, 1);
        int i;
        for (i = -strength; i <= strength; i++) {
            GUI.Label(new Rect(r.x - strength, r.y + i, r.width, r.height), t, style);
            GUI.Label(new Rect(r.x + strength, r.y + i, r.width, r.height), t, style);
        }
        for (i = -strength + 1; i <= strength - 1; i++) {
            GUI.Label(new Rect(r.x + i, r.y - strength, r.width, r.height), t, style);
            GUI.Label(new Rect(r.x + i, r.y + strength, r.width, r.height), t, style);
        }
        GUI.color = new Color(1, 1, 1, 1);
    }
}