using UnityEngine;
using System.Collections;


[RequireComponent (typeof (Camera))]
public class CameraControl : MonoBehaviour
{
	public float zoomFOV = 30.0f;
	public float aimSpeed = 9f;
	public GameObject weapon;
	Vector3 weaponPosition;
	Quaternion weaponRotation;
	public Vector3 weaponPositionOnShoot;
	public Vector3 weaponRotationOnShoot;
	Quaternion weaponRotationOnShootQ;
	public Camera cameraView;
	public Camera cameraWeapon;
	MouseLook cameraViewMouseLook;
	MouseLook cameraWeaponMouseLook;
	
	private float targetFOV;
	private float baseFOV;
	Transform weaponTransform;


	
	void Start ()
	{
		weaponTransform = weapon.GetComponent<Transform>();
		weaponPosition = weaponTransform.localPosition;
		weaponRotation = weaponTransform.localRotation;
		weaponRotationOnShootQ = Quaternion.Euler(weaponRotationOnShoot);
		cameraViewMouseLook = cameraView.GetComponent<MouseLook>();
		cameraWeaponMouseLook = cameraWeapon.GetComponent<MouseLook>();
		SetBaseFOV(cameraView.fieldOfView);
	}
	
	void Update ()
	{
		if(Input.GetButton("Aim") )
		{
			targetFOV = zoomFOV;
			weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, weaponPositionOnShoot, Time.deltaTime*aimSpeed);
			weaponTransform.localRotation = Quaternion.Lerp(weaponTransform.localRotation, weaponRotationOnShootQ, Time.deltaTime*aimSpeed);

		}
		else
		{
			targetFOV = baseFOV;
			weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, weaponPosition, Time.deltaTime*aimSpeed);
			weaponTransform.localRotation = Quaternion.Lerp(weaponTransform.localRotation, weaponRotation, Time.deltaTime*aimSpeed);

		}

	
		UpdateZoom();
	}
	
	private void UpdateZoom()
	{
		cameraView.fieldOfView = Mathf.Lerp(cameraView.fieldOfView, targetFOV, aimSpeed * Time.deltaTime);
		cameraWeapon.fieldOfView = Mathf.Lerp(cameraWeapon.fieldOfView, targetFOV, aimSpeed * Time.deltaTime);
	}
	
	public void SetBaseFOV(float fov)
	{
		baseFOV = fov;
	}
}
