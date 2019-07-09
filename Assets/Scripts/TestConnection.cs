using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestConnection : MonoBehaviour {

    private enum Status { NONE, TRYING, FAILED, SUCCESS };

    private Status status = Status.NONE;

    [SerializeField] public GameObject inputField = null;
    [SerializeField] public GameObject statusObject = null;

    private Image statusImage = null;

    private void Start() {

        if (!inputField) {
            Debug.LogError("Failed to read input field.");
        }
        if (!statusObject) {
            Debug.LogError("Failed to read status indicator.");
        }

        statusImage = statusObject.GetComponent<Image>();

        if (!statusImage) {
            Debug.LogError("Failed to get image component of status object.");
        }

        statusImage.color = Color.white;
    }

    private bool ResultFromConnection(string input) {
        // temp method to mimic a connection
        Debug.Log("Trying connection: " + input);
        return true;
    }

    public void Connect() {
        status = Status.TRYING;
        statusImage.color = Color.yellow;

        string url = inputField.GetComponent<Text>().text;
        bool success = ResultFromConnection(url);

        if (success) {
            status = Status.SUCCESS;
            statusImage.color = Color.green;
        }
        else {
            status = Status.FAILED;
            statusImage.color = Color.red;

        }

    }
}
