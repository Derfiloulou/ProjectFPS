using UnityEngine;
using System.Collections;

public class CameraReset : MonoBehaviour {


	Transform camT;
	Vector3 camP;

	void Start () {
		camT = GetComponent<Camera>().transform;
		camP = camT.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(camT.localPosition != camP){
		camT.localPosition = Vector3.Lerp(camT.localPosition, camP, Time.deltaTime*5);
		}
	}
}
