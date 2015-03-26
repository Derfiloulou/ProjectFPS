using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GUIManager : MonoBehaviour {

	public Canvas canvas;
	public GameObject chatBox;
	Text chatBoxText;
	RectTransform chatBoxRectTransform;
	public Scrollbar chatBoxScrollBar;
	public InputField chatBoxInputField;
	public GameObject hostButton;
	public List<GameObject> hostButtonList = new List<GameObject>();

	bool isChatActive = false;


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

	public void CreateButton(string buttonText, int _id){
		GameObject button = Instantiate(hostButton, Vector3.zero, Quaternion.identity) as GameObject;
		button.transform.SetParent(canvas.transform);
		RectTransform buttonTransform = button.GetComponent<RectTransform>();
		float positionY = -hostButtonList.Count * buttonTransform.sizeDelta.y -160f;
		buttonTransform.position = new Vector3(10, Screen.height + positionY, 0);
		button.GetComponentInChildren<Text>().text = buttonText;
		button.GetComponent<ButtonScript>().id = _id;
		hostButtonList.Add(button);
	}

	void Start () {
		chatBoxRectTransform = chatBox.GetComponent<RectTransform>();
		chatBoxText= chatBox.GetComponent<Text>();
		StartCoroutine(SendMessage("INFO", "Ready"));
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
