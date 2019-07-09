using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{

    private bool isActive = false;

    public void Start()
    {
        // By default, this game object should be disabled
        gameObject.SetActive(isActive);
    }

    public void ToggleActive()
    {
        isActive = !isActive;
        gameObject.SetActive(isActive);
    }
}
