using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CielaSpike;

[RequireComponent(typeof(MeshCollider))]

public class DragWithMouse : MonoBehaviour 
{

	private Vector3 screenPoint;
	private Vector3 offset;
	public Vector3 oldPosition;
	public Vector3 newPosition;
    public Vector3 originalPosition;
	void OnMouseDown()
	{
        foreach (var datasquares in GameObject.FindGameObjectsWithTag("DataSquare"))
        {

            datasquares.GetComponent<Slice>().enabled = false;

        }    
		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        //Initialize positions JIC
        
        originalPosition = newPosition = oldPosition = gameObject.transform.position;
        gameObject.GetComponent<DatasetObjectController>().status = "Moving";
	}

	void OnMouseDrag()
	{
		//Set the old position for references
		oldPosition = transform.position;
		//Get mouse position
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		//Convert it to a world position
		Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint);
	    curPosition.z = transform.position.z;
		//Set as the new position and shift so we drag from the center
	    Vector3 boundsVector3 = gameObject.GetComponent<MeshCollider>().bounds.extents;
	    boundsVector3.z = 0;

        transform.position = curPosition - boundsVector3;
		//Set the new position for references
		newPosition = curPosition - boundsVector3;
        

	}
	void OnMouseUp()
	{
        foreach (var datasquares in GameObject.FindGameObjectsWithTag("DataSquare"))
        {

            datasquares.GetComponent<Slice>().enabled = true;

        }
        GetComponent<Slice> ().boolhit = false;
	    GetComponent<DatasetObjectController>().status = "None";
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        //Initialize positions JIC
        newPosition = oldPosition = gameObject.transform.position;
        
        StartCoroutine(MoveContainedParticles());
	}

    IEnumerator MoveContainedParticles()
    {
        Vector3 movementDelta = newPosition - originalPosition;
        MoveParticlesJob myJob = new MoveParticlesJob();
        myJob.movementDelta = movementDelta;
        myJob.OutDataList = GetComponent<PointCloudMesh>().containedParticles;
        myJob.Start();
        yield return StartCoroutine(myJob.WaitFor());
    }
}