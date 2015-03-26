using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Networking : MonoBehaviour {

	private const string typeName = "ThisIsMyFPS";
	private const string gameName = "ThisIsMyFPSRoom";
	private HostData[] hostList;

	bool isRefreshing = false;





	// ========================= Fonctions GUI =========================

	// Demarrage du serveur GUI
	public void GUIStartServer(){
		if (!Network.isClient && !Network.isServer)
		{
			StartServer();
		}else if(Network.isClient){
			StartCoroutine(GUIManager.instance.SendMessage("GAME", "You can't create a server while you are a client !"));
		}else if(Network.isServer){
			StartCoroutine(GUIManager.instance.SendMessage("GAME", "Server already initialized !"));
		}
	}

	// Rejoindre le serveur GUI
	public void GUIJoinServer(){
		if (!Network.isClient && !Network.isServer)
		{
			JoinServer(hostList[0]);
		}else if(Network.isServer){
			StartCoroutine(GUIManager.instance.SendMessage("GAME", "You can't join while you are a server !"));
		}
	}

	// Refresh la liste des hotes GUI
	public void GUIRefreshHostList(){
		if (!Network.isClient && !Network.isServer)
		{
			RefreshHostList();
		}else if(Network.isServer){
			StartCoroutine(GUIManager.instance.SendMessage("GAME", "You can't refresh while you are the server !"));
		}
	}
	
	// ========================= Fonctions du serveur =========================

	void Start(){

	}


	
	// Demarrage du serveur
	private void StartServer()
	{
		MasterServer.ipAddress = "127.0.0.1";
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);


	}

	// Appelée automatiquement quand le serveur est initialisé
	void OnServerInitialized()
	{
		StartCoroutine(GUIManager.instance.SendMessage("GAME", "Server initialized !"));
	}

	// Refresh la liste des hotes
	private void RefreshHostList()
	{
		isRefreshing = true;
		StartCoroutine(GUIManager.instance.SendMessage("GAME", "Refreshed"));
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
		StartCoroutine(GUIManager.instance.SendMessage("GAME", "Server joined !"));
	}

	void Update(){

		if (hostList != null && isRefreshing == true)
		{
			foreach(GameObject i in GUIManager.instance.hostButtonList){
				Destroy(i);
			}
			GUIManager.instance.hostButtonList.Clear();
			
			for (int i = 0; i < hostList.Length; i++)
			{
				GUIManager.instance.CreateButton("Host " + hostList[i].ip[0].ToString());
			}
			isRefreshing = false;
		}
	}
}
