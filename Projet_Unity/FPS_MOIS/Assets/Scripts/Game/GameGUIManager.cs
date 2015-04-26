using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameGUIManager : MonoBehaviour {

	public Image aimLeft;
	public Image aimRight;
	public Image aimUp;
	public Image aimDown;
	public Image hitmarkerImage;
	public Text healthText;
	public Text shotLevelText;
	public Text shotStrengthText;
	public Text bulletPerSecondText;
	public Text availableBulletsText;
	public Text shotCountText;
	public Text bulletsLeftText;
	public GameObject cam;



	static GameGUIManager mInst;
	static public GameGUIManager instance { get { return mInst; } }

	public void SetAlphaImage(Image _target, float _alpha){
		Color newColor = _target.color;
		newColor.a = _alpha;
		_target.color = newColor;
	}

	public IEnumerator FadeImage(Image _target, float _time){
		Color newColor = _target.color;
		while(newColor.a > 0){
			newColor.a -= Time.deltaTime/_time;
			_target.color = newColor;
			yield return new WaitForEndOfFrame();
		}
	}

	public void LerpAim(float _amount){
		_amount += 7;
		aimUp.rectTransform.position = Vector3.Lerp(aimUp.rectTransform.position, new Vector3(Screen.width/2,_amount + Screen.height/2,0), 10*Time.deltaTime);
		aimDown.rectTransform.position = Vector3.Lerp(aimDown.rectTransform.position, new Vector3(Screen.width/2,-_amount + Screen.height/2,0), 10*Time.deltaTime);
		aimLeft.rectTransform.position = Vector3.Lerp(aimLeft.rectTransform.position, new Vector3(-_amount + Screen.width/2 , Screen.height/2,0), 10*Time.deltaTime);
		aimRight.rectTransform.position = Vector3.Lerp(aimRight.rectTransform.position, new Vector3(_amount + Screen.width/2 , Screen.height/2,0), 10*Time.deltaTime);
	}

	void Awake () {
		if(mInst == null) mInst = this;
		//DontDestroyOnLoad(this); 		
	}

	void Start(){
		cam.SetActive(false);
	}
}
