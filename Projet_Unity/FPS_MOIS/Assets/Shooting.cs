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
    private Color c1 = Color.red;
    private Color c2 = Color.blue;
    private LineRenderer lr;
    private Vector3 origin;

	// Use this for initialization
	void Start () {
        lr = gameObject.AddComponent<LineRenderer>() as LineRenderer;
        lr.material = new Material(Shader.Find("Particles/Additive"));
        lr.SetColors(c1, c2);
        lr.SetWidth(0.2F, 0.2F);
        lr.SetVertexCount(2);

	
	}
	
	// Update is called once per frame
	void Update () {
	
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Time.time > lastShot + 1.0f/(float)GetCurrentShotLevel().bulletsPerSecond) 
            {
                lastShot = Time.time;
                shotcount++;
                Ray ray = GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    origin = transform.position;
                    lr.SetPosition(0, origin);

                    if (impact != null)
                    {
                        GameObject imp = GameObject.Instantiate<GameObject>(impact);
                        imp.transform.position = hit.point;
                        lr.SetPosition(1, hit.point);
                    
                    }
                    if (hit.transform.tag == "Joueur")
                    {
                        Debug.Log("Touché !");
                        hit.transform.gameObject.GetComponent<Etat>().vie -= GetCurrentShotLevel().shotStrength;
                    }

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        lr.SetPosition(0,origin);
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
