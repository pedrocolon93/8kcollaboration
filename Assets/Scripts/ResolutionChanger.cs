using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionChanger : MonoBehaviour {
    public List<LoadLidar.Tuple<int,int>> resolutionsList = new List<LoadLidar.Tuple<int, int>>(new LoadLidar.Tuple<int, int>[] { new LoadLidar.Tuple<int, int>(1920,1080), new LoadLidar.Tuple<int, int>(3840, 2160), new LoadLidar.Tuple<int, int>(7680, 4320)});
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            Screen.SetResolution(resolutionsList[0].Item1, resolutionsList[0].Item2,true);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            Screen.SetResolution(resolutionsList[1].Item1, resolutionsList[1].Item2, true);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            Screen.SetResolution(resolutionsList[2].Item1, resolutionsList[2].Item2, true);
        }
    }
}
