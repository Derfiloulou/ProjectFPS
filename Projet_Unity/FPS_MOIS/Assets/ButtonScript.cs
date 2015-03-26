using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonScript : MonoBehaviour {

	public int id;

	void Awake () {
		GetComponent<Button>().onClick.AddListener(() => {
			NetworkManager.instance.GUIJoinServer(id);
		});
	}
}
