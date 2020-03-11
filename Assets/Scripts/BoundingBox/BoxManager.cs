using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class BoxManager {
    private static BoxManager boxManager;
    public bool debug = true;
    public bool displayNames = true;
    public bool outline = false;

    public Color boxColor = new Color(0, 0, 1, 0.2f);
    public Color textBgColor = new Color(1, 1, 1, 1);
    public int outlineThickness = 1;
    public float boxHeight = 20f, boxWidth = 60f;
    public GUIStyle boxStyle = new GUIStyle();
    public GUIStyle textStyle = new GUIStyle();
    public int borderSize;
    public Texture2D boxTexture;
    public Texture2D whiteTexture;
    public Camera camera;

    public List<BoundingBox> boundingBoxes = new List<BoundingBox>();

    private BoxManager() {
        GenerateWhiteTexture();
        camera = Camera.main;
    }

    public static BoxManager getInstance() {
        if (boxManager == null)
            boxManager = new BoxManager();
        return boxManager;
    }

    private void GenerateWhiteTexture() {
        whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, textBgColor);
        whiteTexture.Apply();
    }

    public List<BasicBoundingBox> GetBasicBoundingBox() {
        List<BasicBoundingBox> ret = new List<BasicBoundingBox>(); 
        foreach(BoundingBox b in boundingBoxes) {
            if (!b.offScreen) {
                ret.Add(new BasicBoundingBox(b.name, (int)b.xMax, (int)b.xMin,
                    (int)b.yMax, (int)b.yMin));
            }
        }
        return ret;
    }

}