using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct ShotLevel {

	[HideInInspector]
	public int level;
    public int shotStrength;
    public int availableBullets;
    public int bulletsPerSecond;
	public Color colorRay;
}

[System.Serializable]
public class Shooting : MonoBehaviour {

	[Header("Game Objects")]
    public GameObject impact;
	public GameObject shotOrigin;

	[Header("Shot Levels")]
    public List<ShotLevel> shotLevels;

	[Header("Shot Effect")]
	public float shake = 0.1f;
	public float shotAccuracy = 100;
   
	[HideInInspector]
	public float shotDispersion;

	float lastShot = 0;
    LineRenderer lr;
	int shotCount = 0;
	NetworkView nView;
	Camera camera;
	bool isLevelMax =false;
	GameGUIManager gameGUIManager;
	FirstPersonController fps;
	Animator shootAnim;
	Light shotLight;
	ParticleSystem shotSystem;

	AudioSource originAudio;
	AudioSource playerAudio;

	AudioClip hitSound;
	AudioClip shotSound;
	
	// DEBUG
	int bulletsLeft;

	// Use this for initialization
	void Start () {
		nView = GetComponent<NetworkView>();
		camera = GetComponentInChildren<Camera>();
		originAudio = shotOrigin.GetComponent<AudioSource>();
		playerAudio = GetComponent<AudioSource>();
		hitSound = GameSoundManager.instance.hit;
		shotSound = GameSoundManager.instance.shot;
		gameGUIManager = GameGUIManager.instance;
		fps = GetComponent<FirstPersonController>();
		shootAnim = GetComponentInChildren<Animator>();
		shotLight = GetComponent<Light>();
		shotSystem = shotOrigin.GetComponent<ParticleSystem>();
		shotDispersion = 0;

		// DEBUG
		GameGUIManager.instance.shotLevelText.text = "Shot level : " + (shotLevels[0].level+1).ToString();
		GameGUIManager.instance.availableBulletsText.text = "Available bullets : " + shotLevels[0].availableBullets.ToString();
		GameGUIManager.instance.shotStrengthText.text = "Shot strength : " + shotLevels[0].shotStrength.ToString();
		GameGUIManager.instance.bulletPerSecondText.text = "Bullet per second : " + shotLevels[0].bulletsPerSecond.ToString();
		GameGUIManager.instance.shotCountText.text = "Shot count : " + shotCount.ToString();
		GameGUIManager.instance.bulletsLeftText.text = "Bullets Left for this level : " + shotLevels[0].availableBullets.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		if(shotLight.range > 0){
			shotLight.range -= 5;
		}

		if(shotDispersion != 0){
			shotDispersion = 0;
		}

        if (nView.isMine && Input.GetKey(KeyCode.Mouse0))
        {
            if (Time.time > lastShot + 1.0f/(float)GetCurrentShotLevel().bulletsPerSecond) 
            {
                lastShot = Time.time;
				shotCount++;
				originAudio.PlayOneShot(shotSound);
				shootAnim.SetTrigger("Shot");
				shotDispersion = shotAccuracy;

				Vector2 newPosition = Random.insideUnitCircle*fps.shootRayon;
				newPosition = new Vector2(newPosition.x/Screen.width, newPosition.y/Screen.height) + new Vector2(0.5f,0.5f);
				shotSystem.Emit(1);
				shotLight.range = 20;

				Ray ray = camera.ViewportPointToRay(newPosition);
                RaycastHit hit;


                if (Physics.Raycast(ray, out hit))
                {                    
                    if (hit.transform.tag == "Player")
                    {
						NetworkView nViewEnemy = hit.collider.gameObject.GetComponent<NetworkView>();
						int damages = GetCurrentShotLevel().shotStrength;

						playerAudio.PlayOneShot(hitSound);
						gameGUIManager.SetAlphaImage(gameGUIManager.hitmarkerImage, 1);
						StartCoroutine(gameGUIManager.FadeImage(gameGUIManager.hitmarkerImage, 0.5f));

						nViewEnemy.RPC("SetHealth", RPCMode.All, damages);
						//dafuq
						GameGUIManager.instance.healthText.text = "Health : " + GetComponent<State>().health.ToString();
                    }

					ShotLevel shotLevel = GetCurrentShotLevel();
					
					// DEBUG
					if(bulletsLeft == 0 || GameGUIManager.instance.shotLevelText.text == "Shot level : max.") {
						if(shotLevel.level+1 < shotLevels.Count){
							// DEBUG
							GameGUIManager.instance.shotLevelText.text = "Shot level : " + (shotLevel.level+2).ToString();
							GameGUIManager.instance.availableBulletsText.text = "Available bullets : " + shotLevels[shotLevel.level+1].availableBullets.ToString();
							GameGUIManager.instance.shotStrengthText.text = "Shot strength : " + shotLevels[shotLevel.level+1].shotStrength.ToString();
							GameGUIManager.instance.bulletPerSecondText.text = "Bullet per second : " + shotLevels[shotLevel.level+1].bulletsPerSecond.ToString();
							GameGUIManager.instance.shotCountText.text = "Shot count : " + shotCount.ToString();
							GameGUIManager.instance.bulletsLeftText.text = "Bullets Left for this level : " + shotLevels[shotLevel.level+1].availableBullets.ToString();
						}
						if(shotLevel.level+2 >= shotLevels.Count && bulletsLeft == 0){
							isLevelMax = true;
						}
						
					}
					else {
						// DEBUG
						GameGUIManager.instance.shotLevelText.text = "Shot level : " + (shotLevel.level+1).ToString();
						GameGUIManager.instance.availableBulletsText.text = "Available bullets : " + shotLevel.availableBullets.ToString();
						GameGUIManager.instance.shotStrengthText.text = "Shot strength : " + shotLevel.shotStrength.ToString();
						GameGUIManager.instance.bulletPerSecondText.text = "Bullet per second : " + shotLevel.bulletsPerSecond.ToString();
						GameGUIManager.instance.shotCountText.text = "Shot count : " + shotCount.ToString();
						GameGUIManager.instance.bulletsLeftText.text = "Bullets Left for this level : " + bulletsLeft.ToString();
					}
					
					if(isLevelMax){
						GameGUIManager.instance.shotLevelText.text = "Shot level : max." ;
						GameGUIManager.instance.availableBulletsText.text = "Available bullets : inf.";
						GameGUIManager.instance.shotStrengthText.text = "Shot strength : " + shotLevel.shotStrength.ToString();
						GameGUIManager.instance.bulletPerSecondText.text = "Bullet per second : " + shotLevel.bulletsPerSecond.ToString();
						GameGUIManager.instance.shotCountText.text = "Shot count : " + shotCount.ToString();
						GameGUIManager.instance.bulletsLeftText.text = "Bullets Left for this level : inf.";
					}


                }
				nView.RPC(
					"DisplayShotEffects", 
					RPCMode.All, 
					hit.point, 
					shotOrigin.transform.position, 
					(int)GetCurrentShotLevel().colorRay.r,
					(int)GetCurrentShotLevel().colorRay.g,
					(int)GetCurrentShotLevel().colorRay.b,
					(int)GetCurrentShotLevel().colorRay.a
				);

				camera.transform.localPosition += new Vector3(Random.Range(-shake,shake), Random.Range(-shake,shake),0);
            }
        }
	}


