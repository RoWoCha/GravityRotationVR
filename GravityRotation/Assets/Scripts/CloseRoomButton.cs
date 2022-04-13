using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class CloseRoomButton : MonoBehaviour
{
    public void OnButtonPress(Valve.VR.InteractionSystem.Hand hand)
    {
        ChangeColor(Color.green);
        hand.TriggerHapticPulse(800);
    }

    public void OnButtonRelease(Valve.VR.InteractionSystem.Hand hand)
    {
        ChangeColor(Color.white);
    }

    void ChangeColor(Color color)
    {
        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = color;
    }
}
