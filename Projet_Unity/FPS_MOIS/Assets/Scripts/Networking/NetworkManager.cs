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
		if (!Network.isClient && !Network.isServer && GUIManager.instance.isRunning == true)
		{
			StartServer();
		}else if(!Network.isClient && !Network.isServer && GUIManager.instance.isRunning == false){
			StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Start ServerManager before creating a server !"));
		}else if(Network.isClient){
			StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "You can't create a server while you are a client !"));
		}else if(Network.isServer){
			StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Server already initialized !"));
		}
	}

	// Rejoindre le serveur GUI
	public void GUIJoinServer(){
		if (!Network.isClient && !Network.isServer)
		{
			JoinServer();
		}else if(Network.isServer){
			StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "You can't join while you are a server !"));
		}
	}

	// Refresh la liste des hotes GUI -- pour un dédié, ne marchera pas en local car pas d'ip fixe pour le serveur
//	public void GUIRefreshHostList(){
//		if (!Network.isClient && !Network.isServer)
//		{
//			RefreshHostList();
//			StartCoroutine(GUIManager.instance.SendMessage("INFO", "Refreshed"));
//		}else if(Network.isServer){
//			StartCoroutine(GUIManager.instance.SendMessage("INFO", "You can't refresh while you are the server !"));
//		}
//	}

	// Quitter le serveur GUI
	public void GUILeaveServer(){
		if (Network.isClient || Network.isServer)
		{
			LeaveServer();
		}else{
			StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Can't disconnect if you are not connected !"));
		}
	}
	
	// ========================= Fonctions du serveur =========================

	void Start(){
//		StartCoroutine(AutoRefresh());
	}

	
	// Demarrage du serveur
	private void StartServer()
	{
		string serverPort = "";
		MasterServer.ipAddress = "127.0.0.1";
		serverPort = GUIManager.instance.portCreateInputField.text;
		if(GUIManager.instance.portCreateInputField.text == ""){
			serverPort = "25000";
			GUIManager.instance.portCreateInputField.text = serverPort;
		}else{
			serverPort = GUIManager.instance.portCreateInputField.text;
		}
		Network.InitializeServer(4, int.Parse(serverPort), !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}



	// Appelée automatiquement quand le serveur est initialisé
	void OnServerInitialized()
	{
		StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Server initialized !"));
	}

	void OnFailedToConnect(NetworkConnectionError error){
		StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Failed to connect to the server : " + error));
	}

	void OnDisconnectedFromServer(NetworkDisconnection message){
		if (Network.isServer)
			StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Local server connection disconnected."));
		else
			if (message == NetworkDisconnection.LostConnection)
				StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Connection lost !"));
		else
			StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Succesfully disconnected from the server"));
	}


	// Refresh la liste des hotes
//	private void RefreshHostList()
//	{
//		isRefreshing = true;
//		MasterServer.ClearHostList();
//		MasterServer.RequestHostList(typeName);
//
//	}

//	IEnumerator AutoRefresh(){
//		RefreshHostList();
//		yield return new WaitForSeconds(1);
//		StartCoroutine(AutoRefresh());
//	}

	// On MasterServer event
//	void OnMasterServerEvent(MasterServerEvent msEvent)
//	{
//		if (msEvent == MasterServerEvent.HostListReceived)
//			hostList = MasterServer.PollHostList();
//	}



	// Rejoindre le serveur
	private void JoinServer()
	{
		string _ip = GUIManager.instance.IPJoinInputField.text;
		int _port;
		bool isNumeric = int.TryParse(GUIManager.instance.portJoinInputField.text, out _port);
		string[] ipCheck =_ip.Split('.');

		if(ipCheck.Length != 4){
			StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "You must enter a valid IP adress !"));
		}
		if(isNumeric && ipCheck.Length == 4){
			
			foreach(string s in ipCheck){
				int i = 0;
				bool check = int.TryParse(s, out i);
				if(check == false){
					StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "You must enter a valid IP adress !"));
					break;
				} 
				else{Network.Connect(_ip, _port);}
			}
		}
		if(!isNumeric){
			StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "You must enter a valid port !"));
		}

	}

	private void LeaveServer(){
		Network.Disconnect();
	}



	// Rappelée automatiquement quand connecté au serveur
	void OnConnectedToServer()
	{
		StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Server joined !"));
	}



	void Update(){

//		if (hostList != null && isRefreshing == true)
//		{
//			foreach(GameObject i in GUIManager.instance.hostButtonList){
//				Destroy(i);
//			}
//			GUIManager.instance.hostButtonList.Clear();
//			
//			for (int i = 0; i < hostList.Length; i++){
//
////				if(hostList[i].ip[0] == Network.player.ipAddress){
////					break;
////				}
//
//				GUIManager.instance.CreateButton("Host " + hostList[i].ip[0].ToString(), i);
//			}
//			isRefreshing = false;
//		}
	}
}
