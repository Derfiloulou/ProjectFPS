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
	Text chatBoxText;
	RectTransform chatBoxRectTransform;
	public Scrollbar chatBoxScrollBar;
	public InputField chatBoxInputField;
	[Header("Connected players")]
	public GameObject connectedPlayers;
	Text connectedPlayersText;
	RectTransform connectedPlayersRectTransform;
	public Scrollbar connectedPlayersScrollBar;

	[Header("Server info name")]
	public string info = "INFO";

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


	// Send message chatBox Custom
	public IEnumerator SendMessage(string source, string message){
		
		Vector2 chatboxSize = chatBoxRectTransform.sizeDelta;
		
		chatBoxText.text += "<" + source + "> : " + message + "\n\r";
		chatboxSize.y += (chatBoxText.lineSpacing*2 + chatBoxText.fontSize);
		chatBoxRectTransform.sizeDelta = chatboxSize;
		yield return new WaitForEndOfFrame();
		chatBoxScrollBar.value = 0;
	}

	// Refresh des joeuurs connectés

	public IEnumerator RefreshConnectedPlayers(){
		
		foreach(NetworkPlayer p in Network.connections){
			Vector2 connectedPlayersSize = connectedPlayersRectTransform.sizeDelta;
			connectedPlayersText.text += p.ipAddress;
			connectedPlayersSize.y += (connectedPlayersText.lineSpacing*2 + connectedPlayersText.fontSize);
			connectedPlayersRectTransform.sizeDelta = connectedPlayersSize;
		}

		yield return new WaitForEndOfFrame();
		connectedPlayersScrollBar.value = 0;
		yield return new WaitForSeconds(1);
		StartCoroutine(RefreshConnectedPlayers());
	}


	void Start () {
		chatBoxRectTransform = chatBox.GetComponent<RectTransform>();
		chatBoxText= chatBox.GetComponent<Text>();
		StartCoroutine(SendMessage(info, "Your IP adress is " + Network.player.ipAddress));
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

			if(chatBoxInputField.text != ""){
				StartCoroutine(SendMessage(playerNameInputField.text, chatBoxInputField.text));
			}

			chatBoxInputField.text = "";
			chatBoxInputField.DeactivateInputField();
			colorBlock.colorMultiplier = 1;
			chatBoxInputField.colors = colorBlock;
			isChatActive = false;
		}
	}
}
