using UnityEngine;
using System.Collections;

public class PlayerNetworking : MonoBehaviour {

	public float moveSpeed = 10f;
	public float rotationSpeed = 10f;

	float lastSynchronizationTime = 0f;
	float syncDelay = 0f;
	float syncTime = 0f;
	Vector3 syncStartPosition = Vector3.zero;
	Quaternion syncStartRotation= Quaternion.identity;
	Quaternion syncStartCamera= Quaternion.identity;
	Vector3 syncEndPosition = Vector3.zero;
	Quaternion syncEndRotation= Quaternion.identity;
	Quaternion syncEndCamera= Quaternion.identity;
	
	Rigidbody playerRigidbody;
	Transform playerTransform;
	NetworkView nView;
	Transform cameraTransform;

	void Start(){
		nView = GetComponent<NetworkView> ();
		playerTransform = GetComponent<Transform> ();
		playerRigidbody = GetComponent<Rigidbody> ();
		cameraTransform = GetComponentInChildren<Camera>().transform;
	}


	void InputMovement()
	{


		if (Input.GetAxis ("Vertical") != 0) {
			playerRigidbody.MovePosition (playerRigidbody.position + playerTransform.forward * Input.GetAxis ("Vertical") * moveSpeed * Time.deltaTime);
		}

		if (Input.GetAxis ("Horizontal") != 0) {
			playerRigidbody.MoveRotation (playerRigidbody.rotation * Quaternion.Euler(0,Input.GetAxis ("Horizontal") * rotationSpeed * Time.deltaTime,0));
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		Quaternion syncRotation = Quaternion.identity;
		Quaternion syncCamera = Quaternion.identity;


		if (stream.isWriting)
		{
			syncPosition = playerRigidbody.position;
			stream.Serialize(ref syncPosition);

			syncVelocity = playerRigidbody.velocity;
			stream.Serialize(ref syncVelocity);

			syncRotation = playerRigidbody.rotation;
			stream.Serialize(ref syncRotation);

			syncCamera = cameraTransform.rotation;
			stream.Serialize(ref syncCamera);
		}
		else
		{
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
			stream.Serialize(ref syncRotation);
			stream.Serialize(ref syncCamera);
			
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			syncEndPosition = syncPosition + syncVelocity * syncDelay;
			syncEndRotation = syncRotation;
			syncEndCamera = syncCamera;
			syncStartPosition = playerRigidbody.position;
			syncStartRotation = playerRigidbody.rotation;
			syncStartCamera = cameraTransform.rotation;
		}
	}

	private void SyncedMovement()
	{
		syncTime += Time.deltaTime;
		playerRigidbody.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
		playerRigidbody.rotation = Quaternion.Lerp(syncStartRotation, syncEndRotation, syncTime / syncDelay);
		cameraTransform.rotation = Quaternion.Lerp(syncStartCamera, syncEndCamera, syncTime / syncDelay);
	}

	void Update()
	{
		if (nView.isMine)
		{
			InputMovement();
		}
		else
		{
			SyncedMovement();
		}
	}

}
