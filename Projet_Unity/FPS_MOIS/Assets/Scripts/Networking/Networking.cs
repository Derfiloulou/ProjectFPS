using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Networking : MonoBehaviour {

	private const string typeName = "UniqueGameName";
	private const string gameName = "RoomName";
	private HostData[] hostList;



	// ========================= Fonctions GUI =========================

	// Demarrage du serveur GUI
	public void GUIStartServer(){
		if (!Network.isClient && !Network.isServer)
		{
			StartServer();
		}else{
			StartCoroutine(ChatBoxManager.instance.SendMessage("GAME", "Server already initialized !"));
		}
	}

	// Rejoindre le serveur GUI
	public void GUIJoinServer(){
		if (!Network.isClient && !Network.isServer)
		{
			JoinServer(hostList[0]);
		}
	}

	// Refresh la liste des hotes GUI
	public void GUIRefreshHostList(){
		if (!Network.isClient && !Network.isServer)
		{
			RefreshHostList();
		}
	}
	
	// ========================= Fonctions du serveur =========================

	void Start(){

	}
	
	// Demarrage du serveur
	private void StartServer()
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
		MasterServer.ipAddress = "127.0.0.1";
	}

	// Appelée automatiquement quand le serveur est initialisé
	void OnServerInitialized()
	{
		StartCoroutine(ChatBoxManager.instance.SendMessage("GAME", "Server initialized !"));
	}

	// Refresh la liste des hotes
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}

	// On MasterServer event
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	// Rejoindre le serveur
	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	// RAppelée automatiquement quand connecté au serveur
	void OnConnectedToServer()
	{
		StartCoroutine(ChatBoxManager.instance.SendMessage("GAME", "Server joined !"));
	}

	void Update(){
		if (hostList != null)
		{
			for (int i = 0; i < hostList.Length; i++)
			{
				Debug.Log(hostList[i].gameName);
					//JoinServer(hostList[i]);
			}
		}
	}
}
