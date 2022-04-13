using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCollision : MonoBehaviour
{
    public Transform head;

    CapsuleCollider bodyCollider;

    // Start is called before the first frame update
    void Start()
    {
        bodyCollider = gameObject.GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceFromFloor = Vector3.Dot(head.localPosition, Vector3.up);
        bodyCollider.height = Mathf.Max(bodyCollider.radius, distanceFromFloor);
        transform.localPosition = head.localPosition - 0.5f * distanceFromFloor * Vector3.up;

        //transform.position = new Vector3(head.position.x, head.transform.position.y - 0.5f * distanceFromFloor, head.position.z);
    }
}
