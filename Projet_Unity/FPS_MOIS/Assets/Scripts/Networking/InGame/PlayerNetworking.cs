using UnityEngine;
using System.Collections;

public class PlayerNetworking : MonoBehaviour {

	public float speed = 10f;
	Rigidbody playerRigidbody;

	void Start(){
		playerRigidbody = GetComponent<Rigidbody> ();
	}

	void Update()
	{
		InputMovement();
	}
	
	void InputMovement()
	{
		if (Input.GetAxis ("Horizontal") != 0) {
			playerRigidbody.MovePosition (playerRigidbody.position + Vector3.forward * Input.GetAxis ("Horizontal") * speed * Time.deltaTime);
		}

		if (Input.GetAxis ("Vertical") != 0) {
			playerRigidbody.MovePosition (playerRigidbody.position + Vector3.right * Input.GetAxis ("Vertical") * speed * Time.deltaTime);
		}
	}
}
