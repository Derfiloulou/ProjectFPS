using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour {

    public GameObject impact;
    public int vie = 5000;

    private int shotcount = 0;
    public int shot1str = 5000;
    public int shot2str = 2500;
    public int shot3str = 1667;
    public int shot4str = 1250;
    public int shot5str = 1000;
    public int shot6tr = 834;
    public int shot7str = 715;
    public int shot8str = 625;

    public int av_bullet1 = 1;
    public int av_bullet2 = 2;
    public int av_bullet3 = 6;
    public int av_bullet4 = 12;
    public int av_bullet5 = 30;
    public int av_bullet6 = 100;
    public int av_bullet7 = 300;
    public int av_bullet8 = 100000;

    public int BPS1 = 1;
    public int BPS2 = 1;
    public int BPS3 = 2;
    public int BPS4 = 2;
    public int BPS5 = 3;
    public int BPS6 = 5;
    public int BPS7 = 6;
    public int BPS8 = 100;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Ray ray = GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

           RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) 
            {
                if (impact!=null)
                {
                    GameObject imp = GameObject.Instantiate<GameObject>(impact);
                    imp.transform.position = hit.point;
                }
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Joueur"))
                {
                    Debug.Log("pan");
                    hit.transform.gameObject.GetComponent<Shooting>.vie = (hit.transform.gameObject.GetComponent<Shooting>.vie -
                }
            }

        }

	}
}
