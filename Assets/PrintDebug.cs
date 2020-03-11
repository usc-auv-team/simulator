using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintDebug : MonoBehaviour
{
    public string s = "";
    public Rect r = new Rect();
    public GUIStyle g = new GUIStyle();
    private BoxManager boxManager;
    private IEnumerator c;

    List<BasicBoundingBox> bbb = new List<BasicBoundingBox>();

    private void Awake() {
        boxManager = BoxManager.getInstance();
        c = MakeString();
        StartCoroutine(c);
    }

    private IEnumerator MakeString() {
        while (true) {
            yield return new WaitForSeconds(0.01f);
            s = "";
            bbb = boxManager.GetBasicBoundingBox();
            foreach(BasicBoundingBox b in bbb) {
                s += b.name + " - tr:(" + b.xMax + ", " + b.yMax +
                    ") | bl:(" + b.xMin + ", " + b.yMin + ")\n"; 
            }
        }
    }

    private void OnGUI() {
        if(Event.current.type == EventType.Repaint) {
            g.alignment = TextAnchor.UpperLeft;
            g.Draw(r, s, false, false, false, false);
        }
    }


}
