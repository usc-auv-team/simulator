using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDFocus : MonoBehaviour
{
    private bool focused = false;
    public void OnFocus() {
        focused = !focused;
        if (focused) {
            Time.timeScale = 0.0001f;
        }
        else {
            Time.timeScale = 1f;
        }
    }
}
