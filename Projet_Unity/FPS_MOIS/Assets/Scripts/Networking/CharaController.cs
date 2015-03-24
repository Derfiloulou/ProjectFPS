using UnityEngine;
using System.Collections;

public class CharaController : MonoBehaviour {

	NetworkView nView;

	public float speed = 5f;
	public float speedRotate = 1f;

	[RPC]
	public void MoveForward(){
		transform.Translate(Vector3.forward * speed * Time.deltaTime * Input.GetAxis("Vertical"));
	}

	[RPC]
	public void MoveRotate(){
		transform.Rotate(0,speedRotate * Input.GetAxis("Horizontal"), 0);
	}


	void Start () {
		nView = GetComponent<NetworkView> ();
	}
	

	void Update () {
		if(Input.GetAxis("Vertical") != 0){
			nView.RPC("MoveForward", RPCMode.All);
		}
		if(Input.GetAxis("Horizontal") != 0){
			nView.RPC("MoveRotate", RPCMode.All);
		}
	}
}
