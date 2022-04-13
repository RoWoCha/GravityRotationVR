using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallZoneScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Object: " + other.gameObject.name + " entered fall zone");
        other.gameObject.SendMessage("OnFall");
    }
}
