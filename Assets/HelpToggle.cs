using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpToggle : MonoBehaviour
{
    private bool light = false, starttimer = false;
    public float targetTime = 2.0f, defaulttime = 2.0f;
    public Material suitMaterial;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.H))
	    {
	        starttimer = true;
	    }
	    if (starttimer == true)
	    {
            targetTime -= Time.deltaTime;

	        if (targetTime <= 0.0f)
	        {
	            //Timer ended
	            starttimer = false;
	            light = false;
	            targetTime = defaulttime;
	        }
	        else
	        {
                light = true;
	        }
        }
        if (light == false) //if the light is on...
        {
           
            suitMaterial.DisableKeyword("_EMISSION"); // Dissable emission of object
        }
        else
        {
            suitMaterial.EnableKeyword("_EMISSION");//Turns the emission of object back on
        }
    }
}
