using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enable : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void enable()
    {
        GetComponent<Canvas>().enabled = true;
    }
    public void disable()
    {
        GetComponent<Canvas>().enabled = false;
    }
    public Vector3 easyScale = new Vector3(0.8f, 0.8f, 0.8f), mediumScale = new Vector3(0.5f, 0.5f, 0.5f), hardScale = new Vector3(0.3f, 0.3f, 0.3f);
    public void easyMode()
    {
        GameObject waldo = GameObject.FindGameObjectWithTag("Waldo");
        waldo.transform.localScale = easyScale;
        waldo.GetComponentInParent<SpawnWaldo>().SpawnWaldoRandom();
        disable();
    }
    public void mediumMode()
    {
        GameObject waldo = GameObject.FindGameObjectWithTag("Waldo");
        waldo.transform.localScale = mediumScale;
        waldo.GetComponentInParent<SpawnWaldo>().SpawnWaldoRandom();
        disable();
    }
    public void hardMode()
    {
        GameObject waldo = GameObject.FindGameObjectWithTag("Waldo");
        waldo.transform.localScale = hardScale;
        waldo.GetComponentInParent<SpawnWaldo>().SpawnWaldoRandom();
        disable();
    }
    public void quit()
    {
        Application.Quit();
    }
}
