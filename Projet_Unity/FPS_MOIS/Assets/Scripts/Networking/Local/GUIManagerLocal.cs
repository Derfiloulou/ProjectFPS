using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;

public class GUIManagerLocal : MonoBehaviour {

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
	string textInChatBox;
	

	bool isChatActive = false;


	static GUIManagerLocal mInst;
	static public GUIManagerLocal instance { get { return mInst; } }

	void Awake () {
		if(mInst == null) mInst = this;
		DontDestroyOnLoad(this); 		
	}

	public void PlayerNameState(bool isInteractable){
		playerNameInputField.interactable = isInteractable;
	}
	
	public void CreateServerState(bool isInteractable){
		createServerButton.interactable = isInteractable;
		serverNameInputField.interactable = isInteractable;
		portCreateInputField.interactable = isInteractable;
	}
	
	public void JoinServerState(bool isInteractable){
		joinServerButton.interactable = isInteractable;
		IPJoinInputField.interactable = isInteractable;
		portJoinInputField.interactable = isInteractable;
	}

	public void DisconnectState(bool isInteractable){
		disconnectServerButton.interactable = isInteractable;
	}

	IEnumerator resetChatScrollBar(){
		yield return new WaitForEndOfFrame();
		chatBoxScrollBar.value = 0;
	}

	void Start () {
		chatBoxRectTransform = chatBox.GetComponent<RectTransform>();
		chatBoxText= chatBox.GetComponent<Text>();
		connectedPlayersRectTransform = connectedPlayers.GetComponent<RectTransform>();
		connectedPlayersText = connectedPlayers.GetComponent<Text>();
		NetworkManagerLocal.instance.SendDebugMessageInChat(NetworkManagerLocal.instance.info, "Your IP adress is " + Network.player.ipAddress);
	}

	void Update () {

		ColorBlock colorBlock = chatBoxInputField.colors;
		
		if((Input.GetKeyDown(KeyCode.Return) && !isChatActive)  || chatBoxInputField.isFocused == true){
			chatBoxInputField.ActivateInputField();
			colorBlock.colorMultiplier = 2;
			chatBoxInputField.colors = colorBlock;
			isChatActive = true;
		}
		if((Input.GetKeyDown(KeyCode.Return) && isChatActive == true)  || chatBoxInputField.isFocused == false){

			if((Network.isServer || Network.isClient) && chatBoxInputField.text != ""){
				NetworkManagerLocal.instance.nView.RPC("SendMessageInChat", RPCMode.All, NetworkManagerLocal.instance.playerName, chatBoxInputField.text);
			}

			chatBoxInputField.text = "";
			chatBoxInputField.DeactivateInputField();
			colorBlock.colorMultiplier = 1;
			chatBoxInputField.colors = colorBlock;
			isChatActive = false;

		}

		if(chatBoxText.text != textInChatBox){
			StartCoroutine(resetChatScrollBar());
		}
		textInChatBox = chatBoxText.text;
	}
}
