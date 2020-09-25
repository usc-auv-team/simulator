using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationVisualization : MonoBehaviour
{
    private Text text;
    private Information information;

    void Start()
    {
        information = GameObject.Find("Submarine").GetComponent<Information>();
        text = GetComponent<Text>();
    }

    void Update()
    {
        UpdateText();
    }

    private void UpdateText() { 
        //Displaying information to 2 sig fig
        text.text = "Acceleration\nx:" + Mathf.Round(information.acceleration.x*100)/100 +
        "\ny:" + Mathf.Round(information.acceleration.y * 100) / 100 +
        "\nz:" + Mathf.Round(information.acceleration.z * 100) / 100 +
        "\n\nPitch:" + Mathf.Round(information.rotation.x * 100) / 100 +
        "\nYaw:" + Mathf.Round(information.rotation.y * 100) / 100 +
        "\nRoll:"+ Mathf.Round(information.rotation.z * 100) / 100;
    }
}
