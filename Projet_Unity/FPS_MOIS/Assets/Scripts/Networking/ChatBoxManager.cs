using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ChatBoxManager : MonoBehaviour {

	public GameObject chatBox;
	Text chatBoxText;
	RectTransform chatBoxRectTransform;
	public Scrollbar chatBoxScrollBar;
	public InputField chatBoxInputField;

	bool isChatActive = false;


	static ChatBoxManager mInst;
	static public ChatBoxManager instance { get { return mInst; } }		

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

	void Start () {
		chatBoxRectTransform = chatBox.GetComponent<RectTransform>();
		chatBoxText= chatBox.GetComponent<Text>();
		StartCoroutine(SendMessage("GAME", "Ready"));
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
					StartCoroutine(SendMessage("PLAYER", chatBoxInputField.text));
				}
				chatBoxInputField.text = "";
				chatBoxInputField.DeactivateInputField();
				colorBlock.colorMultiplier = 1;
				chatBoxInputField.colors = colorBlock;
				isChatActive = false;
			}

	}
}
