using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NetworkManagerLocal : MonoBehaviour {

	public int maxPlayers = 4;
	public List<string> playerList = new List<string>();

	[Header("Server messages")]
	public string info = "INFO";
	public string error = "ERROR";

	[HideInInspector]
	public NetworkView nView;
	[HideInInspector]
	public string playerName;
	string talkingPlayer;

	// Singleton
	static NetworkManagerLocal mInst;
	static public NetworkManagerLocal instance { get { return mInst; } }	
	GUIManagerLocal guiManagerLocal;
	
	void Awake () {
		if(mInst == null) mInst = this;
		DontDestroyOnLoad(this); 
		guiManagerLocal = GUIManagerLocal.instance;
	}

	void Start(){
		nView = GetComponent<NetworkView> ();
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
			SendDebugMessageInChat(error, "Enter a player name !");
		}else if(guiManagerLocal.serverNameInputField.text == ""){
			SendDebugMessageInChat(error, "Enter a server name !");
		}else if (!Network.isClient && !Network.isServer){
			StartServer();
		}
	}
	
	// Rejoindre le serveur GUI
	public void GUIJoinServer(){
		if(guiManagerLocal.playerNameInputField.text == ""){
			SendDebugMessageInChat(error, "Enter a player name !");
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
					SendDebugMessageInChat(error, "Enter a valid IP adress !");
					break;
				} 

				// Vérification du port
				else if(!isNumericPort) {
					SendDebugMessageInChat(error, "Enter a valid port !"); 
					break;
				}

				else {
					Network.Connect(_ip, _port);
					SendDebugMessageInChat(info, "Trying to join : " + _ip + "...");
					break;
				}
			}
		}else{
			SendDebugMessageInChat(error, "Enter a valid IP adress !");
		}
	}

	public void LeaveServer(){
		Network.Disconnect();
		StopAllCoroutines();
		guiManagerLocal.connectedPlayers.GetComponent<Text>().text = "";
	}

	void OnFailedToConnect(NetworkConnectionError errorMessage){
		SendDebugMessageInChat(error, "Failed to connect to the server : " + errorMessage);
	}

	void OnServerInitialized()
	{
		SendDebugMessageInChat(info, "Server initialized !");
		playerName = guiManagerLocal.playerNameInputField.text;
		StartCoroutine(RefreshConnectedPlayers());
		GUIState(true);

	}

	// Appelée automatiquement quand connecté au serveur
	void OnConnectedToServer()
	{
		SendDebugMessageInChat(info, "Server joined as " + guiManagerLocal.playerNameInputField.text + " !");
		playerName = guiManagerLocal.playerNameInputField.text;
		StartCoroutine(RefreshConnectedPlayers());
		GUIState(true);
	}

	void OnDisconnectedFromServer(NetworkDisconnection message){
		// Fermeture du serveur
		if (Network.isServer)
			SendDebugMessageInChat(info, "Local server connection disconnected.");
		// Connexion perdue
		else if (message == NetworkDisconnection.LostConnection)
			SendDebugMessageInChat(error, "Connection lost !");
		// Deconnexion du serveur
		else
			SendDebugMessageInChat(info, "Disconnected from the server");
		StopAllCoroutines();
		guiManagerLocal.connectedPlayers.GetComponent<Text>().text = "";
		GUIState(false);
	}

	// ========================= Network View =========================

	[RPC]
	public void AddPlayerInfo(string infos){
		if(!playerList.Contains(infos))
			playerList.Add(infos);
	}

	public IEnumerator RefreshConnectedPlayers(){
		
		Vector2 connectedPlayersSize = guiManagerLocal.connectedPlayersRectTransform.sizeDelta;
		Text cpt = guiManagerLocal.connectedPlayersText;
		RectTransform cprt = guiManagerLocal.connectedPlayersRectTransform;
		
		cpt.text = "Connected players :" + "\n\r" + "\n\r";
		nView.RPC("AddPlayerInfo", RPCMode.All, playerName);
		playerList.Sort();

		foreach(string s in playerList){
			cpt.text += s + "\n\r";
		}

		// taille d'une ligne (ecart + taille font) multplié par nombre de ligne (nombre de connexions) + premiere ligne + nom du joueur
		connectedPlayersSize.y = (cpt.lineSpacing*2 + cpt.fontSize)*(playerList.Count+2);
		cprt.sizeDelta = connectedPlayersSize;

		playerList.Clear();

		yield return new WaitForSeconds(1);
		StartCoroutine(RefreshConnectedPlayers());
	}

	// Envoie un debug message dans la chatBox
	public void SendDebugMessageInChat(string source, string message){
		
		Vector2 chatboxSize = guiManagerLocal.chatBoxRectTransform.sizeDelta;
		Text cbt = guiManagerLocal.chatBoxText;
		RectTransform cbrt = guiManagerLocal.chatBoxRectTransform;

		cbt.text += "<" + source + "> : " + message + "\n\r";
		chatboxSize.y += (cbt.lineSpacing*2 + cbt.fontSize);
		cbrt.sizeDelta = chatboxSize;
		guiManagerLocal.chatBoxScrollBar.value = 0;
	}

	// Envoie un message dans le chat


	[RPC]
	public void SendMessageInChat(string name, string message){
		
		Vector2 chatboxSize = guiManagerLocal.chatBoxRectTransform.sizeDelta;
		Text cbt = guiManagerLocal.chatBoxText;
		RectTransform cbrt = guiManagerLocal.chatBoxRectTransform;

		cbt.text += "<" + name + "> : " + message + "\n\r";
		chatboxSize.y += (cbt.lineSpacing*2 + cbt.fontSize);
		cbrt.sizeDelta = chatboxSize;
		guiManagerLocal.chatBoxScrollBar.value = 0; // ceci est buggé
	}



	

}
