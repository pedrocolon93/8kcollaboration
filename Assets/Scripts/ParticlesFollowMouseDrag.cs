using UnityEngine;
using System.Collections;

public class ParticlesFollowMouseDrag : MonoBehaviour
{
	public GameObject ParentSquare;
	void Update()
	{
		if (ParentSquare == null)
			return;
		Vector3 newpos = ParentSquare.GetComponent<DragWithMouse> ().newPosition;
		Vector3 oldpos = ParentSquare.GetComponent<DragWithMouse> ().oldPosition;
		Vector3 movementDelta = newpos - oldpos;
//		transform.position += movementDelta;
        if(movementDelta.Equals(Vector3.zero))
            return;
	    GetComponent<PointCloudManager>().UpdatePointCloudPosition(movementDelta);
	}

    
}

