using UnityEngine;
using System.Collections;

public class Etat : MonoBehaviour {
    
	public int vie = 5000;
	NetworkViewID id;
	
	void Start () {
		GameGUIManager.instance.healthText.text = "Health : " + vie.ToString();
		id = GetComponent<NetworkView>().viewID;
    }

	[RPC]
	void KillPlayer(NetworkViewID _id){
		NetworkView[] nViewArray = GameObject.FindObjectsOfType<NetworkView>();
		for(int i=0 ; i<nViewArray.Length ; i++){
			if(nViewArray[i].viewID == _id){
				Destroy(nViewArray[i].gameObject);
			}
		}
	}

	void Update(){
		if(vie <= 0){
			GameGUIManager.instance.camera.SetActive(true);
			GetComponent<NetworkView>().RPC("KillPlayer", RPCMode.All, id);
		}
	}

	/*
	 * synchro les HP enlevés
	 * régler le problème de destroy
	 */
}
