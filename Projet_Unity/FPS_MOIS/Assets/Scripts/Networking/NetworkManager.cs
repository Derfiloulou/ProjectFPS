using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	private const string typeName = "UniqueGameName";
	private const string gameName = "RoomName";

	// Singleton
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
			StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Can't create a server while you are a client !"));
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
			StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Can't join while you are a server !"));
		}
	}


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


	
	// Demarrage du serveur
	private void StartServer()
	{
		string serverPort = "";
		// Va chercher le MasterServer sur la machine locale
		MasterServer.ipAddress = "127.0.0.1";
		serverPort = GUIManager.instance.portCreateInputField.text;
		// Si aucune port n'est rentré, alors port 25000 par défaut
		if(GUIManager.instance.portCreateInputField.text == ""){
			serverPort = "25000";
			GUIManager.instance.portCreateInputField.text = serverPort;
		// Sinon port = inputField.text;
		}else{
			serverPort = GUIManager.instance.portCreateInputField.text;
		}
		// Initialize server
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
		// Fermeture du serveur
		if (Network.isServer)
			StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Local server connection disconnected."));
		// Connexion perdue
		else if (message == NetworkDisconnection.LostConnection)
			StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Connection lost !"));
		// Deconnexion du serveur
		else
			StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Succesfully disconnected from the server"));
	}



	// Rejoindre le serveur
	private void JoinServer()
	{
		string _ip = GUIManager.instance.IPJoinInputField.text;
		int _port;
		bool isNumericPort = int.TryParse(GUIManager.instance.portJoinInputField.text, out _port);
		string[] ipCheck =_ip.Split('.');

		// Vérification de l'ip (000.000.000.000)
		if(ipCheck.Length != 4){
			StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Enter a valid IP adress !"));
		}
		if(ipCheck.Length == 4){
			
			foreach(string s in ipCheck){
				int i = 0;
				bool check = int.TryParse(s, out i);
				if(check == false){
					StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Enter a valid IP adress !"));
					break;
				} 
				// Vérification du port
				else if(!isNumericPort) {StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Enter a valid port !")); break;}
				else {
					MasterServer.ipAddress = _ip;
					MasterServer.RequestHostList();
					Network.Connect(_ip, _port);
					StartCoroutine(GUIManager.instance.SendMessage(GUIManager.instance.info, "Trying to join : " + _ip + "..."));
					break;
				}
			}
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

	}
}
