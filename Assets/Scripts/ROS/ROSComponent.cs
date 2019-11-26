using System;
using UnityEngine;

public abstract class ROSComponent : MonoBehaviour {
    protected static ROSConnector connector;
    private bool isEnabled;
    public bool IsEnabled {
        get { return isEnabled; }
        set => isEnabled = value && (connector.status == ROSConnector.Status.SUCCESS);
    }

    virtual protected void Start() {
        connector = ROSConnector.Instance;
        isEnabled = connector.status == ROSConnector.Status.SUCCESS;
    }
}

