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

	// Visible part of the device
	public GameObject visiblePart;

	// Starting data
	Vector3 startPos;
	Quaternion startRot;

	// On attachement data
	Quaternion onAttachRot;
	Vector3 onAttachPlayerPosition = Vector3.zero;

	// Speed of slerp updating the visible part's rotation
	public float rotationSmooth = 5.0f;

	// Speed of gravity rotation
	public float gravityRotationSpeed = 0.2f;

	public Transform playerTransform;

	void Awake()
	{
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
		onAttachRot = transform.rotation;
		onAttachPlayerPosition = playerTransform.position;
	}

	//-------------------------------------------------
	// Called when this GameObject is detached from the hand
	//-------------------------------------------------
	private void OnDetachedFromHand(Hand hand)
	{
		transform.rotation = onAttachRot;
		visiblePart.transform.rotation = onAttachRot;
		GravityPointer.instance.FinishGravityRotation();
	}

	//-------------------------------------------------
	// Called every Update() while this GameObject is attached to the hand
	//-------------------------------------------------
	private void HandAttachedUpdate(Hand hand)
	{
		// Locking position of the player
		playerTransform.position = onAttachPlayerPosition;
		playerTransform.GetComponent<Rigidbody>().velocity = Vector3.zero;

		// Rotating the visible part of the device
		Quaternion target = Quaternion.Euler(transform.rotation.eulerAngles.x - onAttachRot.eulerAngles.x + startRot.eulerAngles.x,
											 transform.rotation.eulerAngles.y - onAttachRot.eulerAngles.y + startRot.eulerAngles.y,
											 transform.rotation.eulerAngles.z - onAttachRot.eulerAngles.z + startRot.eulerAngles.z);
		visiblePart.transform.rotation = Quaternion.Slerp(visiblePart.transform.rotation, target, Time.deltaTime * rotationSmooth);

		// Getting a fraction of rotation quaternion
		Quaternion addedRotation = Quaternion.Slerp(Quaternion.identity, visiblePart.transform.localRotation, gravityRotationSpeed * Time.deltaTime);

		// Mirroring the quaternion over Y-axis
		addedRotation.x = -addedRotation.x;
		addedRotation.z = -addedRotation.z;

		// Adding rotation to the Gravity Pointer to finalize rotation on detachment
		GravityPointer.instance.transform.rotation = addedRotation * GravityPointer.instance.transform.rotation;
		
		// Applying rotation to the gravity vector
		Physics.gravity = addedRotation * Physics.gravity;
	}
}

