using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct ShotLevel {

    public int shotStrength;
    public int availableBullets;
    public int bulletsPerSecond;


}

[System.Serializable]
public class Shooting : MonoBehaviour {

    public GameObject impact;
    private int shotcount = 0;
    public List<ShotLevel> shotLevels;
    private float lastShot = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Time.time > lastShot + 1.0f/(float)GetCurrentShotLevel().bulletsPerSecond) 
            {
                Debug.Log(GetCurrentShotLevel().bulletsPerSecond);
                lastShot = Time.time;
                shotcount++;
                Ray ray = GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                { 
                    if (impact != null)
                    {
                        GameObject imp = GameObject.Instantiate<GameObject>(impact);
                        imp.transform.position = hit.point;
                    }
                    if (hit.transform.tag == "Joueur")
                    {
                        Debug.Log("Touché !");
                        hit.transform.gameObject.GetComponent<Etat>().vie -= GetCurrentShotLevel().shotStrength;
                    }
                }
            }
        }
	}


    ShotLevel GetCurrentShotLevel()
    {
        int shots = 0;
        for (int i = 0; i < shotLevels.Count; i++)
        {
            ShotLevel sl = shotLevels[i];
            if (shotcount <= shots + sl.availableBullets)
            {
                return sl;
            }

            shots += sl.availableBullets;
        }

        return shotLevels[shotLevels.Count - 1];

    }
}
