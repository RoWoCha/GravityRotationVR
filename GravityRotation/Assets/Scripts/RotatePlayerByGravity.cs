using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlayerByGravity : MonoBehaviour
{

    // Lerp data
    Quaternion startRotation;
    Quaternion targetRotation;
    public float lerpSpeed = 1.0F;
    float startTime;
    float lerpLength;
    bool isLerping = false;
    public float maxLerpOffset = 0.001f;
    Quaternion currentLerpRotation;
    Vector3 startLerpPosition;

    Transform gravityPointerTransform;

    // Lift vector distance that gets applied before starting rotation
    public float liftDistance = 1f;

    private void Start()
    {
        gravityPointerTransform = GravityPointer.instance.transform;
    }

    void Update()
    {
        // Starts player rotation when gravity pointer finished updating gravity direction
        if (GravityPointer.instance.shouldUpdatePlayerRotation)
        {
            GravityPointer.instance.shouldUpdatePlayerRotation = false;
            
            startRotation = transform.rotation;
            targetRotation = gravityPointerTransform.rotation;
            lerpLength = Vector3.Distance(transform.up.normalized, gravityPointerTransform.up.normalized);
            startTime = Time.time;
            isLerping = true;

            // Lifting up the player to insure they don't get stuck in the ground
            startLerpPosition = transform.position + transform.up.normalized * liftDistance;
        }

        if (isLerping)
        {
            transform.position = startLerpPosition;
            float changeCovered = (Time.time - startTime) * lerpSpeed;
            float fractionOfChange = changeCovered / lerpLength;
            currentLerpRotation = Quaternion.Slerp(startRotation, targetRotation, fractionOfChange);
            transform.rotation = currentLerpRotation;

            if (Vector3.Distance(transform.up.normalized, gravityPointerTransform.up.normalized) < maxLerpOffset)
            {
                isLerping = false;
            }
        }
    }
}
