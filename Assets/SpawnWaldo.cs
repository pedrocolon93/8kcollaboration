using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWaldo : MonoBehaviour
{
    public GameObject WaldoRoot;
    public GameObject TerrainGameObject;

	// Use this for initialization
	void Start ()
	{
	    SpawnWaldoRandom();
	}

    public void SpawnWaldoRandom()
    {
        float randx = Random.Range(TerrainGameObject.GetComponentInChildren<MeshRenderer>().bounds.min.x,
            TerrainGameObject.GetComponentInChildren<MeshRenderer>().bounds.max.x);
        float minz=0, maxz=0;
        foreach (MeshRenderer renderer in TerrainGameObject.GetComponentsInChildren<MeshRenderer>())
        {
            if (renderer.bounds.min.z < minz)
            {
                minz = renderer.bounds.min.z;
            }
            if (renderer.bounds.max.z > maxz)
            {
                maxz = renderer.bounds.max.z;
            }
        }
        float randz = Random.Range(minz,maxz);
        float randheight = 0;

        RaycastHit hit;
        float maxHeight = 10000;
        Ray ray = new Ray(new Vector3(randx, maxHeight, randz), Vector3.down);
        foreach (MeshCollider componentsInChild in TerrainGameObject.GetComponentsInChildren<MeshCollider>())
        {
            if (componentsInChild.gameObject.GetComponent<MeshRenderer>().bounds.Contains(new Vector3(randx, 1, randz)))
            {
                if (componentsInChild.Raycast(ray, out hit, 2.0f * maxHeight))
                {
                    Debug.Log("Hit point: " + hit.point);
                    randheight = hit.point.y;
                }
            }
        }
        GameObject.Find("Waldo").GetComponent<Transform>().position = new Vector3(randx,randheight,randz);
        
    }

    // Update is called once per frame
	void Update () {
		
	}
}
