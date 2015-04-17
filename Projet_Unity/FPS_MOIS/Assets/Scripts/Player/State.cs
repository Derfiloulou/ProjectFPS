using UnityEngine;
using System.Collections;

public class State : MonoBehaviour {
    
	public int health = 5000;
	NetworkView nView;
	
	void Start () {
		nView = GetComponent<NetworkView>();
		GameGUIManager.instance.healthText.text = "Health : " + health.ToString();
    }

	[RPC]
	void KillPlayer(NetworkViewID _nViewID){
		NetworkView[] nViewArray = (NetworkView[])FindObjectsOfType(typeof(NetworkView));
		for(int i=0 ; i<nViewArray.Length ; i++){
			if(nViewArray[i].viewID == _nViewID)
				Destroy(nViewArray[i].gameObject);
		}

	}

	void Update(){
		if(nView.isMine && health <= 0){
			GameGUIManager.instance.cam.SetActive(true);
			nView.RPC("KillPlayer", RPCMode.All, nView.viewID);
		}
	}
	/*
	 * synchro les HP enlevés
	 * régler le problème de destroy
	 */
}
