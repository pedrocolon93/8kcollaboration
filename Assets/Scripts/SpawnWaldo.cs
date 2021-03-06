﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWaldo : MonoBehaviour
{
    public GameObject WaldoRoot;
    public GameObject TerrainGameObject;
    public GameObject CityGameObject;

	// Use this for initialization
	void Start ()
	{
        AddCityMeshColliders();
	    SpawnWaldoRandom();
	}

    public void AddCityMeshColliders()
    {
        foreach (MeshRenderer mesh in CityGameObject.GetComponentsInChildren<MeshRenderer>())
        {
            mesh.gameObject.AddComponent<MeshCollider>();
        }
    }
    public int maxAttempts = 1000000;
    public void SpawnWaldoRandom()
    {
        bool findingLocation = true;
        var counter = 0;
        while (findingLocation)
        {
            if (counter >= maxAttempts)
            {
                findingLocation = false;
                GameObject.Find("Waldo").GetComponent<Transform>().position = new Vector3(0, 0, 0);
                break;
            }
            else
                counter++;

            float randx = UnityEngine.Random.Range(TerrainGameObject.GetComponentInChildren<MeshRenderer>().bounds.min.x,
                TerrainGameObject.GetComponentInChildren<MeshRenderer>().bounds.max.x);
            float minz = 0, maxz = 0;
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
            float randz = UnityEngine.Random.Range(minz, maxz);
            float randheight = 0;

            RaycastHit hit;
            float maxHeight = 10000;
            Ray ray = new Ray(new Vector3(randx, maxHeight, randz), Vector3.down);
            //Try to find whare in the ground is it contained
            foreach (MeshCollider componentsInChild in TerrainGameObject.GetComponentsInChildren<MeshCollider>())
            {
                if (componentsInChild.gameObject.GetComponent<MeshRenderer>().bounds.Contains(new Vector3(randx, 1, randz)))
                {
                    //Try to see if we can actually see straight down, if not move it.  
                    if (componentsInChild.Raycast(ray, out hit, 2.0f * maxHeight))
                    {
                        randheight = hit.point.y;
                    }
                    GameObject.Find("Waldo").GetComponent<Transform>().position = new Vector3(randx, randheight, randz);
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject.tag.Equals("Waldo"))
                        {
                            Debug.Log("Hit point: " + hit.point);
                            findingLocation = false;
                            break;
                        }
                    }
                }
            }

        }

    }

    // Update is called once per frame
	void Update () {
		
	}
}
