using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class NetworkManagerLocal : MonoBehaviour {

	public List<PlayerInfo> playerList = new List<PlayerInfo>();
	List<Ping> pingList = new List<Ping>();
	PlayerInfo myInfos;
	string serverBoxText;
	int connectedPlayers;

	[Header("Server Parameters")]
	public int maxPlayers = 4;
	public float refreshPlayerListRate = 1;
	public int minPlayersToLaunch = 2;
	public string gameSceneName;

	[Header("Server messages")]
	public string info = "INFO";
	public string error = "ERROR";

	[HideInInspector]
	public NetworkView nView;
	[HideInInspector]
	public static string playerName;
	string talkingPlayer;
	[HideInInspector]
	public AudioSource audioSource;

	[Header("Sounds")]
	public AudioClip connectedSound;
	public AudioClip disconnectedSound;
	public AudioClip errorSound;
	public AudioClip connectionLostSound;

	// Singleton
	static NetworkManagerLocal mInst;
	static public NetworkManagerLocal instance { get { return mInst; } }	
	GUIManagerLocal guiManagerLocal;
	
	void Awake () {
		if(mInst == null) mInst = this;
		//DontDestroyOnLoad(this); 
		guiManagerLocal = GUIManagerLocal.instance;
	}

	void Start(){
		nView = GetComponent<NetworkView> ();
		audioSource = GetComponent<AudioSource>();
	}


	// Demarrage du serveur GUI
	public void GUIStartServer(){
		if(guiManagerLocal.playerNameInputField.text == ""){
			SendDebugMessageInChat(error, "Enter a player name !");
			audioSource.PlayOneShot(errorSound);
		}else if(guiManagerLocal.serverNameInputField.text == ""){
			SendDebugMessageInChat(error, "Enter a server name !");
			audioSource.PlayOneShot(errorSound);
		}else if (!Network.isClient && !Network.isServer){
			StartServer();
		}
	}
	
	// Rejoindre le serveur GUI
	public void GUIJoinServer(){
		if(guiManagerLocal.playerNameInputField.text == ""){
			SendDebugMessageInChat(error, "Enter a player name !");
			audioSource.PlayOneShot(errorSound);
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

	// Lancer le jeu
	public void GUIStartGame(){
		if (Network.isServer)
		{
			nView.RPC("StartGame", RPCMode.All);
		}
	}
	
	// ========================= Fonctions du serveur =========================


	// Demarrage du serveur
	public void StartServer()
	{
		string serverPort = guiManagerLocal.portCreateInputField.text;
		int _port;
		bool isNumericPort;

		// Si aucune port n'est rentré, alors port 25000 par défaut
		if(guiManagerLocal.portCreateInputField.text == "") 
			serverPort = "25000";

		isNumericPort = int.TryParse(serverPort, out _port);

		if (!isNumericPort) {
			SendDebugMessageInChat (error, "Enter a valid port !"); 
			audioSource.PlayOneShot (errorSound);
		} 

		else 
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
					audioSource.PlayOneShot(errorSound);
					break;
				} 

				// Vérification du port
				else if(!isNumericPort) {
					SendDebugMessageInChat(error, "Enter a valid port !"); 
					audioSource.PlayOneShot(errorSound);
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
			audioSource.PlayOneShot(errorSound);
		}
	}

	public void LeaveServer(){
		Network.Disconnect();
	}

	void OnFailedToConnect(NetworkConnectionError errorMessage){
		SendDebugMessageInChat(error, "Failed to connect to the server : " + errorMessage);
		audioSource.PlayOneShot(errorSound);
	}

	void OnServerInitialized()
	{
		SendDebugMessageInChat(info, "Server initialized !");
		audioSource.PlayOneShot(connectedSound);
		playerName = guiManagerLocal.playerNameInputField.text;
		playerList.Add(new PlayerInfo(Network.player, playerName, 0));
		StartCoroutine(RefreshConnectedPlayers());
		guiManagerLocal.GUIState(true);

	}

	// Appelée automatiquement quand connecté au serveur
	void OnConnectedToServer()
	{
		SendDebugMessageInChat(info, "Server joined as " + guiManagerLocal.playerNameInputField.text + " !");
		audioSource.PlayOneShot(connectedSound);
		playerName = guiManagerLocal.playerNameInputField.text;
		nView.RPC("AddPlayerName", RPCMode.Server, Network.player.guid, playerName);
		StartCoroutine(RefreshConnectedPlayers());
		guiManagerLocal.GUIState(true);
	}

	void OnDisconnectedFromServer(NetworkDisconnection message){

		// Fermeture du serveur
		if (Network.isServer){
			SendDebugMessageInChat(info, "Local server connection disconnected.");
			audioSource.PlayOneShot(disconnectedSound);
		}
		// Connexion perdue
		else if (message == NetworkDisconnection.LostConnection){
			SendDebugMessageInChat(error, "Connection lost !");
			audioSource.PlayOneShot(connectionLostSound);
		}
		// Deconnexion du serveur
		else{
			SendDebugMessageInChat(info, "Disconnected from the server");
			audioSource.PlayOneShot(disconnectedSound);
		}

		StopAllCoroutines();
		playerList.Clear();
		guiManagerLocal.GUIState(false);
	}

	void OnPlayerConnected(NetworkPlayer nPlayer){
		playerList.Add(new PlayerInfo(nPlayer,  "", 0));
	}

	void OnPlayerDisconnected(NetworkPlayer nPlayer){
		for (int i=0; i<playerList.Count; i++) {
			if(playerList[i].netPlayer == nPlayer){
				playerList.Remove(playerList[i]);
				pingList.Remove(pingList[i]);
			}
		}
	}


	public IEnumerator RefreshConnectedPlayers(){
		
		Vector2 connectedPlayersSize = guiManagerLocal.connectedPlayersRectTransform.sizeDelta;
		Text cpt = guiManagerLocal.connectedPlayersText;
		RectTransform cprt = guiManagerLocal.connectedPlayersRectTransform;

		// Gestion de la liste des joueurs et du ping par le serveur
		if(Network.isServer){

			for(int i=0 ; i<pingList.Count; i++){
				if(pingList[i].isDone){
					playerList[i].playerPing = pingList[i].time;
				}
			}
			
			pingList.Clear();

			foreach(PlayerInfo i in playerList){
				Ping ping = new Ping(i.netPlayer.ipAddress);
				pingList.Add(ping);
			}

			cpt.text = guiManagerLocal.serverNameInputField.text + " (" + playerList.Count + "/" + maxPlayers + ")" +"\n\r" + "\n\r";
			cpt.text += "IP ADDRESS" + "\t\t\t" + "PING" + "\t\t" + "PLAYER NAME" +"\n\r";
			foreach(PlayerInfo i in playerList){
				cpt.text += i.netPlayer.ipAddress + "\t\t\t" + i.playerPing + " ms \t\t"  + i.playerName + "\n\r";
			}
			nView.RPC("SendPlayerList", RPCMode.All, cpt.text, playerList.Count);
		}
		
		// taille d'une ligne (ecart + taille font) multplié par nombre de ligne (nombre de joueurs + 4 lignes fixes)
		connectedPlayersSize.y = (cpt.lineSpacing*2 + cpt.fontSize)*(connectedPlayers+4);
		cprt.sizeDelta = connectedPlayersSize;
		
		// refresh rate
		yield return new WaitForSeconds(refreshPlayerListRate);
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
		StartCoroutine(guiManagerLocal.resetChatScrollBar());
	}

	[RPC]
	void StartGame(){
		Application.LoadLevel (gameSceneName);
	}

	// ========================= RPCs =========================

	[RPC]
	public void AddPlayerName(string guid, string name){
		foreach(PlayerInfo p in playerList){
			if(guid == p.netPlayer.guid){
				p.playerName = name;
			}
		}
	}

	[RPC]
	public void SendPlayerList(string serverBoxText, int _connectedPlayers){
		guiManagerLocal.connectedPlayersText.text = serverBoxText;
		connectedPlayers = _connectedPlayers;
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
		StartCoroutine(guiManagerLocal.resetChatScrollBar());
	}
}


[Serializable]
public class PlayerInfo {

	public NetworkPlayer netPlayer;
	public string playerName;
	public int playerPing;
	
	public PlayerInfo(NetworkPlayer _netPlayer, string _playerName, int _playerPing){
		netPlayer = _netPlayer;
		playerName = _playerName;
		playerPing = _playerPing;
	}

}