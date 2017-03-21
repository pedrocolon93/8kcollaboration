using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatasetObjectController : MonoBehaviour {
    public string status = "None";
    private bool selected = false;
    public List<List<Vector4>> JoinedObjectsList = new List<List<Vector4>>();
    // Use this for initialization
    void Start () {
		
	}

    public void setSelect(bool selected)
    {
        this.selected = selected;
    }
	
	// Update is called once per frame
	void Update () {
	    if (selected)
	    {
                gameObject.GetComponent<BoundBoxes_BoundBox>().lineColor = Color.yellow;
        }
	    else
	    {
            if (status.Equals("None"))
            {
                gameObject.GetComponent<BoundBoxes_BoundBox>().lineColor = Color.red;
            }
            else if (status.Equals("Moving"))
            {
                gameObject.GetComponent<BoundBoxes_BoundBox>().lineColor = Color.green;
            }
        }
	}

    public bool isSelected()
    {
        return selected;
    }
}
