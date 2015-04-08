using UnityEngine;
using System.Collections;

public class Etat : MonoBehaviour {
    public int vie = 5000;

	// Use this for initialization
	void Start () {
        vie = 5000;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.Y))
        {
            Debug.Log("Niveau HP :" + vie);
        }
	}
}
