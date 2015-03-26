using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	private const string typeName = "ThisIsMyFPS";
	private const string gameName = "ThisIsMyFPSRoom";
	private HostData[] hostList;

	bool isRefreshing = false;

	
	static NetworkManager mInst;
	static public NetworkManager instance { get { return mInst; } }		
	
	void Awake () {
		if(mInst == null) mInst = this;
		DontDestroyOnLoad(this); 		
	}



	// ========================= Fonctions GUI =========================

	// Demarrage du serveur GUI
	public void GUIStartServer(){
		if (!Network.isClient && !Network.isServer)
		{
			StartServer();
		}else if(Network.isClient){
			StartCoroutine(GUIManager.instance.SendMessage("INFO", "You can't create a server while you are a client !"));
		}else if(Network.isServer){
			StartCoroutine(GUIManager.instance.SendMessage("INFO", "Server already initialized !"));
		}
	}

	// Rejoindre le serveur GUI
	public void GUIJoinServer(int _id){
		if (!Network.isClient && !Network.isServer)
		{
			JoinServer(hostList[_id]);
		}else if(Network.isServer){
			StartCoroutine(GUIManager.instance.SendMessage("INFO", "You can't join while you are a server !"));
		}
	}

	// Refresh la liste des hotes GUI
	public void GUIRefreshHostList(){
		if (!Network.isClient && !Network.isServer)
		{
			RefreshHostList();
			StartCoroutine(GUIManager.instance.SendMessage("INFO", "Refreshed"));
		}else if(Network.isServer){
			StartCoroutine(GUIManager.instance.SendMessage("INFO", "You can't refresh while you are the server !"));
		}
	}

	// Quitter le serveur GUI
	public void GUILeaveServer(){
		if (Network.isClient || Network.isServer)
		{
			LeaveServer();
		}else{
			StartCoroutine(GUIManager.instance.SendMessage("INFO", "Can't disconnect if you are not connected !"));
		}
	}
	
	// ========================= Fonctions du serveur =========================

	void Start(){
		StartCoroutine(AutoRefresh());
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
		StartCoroutine(GUIManager.instance.SendMessage("INFO", "Server initialized !"));
	}

	void OnFailedToConnect(NetworkConnectionError error){
		StartCoroutine(GUIManager.instance.SendMessage("INFO", "Failed to connect to the server : " + error));
	}

	void OnDisconnectedFromServer(NetworkDisconnection info){
		if (Network.isServer)
			StartCoroutine(GUIManager.instance.SendMessage("INFO", "Local server connection disconnected."));
		else
			if (info == NetworkDisconnection.LostConnection)
				StartCoroutine(GUIManager.instance.SendMessage("INFO", "Connection lost !"));
		else
			StartCoroutine(GUIManager.instance.SendMessage("INFO", "Succesfully disconnected from the server"));
	}


	// Refresh la liste des hotes
	private void RefreshHostList()
	{
		isRefreshing = true;
		MasterServer.ClearHostList();
		MasterServer.RequestHostList(typeName);

	}

	IEnumerator AutoRefresh(){
		RefreshHostList();
		yield return new WaitForSeconds(1);
		StartCoroutine(AutoRefresh());
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

	private void LeaveServer(){
		Network.Disconnect();
	}



	// Rappelée automatiquement quand connecté au serveur
	void OnConnectedToServer()
	{
		StartCoroutine(GUIManager.instance.SendMessage("INFO", "Server joined !"));
	}



	void Update(){

		if (hostList != null && isRefreshing == true)
		{
			foreach(GameObject i in GUIManager.instance.hostButtonList){
				Destroy(i);
			}
			GUIManager.instance.hostButtonList.Clear();
			
			for (int i = 0; i < hostList.Length; i++){

//				if(hostList[i].ip[0] == Network.player.ipAddress){
//					break;
//				}

				GUIManager.instance.CreateButton("Host " + hostList[i].ip[0].ToString(), i);
			}
			isRefreshing = false;
		}
	}
}
