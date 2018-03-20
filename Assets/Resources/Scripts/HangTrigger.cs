using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangTrigger : MonoBehaviour {

    public GameObject ObjectToTrigger = null;

    void OnPlayerHang()
    {
        if (ObjectToTrigger != null)
        {
            ObjectToTrigger.SendMessage("OnTrigger", SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnPlayerUnhang()
    {
        if (ObjectToTrigger != null)
        {
            ObjectToTrigger.SendMessage("OnUnTrigger", SendMessageOptions.DontRequireReceiver);
        }
    }
}
