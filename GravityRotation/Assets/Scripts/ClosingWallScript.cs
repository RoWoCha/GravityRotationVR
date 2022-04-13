using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosingWallScript : MonoBehaviour
{
    // Button that triggers opening and closing
    public Valve.VR.InteractionSystem.HoverButton closeRoomButton;

    // End positions
    public Transform openWallMarker;
    public Transform closedWallMarker;

    // Lerp speed
    public float speed = 3.0F;

    // Lerp data
    Vector3 startPosition;
    Vector3 endPosition;
    private float startTime;
    private float journeyLength;


    private void Start()
    {
        closeRoomButton.onButtonDown.AddListener(OnButtonDown);

        startPosition = closedWallMarker.position;
        endPosition = openWallMarker.position;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition, endPosition);
    }

    void Update()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distCovered / journeyLength;

        transform.position = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);
    }

    private void OnButtonDown(Valve.VR.InteractionSystem.Hand hand)
    {
        if (endPosition == closedWallMarker.position)
        {
            endPosition = openWallMarker.position;
        }
        else
        {
            endPosition = closedWallMarker.position;
        }

        startPosition = transform.position;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition, endPosition);
    }
}
