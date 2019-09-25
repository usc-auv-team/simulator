using System;
using UnityEngine;

public abstract class ROSComponent : MonoBehaviour {
    protected static ROSConnector connector = ROSConnector.Instance;
    private bool isEnabled = connector.status == ROSConnector.Status.SUCCESS;
    public bool IsEnabled {
        get { return isEnabled; }
        set => isEnabled = value && (connector.status == ROSConnector.Status.SUCCESS);
    }
}

