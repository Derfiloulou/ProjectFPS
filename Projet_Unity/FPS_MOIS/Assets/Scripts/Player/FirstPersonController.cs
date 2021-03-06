﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class FirstPersonController: MonoBehaviour
{
	[Header("Game Objects")]
	public GameObject camView;
	public GameObject camWeapon;
	public GameObject mesh;

	[Header("Player Speed")]
    public float walkSpeed = 6.0f;
    public float runSpeed = 10.0f;
	public float crouchSpeed = 10;
	public float aimSpeedMultiplier = 3.0f;

	[Header("Crouch")]
	public float crouchAmount = 1;
	public float crouchTime = 10;
	
	[Header("Shot Dispersion")]
	public float shotStop = 20;
	public float shotMovingMultiplier = 20;
	public float shotAimingMultiplier = 2;
	
	[Header("Others")]
	public float jumpSpeed = 4.0f;
    public float gravity = 10.0f;
    public bool slideWhenOverSlopeLimit = false;
    public bool slideOnTaggedObjects = false;
    public float slideSpeed = 5.0f;
    public bool airControl = true;
    public float antiBumpFactor = .75f;
    public int antiBunnyHopFactor = 1;
 
	[HideInInspector]
	public float shootRayonCurrent;
	[HideInInspector]
	public float shootRayon;

    CharacterController controller;
	Shooting shooting;
	int jumpTimer;
	RaycastHit hit;
	NetworkView nView;
   
	Vector3 moveDirection = Vector3.zero;
	Vector3 contactPoint;
	Vector3 syncStartPosition = Vector3.zero;
	Vector3 syncEndPosition = Vector3.zero;
	Vector3 cameraOrigin;
	Vector3 cameraCrouch;
	
	Transform playerTransform;
	Transform cameraTransform;

	float fallingDamageThreshold = 10.0f;
    float speed;
    float fallStartLevel;
    float slideLimit;
    float rayDistance;
	float lastSynchronizationTime = 0f;
	float syncDelay = 0f;
	float syncTime = 0f;
	float aimSpeedMultiplierCurrent;

	Quaternion syncEndRotation= Quaternion.identity;
	Quaternion syncStartRotation= Quaternion.identity;

	bool limitDiagonalSpeed = true;
	bool grounded = false;
	bool falling;
	bool playerControl = false;


 
    void Start()
    {

		nView = GetComponent<NetworkView> ();
		playerTransform = GetComponent<Transform> ();
		controller = GetComponent<CharacterController>();
		shooting = GetComponent<Shooting>();

		if(nView.isMine){
			AudioListener audioListener = camView.AddComponent<AudioListener>();
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			GetComponentInChildren<Camera>().enabled = true;
			cameraTransform = GetComponentInChildren<Camera>().transform;
			shootRayonCurrent = shotStop;
			speed = walkSpeed;
			rayDistance = controller.height * .5f + controller.radius;
			slideLimit = controller.slopeLimit - .1f;
			jumpTimer = antiBunnyHopFactor;
			Destroy(mesh);
			cameraOrigin = cameraTransform.localPosition;
			cameraCrouch = cameraOrigin + new Vector3(0,-crouchAmount,0);
			aimSpeedMultiplierCurrent = aimSpeedMultiplier;
		}else{
			GetComponentInChildren<Camera>().enabled = false;
			camWeapon.SetActive(false);
			Destroy(GetComponent<CapsuleCollider>());
		}
    }
	

	void MovePlayer(){
		float inputX = Input.GetAxis("Horizontal");
		float inputY = Input.GetAxis("Vertical");
		// If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
		float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed)? .7071f : 1.0f;

		if(!Input.GetButton("Aim"))
			shootRayonCurrent = shotStop + shooting.shotDispersion + Mathf.Abs (controller.velocity.magnitude)*shotMovingMultiplier;
		else{
			shootRayonCurrent = shooting.shotDispersion + Mathf.Abs (controller.velocity.magnitude)*shotAimingMultiplier;
		}

		if (grounded) {
			bool sliding = false;
			// See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
			// because that interferes with step climbing amongst other annoyances
			if (Physics.Raycast(playerTransform.position, -Vector3.up, out hit, rayDistance)) {
				if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
					sliding = true;
			}
			// However, just raycasting straight down from the center can fail when on steep slopes
			// So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
			else {
				Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
				if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
					sliding = true;
			}
			
			if (falling) {
				falling = false;
				if (playerTransform.position.y < fallStartLevel - fallingDamageThreshold)
					FallingDamageAlert (fallStartLevel - playerTransform.position.y);
			}

			if(Input.GetButton("Aim")){
				aimSpeedMultiplierCurrent = aimSpeedMultiplier;
			}else{
				aimSpeedMultiplierCurrent = 1;
			}

		 	if(Input.GetButton("Run")){

				speed = runSpeed * aimSpeedMultiplierCurrent;

			}else if(Input.GetButton("Crouch")){
				cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, cameraCrouch, crouchTime * Time.deltaTime);
				speed = crouchSpeed* aimSpeedMultiplierCurrent;
			}else{
				speed = walkSpeed * aimSpeedMultiplierCurrent;
				cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, cameraOrigin, crouchTime * Time.deltaTime);
			}

			
			// If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
			if ( (sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide") ) {
				Vector3 hitNormal = hit.normal;
				moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
				Vector3.OrthoNormalize (ref hitNormal, ref moveDirection);
				moveDirection *= slideSpeed;
				playerControl = false;
			}
			// Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
			else {
				moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
				moveDirection = playerTransform.TransformDirection(moveDirection) * speed;
				playerControl = true;
			}
			
			// Jump! But only if the jump button has been released and player has been grounded for a given number of frames
			if (!Input.GetButton("Jump"))
				jumpTimer++;
			else if (jumpTimer >= antiBunnyHopFactor) {
				moveDirection.y = jumpSpeed;
				jumpTimer = 0;
			}
		}
		else {
			// If we stepped over a cliff or something, set the height at which we started falling
			if (!falling) {
				falling = true;
				fallStartLevel = playerTransform.position.y;
			}
			// If air control is allowed, check movement but don't touch the y component
			if (airControl && playerControl) {
				moveDirection.x = inputX * speed * inputModifyFactor;
				moveDirection.z = inputY * speed * inputModifyFactor;
				moveDirection = playerTransform.TransformDirection(moveDirection);
			}
		}
		
		// Apply gravity
		moveDirection.y -= gravity * Time.deltaTime;
		
		// Move the controller, and set grounded true or false depending on whether we're standing on something
		grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
	}

	private void SyncedMovement()
	{
		syncTime += Time.deltaTime;
		playerTransform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
		playerTransform.rotation = Quaternion.Lerp(syncStartRotation, syncEndRotation, syncTime / syncDelay);
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		Quaternion syncRotation = Quaternion.identity;
		
		
		if (stream.isWriting)
		{
			if(playerTransform != null){
				syncPosition = playerTransform.position;
				stream.Serialize(ref syncPosition);
				
				syncVelocity = controller.velocity;
				stream.Serialize(ref syncVelocity);
				
				syncRotation = playerTransform.rotation;
				stream.Serialize(ref syncRotation);
			}
		}
		else
		{
			if(playerTransform != null){
				stream.Serialize(ref syncPosition);
				stream.Serialize(ref syncVelocity);
				stream.Serialize(ref syncRotation);
				
				syncTime = 0f;
				syncDelay = Time.time - lastSynchronizationTime;
				lastSynchronizationTime = Time.time;
				
				syncEndPosition = syncPosition + syncVelocity * syncDelay;
				syncEndRotation = syncRotation;
				syncStartPosition = playerTransform.position;
				syncStartRotation = playerTransform.rotation;
			}
		}
	}
 

	[RPC]
	void Restart(){
		Application.LoadLevel("Networking_In_Game");
	}
	

    void Update() {

		
		if (nView.isMine)
		{
			MovePlayer();
			GameGUIManager.instance.LerpAim(shootRayon);
			shootRayon = Mathf.Lerp(shootRayon, shootRayonCurrent, Time.deltaTime*10);

		}
		else
		{
			SyncedMovement();
		}

		if(Network.isServer && Input.GetKeyDown(KeyCode.Return)){
			nView.RPC("Restart", RPCMode.All);
		}


    }
 
    // Store point that we're in contact with for use in FixedUpdate if needed
    void OnControllerColliderHit (ControllerColliderHit hit) {
        contactPoint = hit.point;
    }
 
    // If falling damage occured, this is the place to do something about it. You can make the player
    // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
    void FallingDamageAlert (float fallDistance)
    {
        //print ("Ouch! Fell " + fallDistance + " units!");   
    }
}