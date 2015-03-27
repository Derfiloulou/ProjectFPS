using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkManagerLocal : MonoBehaviour {

	public int maxPlayers = 4;

	// Singleton
	static NetworkManagerLocal mInst;
	static public NetworkManagerLocal instance { get { return mInst; } }	
	GUIManagerLocal guiManagerLocal;
	
	void Awake () {
		if(mInst == null) mInst = this;
		DontDestroyOnLoad(this); 
		guiManagerLocal = GUIManagerLocal.instance;
	}

	
	// ========================= Fonctions GUI =========================

	public void GUIState(bool isConnected){
		guiManagerLocal.PlayerNameState(!isConnected);
		guiManagerLocal.CreateServerState(!isConnected);
		guiManagerLocal.JoinServerState(!isConnected);
		guiManagerLocal.DisconnectState(isConnected);
	}

	// Demarrage du serveur GUI
	public void GUIStartServer(){
		if(guiManagerLocal.playerNameInputField.text == ""){
			StartCoroutine(guiManagerLocal.SendMessage(guiManagerLocal.info, "Enter a player name !"));
		}else if(guiManagerLocal.serverNameInputField.text == ""){
			StartCoroutine(guiManagerLocal.SendMessage(guiManagerLocal.info, "Enter a server name !"));
		}else if (!Network.isClient && !Network.isServer){
			StartServer();
		}
	}
	
	// Rejoindre le serveur GUI
	public void GUIJoinServer(){
		if(guiManagerLocal.playerNameInputField.text == ""){
			StartCoroutine(guiManagerLocal.SendMessage(guiManagerLocal.info, "Enter a player name !"));
		}else if (!Network.isClient && !Network.isServer){
			JoinServer();
		}
	}
	
	
	// Quitter le serveur GUI
	public void GUILeaveServer(){
		if (Network.isClient || Network.isServer)
		{
			LeaveServer();
		}
	}
	
	// ========================= Fonctions du serveur =========================


	// Demarrage du serveur
	public void StartServer()
	{
		string serverPort = guiManagerLocal.portCreateInputField.text;

		// Si aucune port n'est rentré, alors port 25000 par défaut
		if(guiManagerLocal.portCreateInputField.text == "") serverPort = "25000";


		// Initialize server
		Network.InitializeServer(maxPlayers, int.Parse(serverPort), !Network.HavePublicAddress());
	}



	// Appelée automatiquement quand le serveur est initialisé
	void OnServerInitialized()
	{
		StartCoroutine(guiManagerLocal.SendMessage(guiManagerLocal.info, "Server initialized !"));
		GUIState(true);
	}
	
	void OnFailedToConnect(NetworkConnectionError error){
		StartCoroutine(guiManagerLocal.SendMessage(guiManagerLocal.info, "Failed to connect to the server : " + error));
	}


	void OnDisconnectedFromServer(NetworkDisconnection message){
		// Fermeture du serveur
		if (Network.isServer)
			StartCoroutine(guiManagerLocal.SendMessage(guiManagerLocal.info, "Local server connection disconnected."));
		// Connexion perdue
		else if (message == NetworkDisconnection.LostConnection)
			StartCoroutine(guiManagerLocal.SendMessage(guiManagerLocal.info, "Connection lost !"));
		// Deconnexion du serveur
		else
			StartCoroutine(guiManagerLocal.SendMessage(guiManagerLocal.info, "Disconnected from the server"));
		GUIState(false);
	}



	// Rejoindre le serveur
	public void JoinServer()
	{
		string _ip = guiManagerLocal.IPJoinInputField.text;
		int _port;
		bool isNumericPort = int.TryParse(guiManagerLocal.portJoinInputField.text, out _port);
		string[] ipCheck =_ip.Split('.');

		// Vérification de l'ip (000.000.000.000)
		if(ipCheck.Length == 4){
			
			foreach(string s in ipCheck){
				int i = 0;
				bool check = int.TryParse(s, out i);
				if(check == false){
					StartCoroutine(guiManagerLocal.SendMessage(guiManagerLocal.info, "Enter a valid IP adress !"));
					break;
				} 

				// Vérification du port
				else if(!isNumericPort) {
					StartCoroutine(guiManagerLocal.SendMessage(guiManagerLocal.info, "Enter a valid port !")); 
					break;
				}

				else {
					Network.Connect(_ip, _port);
					StartCoroutine(guiManagerLocal.SendMessage(guiManagerLocal.info, "Trying to join : " + _ip + "..."));
					break;
				}
			}
		}else{
			StartCoroutine(guiManagerLocal.SendMessage(guiManagerLocal.info, "Enter a valid IP adress !"));
		}
	}

	public void LeaveServer(){
		Network.Disconnect();
	}

	
	// Appelée automatiquement quand connecté au serveur
	void OnConnectedToServer()
	{
		StartCoroutine(guiManagerLocal.SendMessage(guiManagerLocal.info, "Server joined as " + guiManagerLocal.playerNameInputField.text + " !"));
		GUIState(true);
	}
	
}
