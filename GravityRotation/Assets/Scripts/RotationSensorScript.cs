using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSensorScript : MonoBehaviour
{
    // Starting position
    Vector3 startPos;

    // Gravity rotation speed
    float rotSpeed;

    // Type of controller based on its direction (upwards)
    public enum SensorDirection
    {
        FORWARD,
        BACKWARD,
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    public SensorDirection sensorDirection;

    void Start()
    {
        startPos = transform.position;
    }

    // Calculates delta for gravity direction and applies it to the gravity pointer
    public void UpdateGravityDir()
    {
        Vector3 positionChange = transform.position - startPos;
        
        switch (sensorDirection)
        {
            case SensorDirection.UP:
                {
                    GravityPointer.instance.UpdatePointerDirection(new Vector3(-positionChange.z * rotSpeed * Time.deltaTime, 0.0f, positionChange.x * rotSpeed * Time.deltaTime));
                    break;
                }
            case SensorDirection.DOWN:
                {
                    GravityPointer.instance.UpdatePointerDirection(new Vector3(positionChange.z * rotSpeed * Time.deltaTime, 0.0f, positionChange.x * rotSpeed * Time.deltaTime));
                    break;
                }
            case SensorDirection.LEFT:
                {
                    GravityPointer.instance.UpdatePointerDirection(new Vector3(positionChange.y * rotSpeed * Time.deltaTime, 0.0f, positionChange.x * rotSpeed * Time.deltaTime));
                    break;
                }
            case SensorDirection.RIGHT:
                {
                    GravityPointer.instance.UpdatePointerDirection(new Vector3(-positionChange.y * rotSpeed * Time.deltaTime, 0.0f, positionChange.x * rotSpeed * Time.deltaTime));
                    break;
                }
            case SensorDirection.FORWARD:
                {
                    GravityPointer.instance.UpdatePointerDirection(new Vector3(-positionChange.z * rotSpeed * Time.deltaTime, 0.0f, -positionChange.y * rotSpeed * Time.deltaTime));
                    break;
                }
            case SensorDirection.BACKWARD:
                {
                    GravityPointer.instance.UpdatePointerDirection(new Vector3(-positionChange.z * rotSpeed * Time.deltaTime, 0.0f, positionChange.y * rotSpeed * Time.deltaTime));
                    break;
                }

        }
    }

    // Sets gravity rotation speed to the incoming value
    public void UpdateGravityRotationSpeed(float newSpeed)
    {
        rotSpeed = newSpeed;
    }
}
