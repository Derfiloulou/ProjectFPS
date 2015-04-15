using UnityEngine;
using System.Collections;

public class PlayerNetworking : MonoBehaviour {

	public float moveSpeed = 10f;
	public float rotationSpeed = 10f;

	float lastSynchronizationTime = 0f;
	float syncDelay = 0f;
	float syncTime = 0f;
	Vector3 syncStartPosition = Vector3.zero;
	Vector3 syncEndPosition = Vector3.zero;
	
	Rigidbody playerRigidbody;
	Transform playerTransform;
	NetworkView nView;

	void Start(){
		nView = GetComponent<NetworkView> ();
		playerTransform = GetComponent<Transform> ();
		playerRigidbody = GetComponent<Rigidbody> ();
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


		if (stream.isWriting)
		{
			syncPosition = playerRigidbody.position;
			stream.Serialize(ref syncPosition);

			syncVelocity = playerRigidbody.velocity;
			stream.Serialize(ref syncVelocity);
		}
		else
		{
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
			
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			syncEndPosition = syncPosition + syncVelocity * syncDelay;
			syncStartPosition = playerRigidbody.position;
		}
	}

	private void SyncedMovement()
	{
		syncTime += Time.deltaTime;
		playerRigidbody.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
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
