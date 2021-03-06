﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameGUIManager : MonoBehaviour {

	public Image[] aim;
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

	public IEnumerator FadeImage(Image _target, float _time, bool _increase){

		Color newColor = _target.color;

		if (_increase) {
			while(newColor.a < 1){
				newColor.a += Time.deltaTime/_time;
				_target.color = newColor;
				yield return new WaitForEndOfFrame();
			}
		} else {
			while(newColor.a > 0){
				newColor.a -= Time.deltaTime/_time;
				_target.color = newColor;
				yield return new WaitForEndOfFrame();
			}
		}
	}

	public void LerpAim(float _amount){
		_amount += 7;
		aim[0].rectTransform.position = Vector3.Lerp(aim[0].rectTransform.position, new Vector3(Screen.width/2,_amount + Screen.height/2,0), 10*Time.deltaTime);
		aim[1].rectTransform.position = Vector3.Lerp(aim[1].rectTransform.position, new Vector3(_amount + Screen.width/2 , Screen.height/2,0), 10*Time.deltaTime);
		aim[2].rectTransform.position = Vector3.Lerp(aim[2].rectTransform.position, new Vector3(Screen.width/2,-_amount + Screen.height/2,0), 10*Time.deltaTime);
		aim[3].rectTransform.position = Vector3.Lerp(aim[3].rectTransform.position, new Vector3(-_amount + Screen.width/2 , Screen.height/2,0), 10*Time.deltaTime);

	}

	void Awake () {
		if(mInst == null) mInst = this;
		//DontDestroyOnLoad(this); 		
	}

	void Start(){
		cam.SetActive(false);
	}

	void Update(){
		if(Input.GetButtonDown ("Aim")){
			foreach(Image i in aim){
				SetAlphaImage(i, 1);
				StartCoroutine(FadeImage(i, 0.2f, false));
			}
		}
		
		if(Input.GetButtonUp ("Aim")){
			foreach(Image i in aim){
				SetAlphaImage(i, 0);
				StartCoroutine(FadeImage(i, 0.2f, true));
			}
		}

	}
}
