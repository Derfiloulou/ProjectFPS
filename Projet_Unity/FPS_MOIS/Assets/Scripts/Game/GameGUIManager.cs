using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameGUIManager : MonoBehaviour {

	public Text healthText;
	public Text shotLevelText;
	public Text shotStrengthText;
	public Text bulletPerSecondText;
	public Text availableBulletsText;
	public Text shotCountText;
	public Text bulletsLeftText;
	public GameObject camera;


	static GameGUIManager mInst;
	static public GameGUIManager instance { get { return mInst; } }
	
	void Awake () {
		if(mInst == null) mInst = this;
		//DontDestroyOnLoad(this); 		
	}

	void Start(){
		camera.SetActive(false);
	}
}