    ShotLevel GetCurrentShotLevel()
    {
        int shots = 0;
        
		for (int i = 0; i < shotLevels.Count; i++)
        {
            ShotLevel sl = shotLevels[i];

			if (shotCount <= shots + sl.availableBullets)
            {
				sl.level = i;
				bulletsLeft = shots + sl.availableBullets - shotCount;
                return sl;
            }
            shots += sl.availableBullets;
        }

        return shotLevels[shotLevels.Count - 1];

    }

	[RPC]
	void SetHealth(int _damages){
		GetComponent<State>().health -= _damages;
	}



	[RPC]
	void DisplayShotEffects(Vector3 _hitPoint, Vector3 _shotOrigin, int _colorRayR, int _colorRayG, int _colorRayB, int _colorRayA){

		// Impact
		GameObject _imp = Instantiate(impact, _hitPoint, Quaternion.identity) as GameObject;
		LineRenderer _lrShot = _imp.AddComponent<LineRenderer>();
		Color _colorRay = new Color(_colorRayR, _colorRayG, _colorRayB, _colorRayA);

		// Line Renderer tir
		_lrShot.enabled = true;
		_lrShot.material = new Material(Shader.Find("Unlit/Color"));
		_lrShot.material.color = _colorRay;
		_lrShot.SetWidth(0.02f, 0.02f);
		_lrShot.SetPosition(0, _shotOrigin);
		_lrShot.SetPosition(1, _hitPoint);

		StartCoroutine(DestroyShotEffects(_lrShot, _imp));
	}

	IEnumerator DestroyShotEffects(LineRenderer _lr, GameObject _go){
		yield return new WaitForEndOfFrame();
		_lr.enabled = false;
		yield return new WaitForSeconds(10f);
		Destroy(_go);
	}
}
