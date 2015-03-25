using UnityEngine;
using System.Collections;

public class Networking : MonoBehaviour {

	private const string typeName = "UniqueGameName";
	private const string gameName = "RoomName";
	
	private void StartServer()
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}

	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
	}

	public void CreateServer(){
		Network.InitializeServer (2, 25000, false);
	}

	public void JoinServer(){
		Network.Connect("10.51.0.218", 25000);
	}

	void Start () {
		Application.runInBackground = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
