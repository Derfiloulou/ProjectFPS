using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;

public class GUIManager : MonoBehaviour {

	public Canvas canvas;
	public GameObject chatBox;
	Text chatBoxText;
	RectTransform chatBoxRectTransform;
	public Scrollbar chatBoxScrollBar;
	public InputField chatBoxInputField;
	public InputField portCreateInputField;
	public InputField IPJoinInputField;
	public InputField portJoinInputField;
	public GameObject hostButton;
	public string info = "INFO";
	//public List<GameObject> hostButtonList = new List<GameObject>();

	bool isChatActive = false;
	public bool isRunning = false;


	static GUIManager mInst;
	static public GUIManager instance { get { return mInst; } }		

	void Awake () {
		if(mInst == null) mInst = this;
		DontDestroyOnLoad(this); 		
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

	IEnumerator ServerRunning(){
		bool isSameState = isRunning;
		Process[] isServerRunning = System.Diagnostics.Process.GetProcessesByName("MasterServer");

		if(isServerRunning.Length == 0){
			isRunning = false;
		}else{
			isRunning = true;
		}
		if(isSameState == true && isRunning == false) StartCoroutine(SendMessage(info, "MasterServer is not running. You can't create a server."));
		if(isSameState == false && isRunning == true) StartCoroutine(SendMessage(info, "You can now create a server or join an existing one."));
		yield return new WaitForSeconds(1);
		StartCoroutine(ServerRunning());
	}

//	public void CreateButton(string buttonText, string _ip){
//		GameObject button = Instantiate(hostButton, Vector3.zero, Quaternion.identity) as GameObject;
//		button.transform.SetParent(canvas.transform);
//		RectTransform buttonTransform = button.GetComponent<RectTransform>();
//		float positionY = -hostButtonList.Count * buttonTransform.sizeDelta.y -160f;
//		buttonTransform.position = new Vector3(10, Screen.height + positionY, 0);
//		button.GetComponentInChildren<Text>().text = buttonText;
//		button.GetComponent<ButtonScript>().ip = _ip;
//		hostButtonList.Add(button);
//	}

	void Start () {
		chatBoxRectTransform = chatBox.GetComponent<RectTransform>();
		chatBoxText= chatBox.GetComponent<Text>();
		StartCoroutine(SendMessage(info, "Your IP adress is " + Network.player.ipAddress));
		StartCoroutine(ServerRunning());
		if(!isRunning){
			StartCoroutine(SendMessage(info, "MasterServer is not running. You can't create a server."));
		}
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

				string splitText = chatBoxInputField.text;
				string[] splitArray = splitText.Split(' ');

				if(chatBoxInputField.text == "/createServer" || chatBoxInputField.text == "/c") {NetworkManager.instance.GUIStartServer();}
				else if(chatBoxInputField.text == "/disconnect" || chatBoxInputField.text == "/d"){NetworkManager.instance.GUILeaveServer();}
				//else if(chatBoxInputField.text == "/refresh" || chatBoxInputField.text == "/r"){NetworkManager.instance.GUIRefreshHostList();}
				else if(splitArray.Length > 1){
					string ipAdress = splitArray[1];
					string port = splitArray[2];
					if(chatBoxInputField.text == "/join " + ipAdress + " " + port || chatBoxInputField.text == "/j " + ipAdress + " " + port){
						IPJoinInputField.text = ipAdress;
						portJoinInputField.text = port;
					}

					NetworkManager.instance.GUIJoinServer();
				}
				else{StartCoroutine(SendMessage("PLAYER", chatBoxInputField.text));}

			}
			chatBoxInputField.text = "";
			chatBoxInputField.DeactivateInputField();
			colorBlock.colorMultiplier = 1;
			chatBoxInputField.colors = colorBlock;
			isChatActive = false;
		}
	}
}
