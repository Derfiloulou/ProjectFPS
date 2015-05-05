using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct ShotLevel {
	
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
	public GameObject weapon;
	public TextMesh availableBulletsText;

	[Header("Shot Effect")]
	public float shake = 0.1f;
	public float shotAccuracy = 30;

	[Header("Shot Multiplier")]
	public int limbMultiplier = 1;
	public int bodyMultiplier = 2;
	public int headMultiplier = 4;


	[Header("Shot Levels")]
    public List<ShotLevel> shotLevels;


   
	[HideInInspector]
	public float shotDispersion;

	float lastShot = 0;
    LineRenderer lr;
	int shotCount = 0;
	NetworkView nView;
	Camera cam;
	GameGUIManager gameGUIManager;
	FirstPersonController fps;
	Animator shootAnim;
	Light shotLight;
	ParticleSystem shotSystem;
	ShotLevel currentShotLevel;
	int currentShotLevelInt ;
	int bulletsForThisLevel;
	Renderer weaponRenderer;


	AudioSource originAudio;
	AudioSource playerAudio;

	AudioClip hitSound;
	AudioClip shotSound;
	
	// DEBUG
	int bulletsLeft;

	// Use this for initialization
	void Start () {
		nView = GetComponent<NetworkView>();
		cam = GetComponentInChildren<Camera>();
		originAudio = shotOrigin.GetComponent<AudioSource>();
		playerAudio = GetComponent<AudioSource>();
		hitSound = GameSoundManager.instance.hit;
		shotSound = GameSoundManager.instance.shot;
		gameGUIManager = GameGUIManager.instance;
		fps = GetComponent<FirstPersonController>();
		shootAnim = GetComponentInChildren<Animator>();
		shotLight = GetComponent<Light>();
		shotSystem = shotOrigin.GetComponent<ParticleSystem>();
		weaponRenderer = weapon.GetComponent<Renderer>();
		weaponRenderer.material = new Material(Shader.Find("Diffuse"));

		shotDispersion = 0;
		currentShotLevelInt = 0;
		currentShotLevel = shotLevels[0];
		bulletsForThisLevel = shotLevels[0].availableBullets;
		weaponRenderer.material.color = shotLevels[0].colorRay;
		availableBulletsText.text = bulletsForThisLevel.ToString();

	}
	
	// Update is called once per frame
	void Update () {
		if(shotLight.range > 0){
			shotLight.range -= 5;
		}

		if(shotDispersion != 0){
			shotDispersion = 0;
		}

		// Tir
        if (nView.isMine && Input.GetKey(KeyCode.Mouse0))
        {
			// Tir dispo
            if (Time.time > lastShot + 1.0f/(float)currentShotLevel.bulletsPerSecond) 
            {
                lastShot = Time.time;

				originAudio.pitch = 0.5f + (((float)currentShotLevelInt)/(float)shotLevels.Count);
				originAudio.PlayOneShot(shotSound);
				shootAnim.SetTrigger("Shot");
				shotDispersion = shotAccuracy;

				Vector2 newPosition = Random.insideUnitCircle*fps.shootRayon;
				newPosition = new Vector2(newPosition.x/Screen.width, newPosition.y/Screen.height) + new Vector2(0.5f,0.5f);
				shotSystem.Emit(1);
				shotLight.range = 20;

				Ray ray = cam.ViewportPointToRay(newPosition);
                RaycastHit hit;

				// Touche le joueur
                if (Physics.Raycast(ray, out hit))
                {                    
					if (hit.transform.tag == "Body" || hit.transform.tag == "Limb" || hit.transform.tag == "Head")
                    {
						NetworkView nViewEnemy = hit.collider.gameObject.GetComponentInParent<NetworkView>();
						int multiplier = 1;
						int damages = 0;

						if(hit.transform.tag == "Body") multiplier = bodyMultiplier;
						if(hit.transform.tag == "Limb") multiplier = limbMultiplier;
						if(hit.transform.tag == "Head") multiplier = headMultiplier;

						damages = currentShotLevel.shotStrength * multiplier;
						nViewEnemy.RPC("SetHealth", RPCMode.All, damages);
						GameGUIManager.instance.healthText.text = "Health : " + GetComponent<State>().health.ToString();

						Debug.Log(hit.transform.tag + " / " + currentShotLevel.shotStrength + " / " + multiplier);

						playerAudio.PlayOneShot(hitSound);
						gameGUIManager.SetAlphaImage(gameGUIManager.hitmarkerImage, 1);
						StartCoroutine(gameGUIManager.FadeImage(gameGUIManager.hitmarkerImage, 0.5f, false));


                    }
                }

				nView.RPC(
					"DisplayShotEffects", 
					RPCMode.All, 
					hit.point, 
					shotOrigin.transform.position, 
					(int)currentShotLevel.colorRay.r,
					(int)currentShotLevel.colorRay.g,
					(int)currentShotLevel.colorRay.b,
					(int)currentShotLevel.colorRay.a
				);

				cam.transform.localPosition += new Vector3(Random.Range(-shake,shake), Random.Range(-shake,shake),0);
				UpdateCurrentShotLevel();
            }
        }
	}

	void UpdateCurrentShotLevel(){
		shotCount++;
		bulletsForThisLevel --;

		if(bulletsForThisLevel == 0 && currentShotLevelInt < shotLevels.Count-1){
			currentShotLevelInt ++;
			currentShotLevel = shotLevels[currentShotLevelInt];
			bulletsForThisLevel = currentShotLevel.availableBullets;
			weaponRenderer.material.color = shotLevels[currentShotLevelInt].colorRay;
		}
		if(currentShotLevelInt == shotLevels.Count-1)
			availableBulletsText.text = "∞";
		else
			availableBulletsText.text = bulletsForThisLevel.ToString();

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
		_lrShot.SetWidth(0.005f, 0.005f);
		_lrShot.SetPosition(0, _shotOrigin);
		_lrShot.SetPosition(1, _hitPoint);

		StartCoroutine(DestroyShotEffects(_lrShot, _imp));
	}

	IEnumerator DestroyShotEffects(LineRenderer _lr, GameObject _go){
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		_lr.enabled = false;
		yield return new WaitForSeconds(10f);
		Destroy(_go);
	}
}
