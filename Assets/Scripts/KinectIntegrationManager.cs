using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class KinectIntegrationManager : MonoBehaviour {

    [Tooltip("List of the objects that may be dragged and dropped.")]
    public GameObject[] draggableObjects;

    [Tooltip("Drag speed of the selected object.")]
    public float dragSpeed = 3.0f;

    [Tooltip("Minimum Z-position of the dragged object, when moving forward and back.")]
    public float minZ = 0f;

    [Tooltip("Maximum Z-position of the dragged object, when moving forward and back.")]
    public float maxZ = 5f;

    [Tooltip("GUI-Text used to display information messages.")]
    public Text modeText;


    // interaction interactionManager reference
    private InteractionManager interactionManager;
    private bool isLeftHandDrag;

    // currently dragged object and its parameters
    private GameObject draggedObject;
    private bool cameraDrag = false;
    //private float draggedObjectDepth;
    private Vector3 draggedObjectOffset;
    private float draggedNormalZ;

    // normalized and pixel position of the cursor
    private Vector3 screenNormalPos = Vector3.zero;
    private Vector3 screenPixelPos = Vector3.zero;
    private Vector3 newObjectPos = Vector3.zero;

	private float speed = 2.0f;
	private float zoomSpeed = 2.0f;

    public float movementStrength = 10;

    private GestureManager gestureManager;
    public float zoominlimit = 1400;
    public float zoomoutlimit = -1400;
    private Vector3 newPosition;
    private Vector3 oldPosition;
    private Vector3 originalposition;
    //                          0                   1               2           3           4           5           6
    private string[] modes = {"SelectionMode", "SliceMode", "MoveMode", "RotationMode", "ZoomMode", "JoinMode", "DestroyMode"};
    private int currentmode = 0;
    public List<GameObject> SelectedObjects = new List<GameObject>();
    private float oldangle;

    void Start()
    {
        // get the gestures listener
        gestureManager = GetComponent<GestureManager>().instance;
        interactionManager = GetComponent<InteractionManager>().instance;

    }
    void Update ()
    {
        
        if (!gestureManager)
            return;

        modeText.text = modes[currentmode];
        

        if (currentmode == 4)
        {
            if (gestureManager.IsZoomIn())
            {
                if (transform.position.z < zoominlimit)
                    transform.position += Vector3.forward * movementStrength * gestureManager.GetZoomFactor() / 10;
            }
            if (gestureManager.IsZoomOut())
            {
                if (transform.position.z > zoomoutlimit)
                    transform.position += Vector3.back * movementStrength * gestureManager.GetZoomFactor() / 10;
            }
        }

        if (gestureManager.IsRaiseRightHand())
        {
            currentmode += 1;
            currentmode = (currentmode)% modes.Length;
            return;
        }
        if(currentmode == 3)
        { 
            if (gestureManager.IsWheel())
            {
                // rotate the model
                float turnAngle = gestureManager.GetWheelAngle();
                /*
                Vector3 newRotation = transform.rotation.eulerAngles;
                newRotation.y += turnAngle;
                float spinSpeed = 1;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(newRotation), spinSpeed * Time.deltaTime);*/
                gameObject.transform.RotateAround(Vector3.zero,Vector3.up,oldangle-turnAngle);
                oldangle = turnAngle;
            }
        }
        if (currentmode == 5)
        {
            if (gestureManager.IsRaiseLeftHand())
            {
                List<GameObject> replacement = new List<GameObject>();
                foreach(var item in SelectedObjects)
                {
                    if(item!=null)
                        replacement.Add(item);
                }
                SelectedObjects = replacement;
                if (SelectedObjects.Count >= 2)
                {
                    List<List<Vector4>> originalPointsCollection = new List<List<Vector4>>();
                    List<Vector4> joinedPoints = new List<Vector4>();
                    for (int i = 0; i < SelectedObjects.Count; i++)
                    {
                        joinedPoints.AddRange(SelectedObjects[i].GetComponent<PointCloudMesh>().originalContainedParticles);
                        originalPointsCollection.Add(SelectedObjects[i].GetComponent<PointCloudMesh>().originalContainedParticles.ToArray().ToList());
                        SelectedObjects[i].GetComponent<PointCloudManager>().DestroyPointCloud();
                        Destroy(SelectedObjects[i]);
                    }
                    GameObject joinedGameObject = CreateCube.CreateDataGameObject(joinedPoints,
                        GameObject.FindGameObjectWithTag("DataContainer").transform);
                    joinedGameObject.GetComponent<DatasetObjectController>().JoinedObjectsList = originalPointsCollection;
                    SelectedObjects.Clear();
                }
            }
            
            
        }
        if (currentmode == 6)
        {
            if (gestureManager.IsRaiseLeftHand())
            {
                if (SelectedObjects.Count > 0)
                {
                    for (int i = 0; i < SelectedObjects.Count; i++)
                    {
                        if (SelectedObjects[i].GetComponent<DatasetObjectController>().JoinedObjectsList.Count > 0)
                        {
                            foreach (var ChildList in SelectedObjects[i].GetComponent<DatasetObjectController>().JoinedObjectsList)
                            {
                                GameObject joinedGameObject = CreateCube.CreateDataGameObject(ChildList,
                                 GameObject.FindGameObjectWithTag("DataContainer").transform);

                            }
                            SelectedObjects[i].GetComponent<PointCloudManager>().DestroyPointCloud();
                            Destroy(SelectedObjects[i]);
                        }

                    }
                    SelectedObjects.Clear();
                }
               
              
            }


        }
        if (gestureManager.IsRaiseLeftHand())
        {
            currentmode -= 1;
            if (currentmode < 0)
            {
                currentmode = modes.Length - 1;
            }
            return;
        }
        //We are in slicemode
        if (currentmode == 1)
        {
            if (interactionManager != null && interactionManager.IsInteractionInitiated())
            {
                //Grab a slice target
                if (SelectedObjects.Count > 0)
                {
                    screenNormalPos = Vector3.zero;
                    screenPixelPos = Vector3.zero;
                    //Left hand slice4
                    if (interactionManager.IsLeftHandPrimary())
                    {
                        screenNormalPos = interactionManager.GetLeftHandScreenPos();
                        
                    }
                    //Right hand slice
                    // if there is a hand grip, select the underlying object and start dragging it.

                    else if (interactionManager.IsRightHandPrimary())
                    {
                       
                        screenNormalPos = interactionManager.GetRightHandScreenPos();
                        
                    }

                    // check if there is an underlying object to be selected
                    if (screenNormalPos != Vector3.zero)
                    {
                        // convert the normalized screen pos to pixel pos
                        if (interactionManager.playerIndex > 0 || gestureManager.playerIndex > 0)
                            screenPixelPos.x = (int)(screenNormalPos.x * gameObject.GetComponent<Camera>().pixelWidth) * (interactionManager.playerIndex + 1);
                        else
                            screenPixelPos.x = (int) (screenNormalPos.x*gameObject.GetComponent<Camera>().pixelWidth);
						screenPixelPos.y = (int) (screenNormalPos.y*gameObject.GetComponent<Camera>().pixelHeight);
                        Ray ray = gameObject.GetComponent<Camera>().ScreenPointToRay(screenPixelPos);
                        foreach(GameObject dataSquare in GameObject.FindGameObjectsWithTag("DataSquare"))
                        {
                            if(dataSquare.GetComponent<DatasetObjectController>().isSelected())
                            if (dataSquare.GetComponent<Slice>().TrySlice(ray))
                                break;
                        }
                    }
                }
            }
            
        }
        
        else
        {
            //If we are not in psi mode (drag and drop/drag to move)
            if (currentmode == 0)
            {
                //Be able to select a slice target.
                if (interactionManager != null && interactionManager.IsInteractionInitiated())
                {
                    screenNormalPos = Vector3.zero;
                    screenPixelPos = Vector3.zero;
                    
                    // if there is a hand grip, select the underlying object and start dragging it.
                    if (interactionManager.IsLeftHandPrimary())
                    {
                        // if the left hand is primary, check for left hand grip
                        if (interactionManager.GetLastLeftHandEvent() ==
                            InteractionManager.HandEventType.Grip)
                        {
                            
                            isLeftHandDrag = true;
                            screenNormalPos = interactionManager.GetLeftHandScreenPos();
                        }
                    }
                    else if (interactionManager.IsRightHandPrimary())
                    {
                        
                        // if the right hand is primary, check for right hand grip
                        if (interactionManager.GetLastRightHandEvent() ==
                            InteractionManager.HandEventType.Grip)
                        {
                            
                            isLeftHandDrag = false;
                            screenNormalPos = interactionManager.GetRightHandScreenPos();
                            
                        }
                       
                    }
                    //Check for release

                   
                    // check if there is an   underlying object to be selected
                    if (screenNormalPos != Vector3.zero)
                    {
                        // convert the normalized screen pos to pixel pos
                        if (interactionManager.playerIndex > 0 || gestureManager.playerIndex > 0)
                            screenPixelPos.x = (int)(screenNormalPos.x * gameObject.GetComponent<Camera>().pixelWidth)*(interactionManager.playerIndex+1);
                        else
                        screenPixelPos.x = (int) (screenNormalPos.x*gameObject.GetComponent<Camera>().pixelWidth);
                        screenPixelPos.y = (int) (screenNormalPos.y*gameObject.GetComponent<Camera>().pixelHeight);
                        if(interactionManager.playerIndex > 0 || gestureManager.playerIndex>0)
                            Debug.Log("");
                        Ray ray = gameObject.GetComponent<Camera>().ScreenPointToRay(screenPixelPos);

                        // check if there is an underlying objects
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {

                            for( int i = 0; i < draggableObjects.Length; i++)
                            {
                                GameObject obj = draggableObjects[i];
                                if (hit.collider.gameObject == obj && obj.tag.Equals("DataSquare"))
                                {
                                    if (SelectedObjects.Contains(obj))
                                    {
                                        break;
                                    }
                                    SelectedObjects.Add(obj);
                                    
                                    //draggedObjectDepth = draggedObject.transform.position.z - gameObject.GetComponent<Camera>().transform.position.z;                                   
                                    // start from the initial hand-z
                                    obj.GetComponent<DatasetObjectController>().setSelect(true);
                                    bool selcted = obj.GetComponent<DatasetObjectController>().isSelected();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (SelectedObjects.Count > 0)
                            {
                                foreach (var datasquare in GameObject.FindGameObjectsWithTag("DataSquare"))
                                {
                                    if (datasquare.GetComponent<DatasetObjectController>().isSelected())
                                    {
                                        datasquare.GetComponent<DatasetObjectController>().setSelect(false);
                                       
                                    }
                                }
                                SelectedObjects.Clear();

                            }
                        }
                    }
                    
                    
                }
            }
            else if(currentmode == 2)
            {
                //Psi mode enabled
                if (interactionManager != null && interactionManager.IsInteractionInitiated())
                {
                    if (draggedObject == null && cameraDrag == false)
                    {
                        screenNormalPos = Vector3.zero;
                        screenPixelPos = Vector3.zero;

                        // if there is a hand grip, select the underlying object and start dragging it.
                        if (interactionManager.IsLeftHandPrimary())
                        {
                            // if the left hand is primary, check for left hand grip
                            if (interactionManager.GetLastLeftHandEvent() ==
                                InteractionManager.HandEventType.Grip)
                            {
                                isLeftHandDrag = true;
                                screenNormalPos = interactionManager.GetLeftHandScreenPos();
                            }
                        }
                        else if (interactionManager.IsRightHandPrimary())
                        {
                            // if the right hand is primary, check for right hand grip
                            if (interactionManager.GetLastRightHandEvent() ==
                                InteractionManager.HandEventType.Grip)
                            {
                                isLeftHandDrag = false;
                                screenNormalPos = interactionManager.GetRightHandScreenPos();
                            }
                        }

                        // check if there is an underlying object to be selected
                        if (screenNormalPos != Vector3.zero)
                        {
                            // convert the normalized screen pos to pixel pos
                            if (interactionManager.playerIndex > 0 || gestureManager.playerIndex > 0)
                                screenPixelPos.x = (int)(screenNormalPos.x * gameObject.GetComponent<Camera>().pixelWidth) * (interactionManager.playerIndex + 1);
                            else
                                screenPixelPos.x = (int) (screenNormalPos.x*gameObject.GetComponent<Camera>().pixelWidth);
                            screenPixelPos.y = (int) (screenNormalPos.y*gameObject.GetComponent<Camera>().pixelHeight);
                            Ray ray = gameObject.GetComponent<Camera>().ScreenPointToRay(screenPixelPos);

                            // check if there is an underlying objects
                            RaycastHit hit;
                            if (Physics.Raycast(ray, out hit) && !cameraDrag)
                            {
                                foreach (GameObject obj in draggableObjects)
                                {
                                    if (hit.collider.gameObject == obj && obj.tag.Equals("DataSquare"))
                                    {
                                        // an object was hit by the ray. select it and start drgging
                                        draggedObject = obj;
                                        //draggedObjectDepth = draggedObject.transform.position.z - gameObject.GetComponent<Camera>().transform.position.z;
                                        draggedObjectOffset = hit.point - draggedObject.transform.position;
                                        draggedObjectOffset.z = 0; // don't change z-pos

                                        draggedNormalZ = (minZ + screenNormalPos.z*(maxZ - minZ)) -
                                                         draggedObject.transform.position.z;
                                        // start from the initial hand-z
                                        draggedObject.GetComponent<DatasetObjectController>().status = "Moving";
                                        draggedObject.GetComponent<Slice>().enabled = false;
                                        originalposition = draggedObject.transform.position;
                                        break;
                                    }
                                }
                            }
                            //We are dragging blank space
                            else
                            {
                                cameraDrag = true;

                                //draggedObjectDepth = draggedObject.transform.position.z - gameObject.GetComponent<Camera>().transform.position.z;
                                screenPixelPos.z = 10;
                                Vector3 vr = gameObject.GetComponent<Camera>().ScreenToWorldPoint(screenPixelPos);
                                draggedObjectOffset = gameObject.GetComponent<Camera>().ScreenToWorldPoint(screenPixelPos) - transform.position;
                                draggedObjectOffset.z = 0; // don't change z-pos

                                draggedNormalZ = (minZ + screenNormalPos.z*(maxZ - minZ)) -
                                                 transform.position.z;
                            }
                        }

                    }
                    else
                    {
                        if (cameraDrag)
                        {
                            // continue dragging the object
                            screenNormalPos = isLeftHandDrag
                                ? interactionManager.GetLeftHandScreenPos()
                                : interactionManager.GetRightHandScreenPos();

                            // convert the normalized screen pos to 3D-world pos
                            if (interactionManager.playerIndex > 0 || gestureManager.playerIndex > 0)
                                screenPixelPos.x = (int)(screenNormalPos.x * gameObject.GetComponent<Camera>().pixelWidth) * (interactionManager.playerIndex + 1);
                            else
                                screenPixelPos.x = (int) (screenNormalPos.x*gameObject.GetComponent<Camera>().pixelWidth);
                            screenPixelPos.y = (int) (screenNormalPos.y*gameObject.GetComponent<Camera>().pixelHeight);
                            //screenPixelPos.z = screenNormalPos.z + draggedObjectDepth;
                            screenPixelPos.z = 10;

                            newObjectPos = gameObject.GetComponent<Camera>().ScreenToWorldPoint(screenPixelPos) - draggedObjectOffset;
                            newObjectPos.z = transform.position.z;
                            transform.position = Vector3.Lerp(transform.position, newObjectPos,
                                dragSpeed*10*Time.deltaTime);

                            // check if the object (hand grip) was released
                            bool isReleased = isLeftHandDrag
                                ? (interactionManager.GetLastLeftHandEvent() ==
                                   InteractionManager.HandEventType.Release)
                                : (interactionManager.GetLastRightHandEvent() ==
                                   InteractionManager.HandEventType.Release);
                            if (isReleased)
                            {
                                cameraDrag = false;
                            }
                        }
                        else
                        {
                            // continue dragging the object
                            screenNormalPos = isLeftHandDrag
                                ? interactionManager.GetLeftHandScreenPos()
                                : interactionManager.GetRightHandScreenPos();

                            // convert the normalized screen pos to 3D-world pos
                            if (interactionManager.playerIndex > 0 || gestureManager.playerIndex > 0)
                                screenPixelPos.x = (int)(screenNormalPos.x * gameObject.GetComponent<Camera>().pixelWidth) * (interactionManager.playerIndex + 1);
                            else
                                screenPixelPos.x = (int) (screenNormalPos.x*gameObject.GetComponent<Camera>().pixelWidth);
                            screenPixelPos.y = (int) (screenNormalPos.y*gameObject.GetComponent<Camera>().pixelHeight);
                            //screenPixelPos.z = screenNormalPos.z + draggedObjectDepth;
                            screenPixelPos.z = (minZ + screenNormalPos.z*(maxZ - minZ)) - draggedNormalZ -
                                               gameObject.GetComponent<Camera>().transform.position.z;

                            newObjectPos = gameObject.GetComponent<Camera>().ScreenToWorldPoint(screenPixelPos) - draggedObjectOffset;
                            oldPosition = draggedObject.transform.position;
                            draggedObject.transform.position = Vector3.Lerp(draggedObject.transform.position,
                                newObjectPos,
                                dragSpeed*Time.deltaTime);
                            newPosition = draggedObject.transform.position;
                            draggedObject.GetComponent<PointCloudMesh>()
                                   .UpdateContainedPointDisplay(newPosition - oldPosition);



                            // check if the object (hand grip) was released
                            bool isReleased = isLeftHandDrag
                                ? (interactionManager.GetLastLeftHandEvent() ==
                                   InteractionManager.HandEventType.Release)
                                : (interactionManager.GetLastRightHandEvent() ==
                                   InteractionManager.HandEventType.Release);
                            if (isReleased)
                            {
                                // restore the object's material and stop dragging the object
                                draggedObject.GetComponent<DatasetObjectController>().status = "None";
                                draggedObject.GetComponent<Slice>().enabled = true;
                                draggedObject.GetComponent<Slice>().boolhit = false;
                                StartCoroutine(MoveContainedParticles(draggedObject));
                                draggedObject = null;
                            }
                        }


                    }
                }
            }
        }

    }

    IEnumerator MoveContainedParticles(GameObject g)
    {
        Vector3 movementDelta = newPosition - originalposition;
        MoveParticlesJob myJob = new MoveParticlesJob();
        myJob.movementDelta = movementDelta;
        myJob.OutDataList = g.GetComponent<PointCloudMesh>().containedParticles;
        myJob.Start();
        yield return StartCoroutine(myJob.WaitFor());
    }
    public void AddDragable(GameObject dataManipulationCube)
    {
        List<GameObject> dragableList = new List<GameObject>();
        foreach (var VARIABLE in draggableObjects)
        {
            dragableList.Add(VARIABLE);
        }
        dragableList.Add(dataManipulationCube);
        draggableObjects = dragableList.ToArray();
    }

    public void RemoveDragable(GameObject dataManipulationCube)
    {
        List<GameObject> newDraggableGameObjects = new List<GameObject>();
        for (int i = 0; i < draggableObjects.Length; i++)
        {
            if (draggableObjects[i].Equals(dataManipulationCube))
            {
                continue;
            }
            newDraggableGameObjects.Add(draggableObjects[i]);
        }
        draggableObjects = newDraggableGameObjects.ToArray();
    }
}
