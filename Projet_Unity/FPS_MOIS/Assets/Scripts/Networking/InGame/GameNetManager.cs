using UnityEngine;
using System.Collections;

public class GameNetManager : MonoBehaviour {

	public GameObject playerPrefab;
	
	void Start()
	{
		SpawnPlayer();
	}
	
	private void SpawnPlayer()
	{
		GameObject clone = Network.Instantiate(playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity, 0) as GameObject;
		clone.GetComponentInChildren<TextMesh>().text = NetworkManagerLocal.playerName;
	}
}
