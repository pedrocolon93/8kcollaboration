using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockToBoundaries : MonoBehaviour {
    public GameObject myTerrain; // link from inspector or obtained via GetComponent / Object.Find

    //this value is used to further clamp the camera away from the edge.
    //Transform will not travel closer than nonPassibleBorderWidth from a terrain edge
    public float nonPassibleBorderWidth = 30;

    Vector3 mapMinBounds;
    Vector3 mapMaxBounds;
    public float heightMaxMultiplier = 5;
    // Use this for initialization
    void Start () {
        //calculate Terrain bounds
        float xsize = myTerrain.GetComponentInChildren<MeshRenderer>().bounds.size.x;
        float ysize = myTerrain.GetComponentInChildren<MeshRenderer>().bounds.size.y;
        float zsize = myTerrain.GetComponentInChildren<MeshRenderer>().bounds.size.z;
        int zmultiplier = 0;
        foreach(MeshRenderer terrainMesh in myTerrain.GetComponentsInChildren<MeshRenderer>())
        {
            zmultiplier += 1;
            if (terrainMesh.bounds.size.y > ysize)
                ysize = terrainMesh.bounds.size.y;
        }
        zsize *= zmultiplier; 
        var myTerrainTransform = myTerrain.transform;
        mapMinBounds = new Vector3(myTerrainTransform.position.x- xsize/2.0f, myTerrainTransform.position.y, myTerrainTransform.position.z- zsize / 2.0f);
        mapMaxBounds += mapMinBounds + new Vector3(xsize, ysize*heightMaxMultiplier, zsize);
        
        //apply any border edging spce clamping
        mapMinBounds.x += nonPassibleBorderWidth;
        mapMinBounds.z += nonPassibleBorderWidth;
        mapMaxBounds.x -= nonPassibleBorderWidth;
        mapMaxBounds.z -= nonPassibleBorderWidth;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    Vector3 cameraTransformPosition = transform.position;
        if (cameraTransformPosition.x > mapMaxBounds.x)
        {
            cameraTransformPosition.x = mapMaxBounds.x;
        }
        if (cameraTransformPosition.z > mapMaxBounds.z)
        {
            cameraTransformPosition.z = mapMaxBounds.z;
        }
        if (cameraTransformPosition.x < mapMinBounds.x)
        {
            cameraTransformPosition.x = mapMinBounds.x;
        }
	    if (cameraTransformPosition.z < mapMinBounds.z)
	    {
	        cameraTransformPosition.z = mapMinBounds.z;
	    }
        if (cameraTransformPosition.y <= 0)
        {
            cameraTransformPosition.y = 0;
        }
	    transform.position = cameraTransformPosition;
	}
}
