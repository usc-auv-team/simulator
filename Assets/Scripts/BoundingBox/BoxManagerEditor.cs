using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxManagerEditor : MonoBehaviour {
    private BoxManager boxManager;

    [Header("Debug Settings")]
    public bool debug = true;
    public bool outline = true;
    public bool displayNames = true;

    [Header("Box Settings")]
    public int borderSize = 1;
    public Color boxColor = new Color(0, 0, 1, 0.2f);
    public GUIStyle boxStyle = new GUIStyle();

    [Header("Text Settings")]
    public int outlineThickness = 1;
    public float boxHeight = 20f, boxWidth = 60f;
    public GUIStyle textStyle = new GUIStyle();

    [Header("Misc Settings")]
    public new Camera camera;

    private void Awake() {
        boxManager = BoxManager.getInstance();
    }

    private void Update() {
        boxManager.borderSize = borderSize;
        boxManager.displayNames = displayNames;
        boxManager.debug = debug;
        boxManager.outline = outline;
        boxManager.outlineThickness = outlineThickness;
        boxManager.boxColor = boxColor;
        boxManager.boxHeight = boxHeight;
        boxManager.boxWidth = boxWidth;
        boxManager.textStyle = textStyle;
        boxManager.boxStyle = boxStyle;
    }
}