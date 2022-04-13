using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPointer : MonoBehaviour
{
    // Instance of the gravity pointer
    public static GravityPointer instance = null;

    // Gravity euler angles
    [HideInInspector] public Vector3 gravityDirection;

    // Gravitational strength
    public float gravityStrength = 9.81f;

    // Lerp data
    Quaternion startRotation;
    Quaternion endRotation;
    public float lerpSpeed = 1.0F;
    float startTime;
    float lerpLength;
    bool isLerping = false;
    public float maxLerpGravityOffset = 0.001f;
    int closestAxisIndex = 0;

    // Updates player rotation only when gravity rotation is done
    [HideInInspector] public bool shouldUpdatePlayerRotation = false;

    struct VectorWithRotation
    {
        public Vector3 vector;
        public Quaternion rotation;

        public VectorWithRotation(Vector3 startVec, Quaternion startRot) {vector = startVec; rotation = startRot; }
    }

    // Axis vectors and their corresponding direction quaternions
    VectorWithRotation[] axisArray = { new VectorWithRotation(Vector3.up,       new Quaternion(0,0,0,1)),
                                       new VectorWithRotation(Vector3.down,     new Quaternion(1,0,0,0)),
                                       new VectorWithRotation(Vector3.right,    new Quaternion(0,0,-0.7071068f,0.7071068f)),
                                       new VectorWithRotation(Vector3.left,     new Quaternion(0,0,0.7071068f,0.7071068f)),
                                       new VectorWithRotation(Vector3.forward,  new Quaternion(0.7071068f,0,0,0.7071068f)),
                                       new VectorWithRotation(Vector3.back,     new Quaternion(-0.7071068f,0,0,0.7071068f))};

    void Awake()
    {
        // Creates the instance of the gravity pointer
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        gravityDirection = transform.rotation.eulerAngles;
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float changeCovered = (Time.time - startTime) * lerpSpeed;
            float fractionOfChange = changeCovered / lerpLength;
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, fractionOfChange);
            gravityDirection = transform.rotation.eulerAngles;
            SetGravity();

            if (Vector3.Distance(transform.up.normalized, axisArray[closestAxisIndex].vector) < maxLerpGravityOffset)
            {
                isLerping = false;
                closestAxisIndex = 0;
                shouldUpdatePlayerRotation = true;
            }
        }
    }

    // Updates pointer's direction and applies it to gravity
    public void UpdatePointerDirection(Vector3 gravityDelta)
    {
        isLerping = false;
        
        // Update gravity pointer's euler angles
        gravityDirection += gravityDelta;
        transform.eulerAngles = gravityDirection;

        // Update gravity
        SetGravity();
    }

    // Rotates gravity to the closest axis
    public void FinishGravityRotation()
    {
        float minDistance = 10f;

        // Searching for the closest axis to lerp rotation to it
        for (int i = 0; i < axisArray.Length; i++)
        {
            float dist = Vector3.Distance(transform.up.normalized, axisArray[i].vector);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestAxisIndex = i;
            }
        }

        startRotation = transform.rotation;
        endRotation = axisArray[closestAxisIndex].rotation;
        lerpLength = Vector3.Distance(transform.up.normalized, axisArray[closestAxisIndex].vector);
        startTime = Time.time;
        isLerping = true;
    }

    // Sets gravity to the pointer's direction
    void SetGravity()
    {
        Vector3 newGravityVec = -transform.up.normalized * gravityStrength;
        Physics.gravity = newGravityVec;
    }
}
