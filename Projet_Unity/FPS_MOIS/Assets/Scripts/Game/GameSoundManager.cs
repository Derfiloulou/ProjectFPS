using UnityEngine;
using System.Collections;

public class GameSoundManager : MonoBehaviour {

	public AudioClip hit;
	public AudioClip shot;

	static GameSoundManager mInst;
	static public GameSoundManager instance { get { return mInst; } }
	
	void Awake () {
		if(mInst == null) mInst = this;
	}
	
}
