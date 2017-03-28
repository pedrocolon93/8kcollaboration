using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DisableCanvas()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
    }

    public void EnableCanvas()
    {
        gameObject.GetComponent<Canvas>().enabled = true;
    }
}
