using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameNetManager : MonoBehaviour {

	public GameObject playerPrefab;
	public GameObject[] players;
	NetworkView nView;

	void Start()
	{
		nView = GetComponent<NetworkView> ();
		SpawnPlayer();
	}
	
	void SpawnPlayer()
	{
		GameObject playerClone = Network.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, 0) as GameObject;
	}

	[RPC]
	void Restart(){
		Application.LoadLevel("Networking_In_Game");
	}
	
	void Update(){
		if(Network.isServer && Input.GetKeyDown(KeyCode.Return)){
			nView.RPC("Restart", RPCMode.All);
		}
	}

}
