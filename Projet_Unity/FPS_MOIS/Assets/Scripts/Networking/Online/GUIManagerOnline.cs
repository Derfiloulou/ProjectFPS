//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections.Generic;
//using System.Collections;
//using System.Diagnostics;
//using System.Linq;
//
//public class GUIManagerOnline : MonoBehaviour {
//
//	public Canvas canvas;
//	public GameObject chatBox;
//	Text chatBoxText;
//	RectTransform chatBoxRectTransform;
//	public Scrollbar chatBoxScrollBar;
//	public InputField chatBoxInputField;
//	public GameObject connectedPlayers;
//	Text connectedPlayersText;
//	RectTransform connectedPlayersRectTransform;
//	public Scrollbar connectedPlayersScrollBar;
//	public InputField portCreateInputField;
//	public InputField IPJoinInputField;
//	public InputField portJoinInputField;
//	public InputField roomName;
//	public string info = "INFO";
//
//	bool isChatActive = false;
//	public bool isRunning = false;
//
//
//	static GUIManager mInst;
//	static public GUIManager instance { get { return mInst; } }		
//
//	void Awake () {
//		if(mInst == null) mInst = this;
//		DontDestroyOnLoad(this); 		
//	}
//
//	// Send message chatBox Custom
//	public IEnumerator SendMessage(string source, string message){
//		
//		Vector2 chatboxSize = chatBoxRectTransform.sizeDelta;
//		
//		chatBoxText.text += "<" + source + "> : " + message + "\n\r";
//		chatboxSize.y += (chatBoxText.lineSpacing*2 + chatBoxText.fontSize);
//		chatBoxRectTransform.sizeDelta = chatboxSize;
//		yield return new WaitForEndOfFrame();
//		chatBoxScrollBar.value = 0;
//	}
//
//	// Refresh connectedPlayers
//	public IEnumerator RefreshConnectedPlayers(){
//		
//		foreach(NetworkPlayer p in Network.connections){
//			Vector2 connectedPlayersSize = connectedPlayersRectTransform.sizeDelta;
//			connectedPlayersText.text += p.ipAddress;
//			connectedPlayersSize.y += (connectedPlayersText.lineSpacing*2 + connectedPlayersText.fontSize);
//			connectedPlayersRectTransform.sizeDelta = connectedPlayersSize;
//		}
//
//		yield return new WaitForEndOfFrame();
//		connectedPlayersScrollBar.value = 0;
//		yield return new WaitForSeconds(1);
//		StartCoroutine(RefreshConnectedPlayers());
//	}
//
//	IEnumerator ServerRunning(){
//		bool previousState = isRunning;
//		Process[] isServerRunning = System.Diagnostics.Process.GetProcessesByName("MasterServer");
//
//		if(isServerRunning.Length == 0){
//			isRunning = false;
//		}else{
//			isRunning = true;
//		}
//		if(previousState == true && isRunning == false && !Network.isServer){
//			StartCoroutine(SendMessage(info, "MasterServer is not running. You can't create a server."));
//		} 
//		if(previousState == true && isRunning == false && Network.isServer){
//			StartCoroutine(SendMessage(info, "MasterServer is not running anymore !"));
//			NetworkManager.instance.GUILeaveServer();
//		} 
//		if(previousState == false && isRunning == true) StartCoroutine(SendMessage(info, "Create a server or join an existing one."));
//		yield return new WaitForSeconds(1);
//		StartCoroutine(ServerRunning());
//	}
//
//
//	void Start () {
//		chatBoxRectTransform = chatBox.GetComponent<RectTransform>();
//		chatBoxText= chatBox.GetComponent<Text>();
//		StartCoroutine(SendMessage(info, "Your IP adress is " + Network.player.ipAddress));
//		StartCoroutine(ServerRunning());
//		if(!isRunning){
//			StartCoroutine(SendMessage(info, "MasterServer is not running. Can't create a server."));
//		}
//	}
//
//	void Update () {
//
//		ColorBlock colorBlock = chatBoxInputField.colors;
//		
//		if((Input.GetKeyDown(KeyCode.Return) && !isChatActive)  || chatBoxInputField.isFocused == true){
//			chatBoxInputField.ActivateInputField();
//			colorBlock.colorMultiplier = 2;
//			chatBoxInputField.colors = colorBlock;
//			isChatActive = true;
//		}
//		if((Input.GetKeyDown(KeyCode.Return) && isChatActive == true)  || chatBoxInputField.isFocused == false){
//			if(chatBoxInputField.text != ""){
//
//				string splitText = chatBoxInputField.text;
//				string[] splitArray = splitText.Split(' ');
//
//				if(chatBoxInputField.text == "/createServer" || chatBoxInputField.text == "/c") {NetworkManager.instance.GUIStartServer();}
//				else if(chatBoxInputField.text == "/disconnect" || chatBoxInputField.text == "/d"){NetworkManager.instance.GUILeaveServer();}
//				else if(splitArray.Length > 1){
//					string ipAdress = splitArray[1];
//					string port = splitArray[2];
//					if(chatBoxInputField.text == "/join " + ipAdress + " " + port || chatBoxInputField.text == "/j " + ipAdress + " " + port){
//						IPJoinInputField.text = ipAdress;
//						portJoinInputField.text = port;
//					}
//
//					NetworkManager.instance.GUIJoinServer();
//				}
//				else{StartCoroutine(SendMessage("PLAYER", chatBoxInputField.text));}
//
//			}
//			chatBoxInputField.text = "";
//			chatBoxInputField.DeactivateInputField();
//			colorBlock.colorMultiplier = 1;
//			chatBoxInputField.colors = colorBlock;
//			isChatActive = false;
//		}
//	}
//}
