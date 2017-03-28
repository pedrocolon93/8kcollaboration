using System.Collections;using System.Collections.Generic;
using UnityEngine;

public class ClickWaldo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnMouseDown()
    {
        GameObject.Find("Canvas").GetComponent<CanvasManager>().EnableCanvas();
    }
}
