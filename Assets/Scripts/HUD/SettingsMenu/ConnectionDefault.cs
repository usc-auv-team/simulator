using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionDefault : MonoBehaviour
{

    [SerializeField] private SettingsFields defaultSettings = null;
    [SerializeField] private InputField ipAddress = null;
    [SerializeField] private InputField port = null;


    private void Start()
    {
        if (ipAddress) {
            ipAddress.text = defaultSettings.ipAddress;
        }

        if (port) {
            port.text = defaultSettings.port;
        }
    }
}
