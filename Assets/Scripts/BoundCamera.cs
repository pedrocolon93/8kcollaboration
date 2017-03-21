using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundCamera : MonoBehaviour {
	public float maxx = 500f, maxy = 500f, maxz = 500f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 transformposition = transform.position;
		if (transform.position.x >= maxx)
			transformposition.x = maxx;
		if (transform.position.x <= -maxx)
			transformposition.x = -maxx;
		if (transform.position.y >= maxy)
			transformposition.y = maxy;
		if (transform.position.y <= -maxy)
			transformposition.y = -maxy;
		if (transform.position.z >= maxz)
			transformposition.z = maxz;
		if (transform.position.z <= -maxz)
			transformposition.z = -maxz;
		transform.position = transformposition;
	}
}
