using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

[RequireComponent (typeof (Canvas))]
public class GUIManagerLocal : MonoBehaviour {

	// Main Menu
	[HideInInspector]
	public Canvas canvas;

	[Header("Player name")]
	public InputField playerNameInputField;

	[Header("Create server")]
	public Button createServerButton;
	public InputField serverNameInputField;
	public InputField portCreateInputField;

	[Header("Join server")]
	public Button joinServerButton;
	public InputField IPJoinInputField;
	public InputField portJoinInputField;

	[Header("Disconnect")]
	public Button disconnectServerButton;

	[Header("Chat box")]
	public GameObject chatBox;
	[HideInInspector]
	public Text chatBoxText;
	[HideInInspector]
	public RectTransform chatBoxRectTransform;
	public Scrollbar chatBoxScrollBar;
	public InputField chatBoxInputField;

	[Header("Connected players")]
	public GameObject connectedPlayers;
	[HideInInspector]
	public Text connectedPlayersText;
	[HideInInspector]
	public RectTransform connectedPlayersRectTransform;
	public Scrollbar connectedPlayersScrollBar;

	[Header("Start Game")]
	public Button startGameButton;


	// Other
	bool isChatActive = false;

	// Singleton
	static GUIManagerLocal mInst;
	static public GUIManagerLocal instance { get { return mInst; } }

	void Awake () {
		if(mInst == null) mInst = this;
		//DontDestroyOnLoad(this); 		
	}


	// Fonctions pour activer ou désactiver les champs/boutons de connexion

	public void PlayerNameState(bool isInteractable){
		playerNameInputField.interactable = isInteractable;
	}

	public void ChatBoxState(bool isInteractable){
		chatBoxInputField.interactable = isInteractable;
	}

	public void CreateServerState(bool isInteractable){
		createServerButton.interactable = isInteractable;
		serverNameInputField.interactable = isInteractable;
		portCreateInputField.interactable = isInteractable;
		chatBoxInputField.interactable = isInteractable;
		playerNameInputField.interactable = isInteractable;
	}
	
	public void JoinServerState(bool isInteractable){
		joinServerButton.interactable = isInteractable;
		IPJoinInputField.interactable = isInteractable;
		portJoinInputField.interactable = isInteractable;
		chatBoxInputField.interactable = isInteractable;
		playerNameInputField.interactable = isInteractable;
	}

	public void DisconnectState(bool isInteractable){
		disconnectServerButton.interactable = isInteractable;
		chatBoxInputField.interactable = isInteractable;
	}

	public void StartGameState(bool isInteractable){
		disconnectServerButton.interactable = isInteractable;
	}

	// Switch état GUI connecté/déconnecté
	public void GUIState(bool isConnected){
		ChatBoxState (isConnected);
		PlayerNameState(!isConnected);
		CreateServerState(!isConnected);
		JoinServerState(!isConnected);
		DisconnectState(isConnected);
		if(isConnected = false)
			connectedPlayersText.text = "";
	}


	// Etat Chatbox (activé/desactivé)

	void ChangeChatBoxState(){

		ColorBlock colorBlock = chatBoxInputField.colors;

		if (!isChatActive) {
			chatBoxInputField.ActivateInputField ();
			colorBlock.colorMultiplier = 2;
		}

		else if(isChatActive || !chatBoxInputField.isFocused){
			chatBoxInputField.text = "";
			chatBoxInputField.DeactivateInputField();
			colorBlock.colorMultiplier = 1;
		}

		chatBoxInputField.colors = colorBlock;
		isChatActive = !isChatActive;
	}

	// Correctif de la scrollbar pour qu'elle descende jusqu'en bas en cas de nouveau message

	public IEnumerator resetChatScrollBar(){
		yield return new WaitForEndOfFrame();
		chatBoxScrollBar.value = 0;
	}


	//Start

	void Start () {

		canvas = GetComponent <Canvas> ();
		chatBoxRectTransform = chatBox.GetComponent<RectTransform>();
		chatBoxText= chatBox.GetComponent<Text>();
		connectedPlayersRectTransform = connectedPlayers.GetComponent<RectTransform>();
		connectedPlayersText = connectedPlayers.GetComponent<Text>();
		// Donne l'ip du joueur dans la chat box
		NetworkManagerLocal.instance.SendDebugMessageInChat(NetworkManagerLocal.instance.info, "Your IP adress is " + Network.player.ipAddress);
	}

	void Update () {

		if (chatBoxInputField.isFocused){
			isChatActive = false;
			ChangeChatBoxState ();
		}

		// Si lejoueur est connecté
		if (Network.isServer || Network.isClient) {
			// Si le chat n'est pas actif
			if (Input.GetKeyDown (KeyCode.Return)) {
				if (chatBoxInputField.text != "")
					NetworkManagerLocal.instance.nView.RPC ("SendMessageInChat", RPCMode.All, NetworkManagerLocal.playerName, chatBoxInputField.text);
				ChangeChatBoxState ();
			}
		}

		if (Network.isServer && NetworkManagerLocal.instance != null && NetworkManagerLocal.instance.playerList.Count >= NetworkManagerLocal.instance.minPlayersToLaunch)
			startGameButton.interactable = true;
		else
			startGameButton.interactable = false;
	}
}
