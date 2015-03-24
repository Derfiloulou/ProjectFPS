using UnityEngine;
using System.Collections;

public class Networking : MonoBehaviour {


	public void CreateServer(){
		Network.InitializeServer (2, 25000, false);
	}

	public void JoinServer(){
		Network.Connect("10.51.0.218", 25000);
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
