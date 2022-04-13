using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


[RequireComponent(typeof(Interactable))]
public class InteractableRotationBall : MonoBehaviour
{
	Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers) & (~Hand.AttachmentFlags.VelocityMovement);

	// Interactable script of the device
	Interactable interactable;

	// Rotation sensor of the device
	public RotationSensorScript rotationSensor;

	// Viewable part of the device
	public GameObject viewableDevice;

	// Starting data
	Vector3 startPos;
	Quaternion startRot;

	// On attachement data
	Quaternion onAttachedRot;
	Vector3 onAttachedPlayerPosition = Vector3.zero;

	// Speed of slerp updating the viewable part's rotation
	public float rotationSmooth = 5.0f;

	// Speed of gravity rotation
	public float gravityRotationSpeed = 0.3f;

	public Transform playerTransform;

	void Awake()
	{
		rotationSensor.UpdateGravityRotationSpeed(gravityRotationSpeed);
		interactable = this.GetComponent<Interactable>();
		startPos = transform.position;
		startRot = transform.rotation;
	}

	private void Update()
	{
		transform.position = startPos;
	}

	//-------------------------------------------------
	// Called every Update() while a Hand is hovering over this object
	//-------------------------------------------------
	private void HandHoverUpdate(Hand hand)
	{
		GrabTypes startingGrabType = hand.GetGrabStarting();
		bool isGrabEnding = hand.IsGrabEnding(this.gameObject);

		if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
		{
			// Call this to continue receiving HandHoverUpdate messages,
			// and prevent the hand from hovering over anything else
			hand.HoverLock(interactable);

			// Attach this object to the hand
			hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
		}
		else if (isGrabEnding)
		{
			// Detach this object from the hand
			hand.DetachObject(gameObject);

			// Call this to undo HoverLock
			hand.HoverUnlock(interactable);
		}
	}

	//-------------------------------------------------
	// Called when this GameObject becomes attached to the hand
	//-------------------------------------------------
	private void OnAttachedToHand(Hand hand)
	{
		onAttachedRot = transform.rotation;
		onAttachedPlayerPosition = playerTransform.position;
	}

	//-------------------------------------------------
	// Called when this GameObject is detached from the hand
	//-------------------------------------------------
	private void OnDetachedFromHand(Hand hand)
	{
		transform.rotation = onAttachedRot;
		viewableDevice.transform.rotation = onAttachedRot;
		GravityPointer.instance.FinishGravityRotation();
	}

	//-------------------------------------------------
	// Called every Update() while this GameObject is attached to the hand
	//-------------------------------------------------
	private void HandAttachedUpdate(Hand hand)
	{
		// Locking position of the player
		playerTransform.position = onAttachedPlayerPosition;
		playerTransform.GetComponent<Rigidbody>().velocity = Vector3.zero;

		// Rotating the viewable part of the device
		Quaternion target = Quaternion.Euler(transform.rotation.eulerAngles.x - onAttachedRot.eulerAngles.x + startRot.eulerAngles.x,
											 transform.rotation.eulerAngles.y - onAttachedRot.eulerAngles.y + startRot.eulerAngles.y,
											 transform.rotation.eulerAngles.z - onAttachedRot.eulerAngles.z + startRot.eulerAngles.z);
		viewableDevice.transform.rotation = Quaternion.Slerp(viewableDevice.transform.rotation, target, Time.deltaTime * rotationSmooth);

		// Updating gravity direction
		rotationSensor.UpdateGravityDir();
	}
}

