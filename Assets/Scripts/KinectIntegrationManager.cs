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
    public float dragSpeed = 1.0f;

    [Tooltip("Minimum Z-position of the dragged object, when moving forward and back.")]
    public float minZ = -10000f;

    [Tooltip("Maximum Z-position of the dragged object, when moving forward and back.")]
    public float maxZ = 10000f;

    [Tooltip("GUI-Text used to display information messages.")]
    public Text modeText;

    [Tooltip("Camera gameobjet that will be manipulated")]
    public GameObject manipulatedCamera;


    // interaction interactionManager reference
    private InteractionManager interactionManager;
    private bool isLeftHandDrag;

    // currently dragged object and its parameters
    private GameObject draggedObject;
    private bool cameraDrag = false;
    //private float draggedObjectDepth;
    private Vector3 draggedObjectOffset;

    // normalized and pixel position of the cursor
    private Vector3 screenNormalPos = Vector3.zero;
    private Vector3 screenPixelPos = Vector3.zero;
    private Vector3 newObjectPos = Vector3.zero;

	private float speed = 2.0f;
	private float zoomSpeed = 2.0f;

    public float movementStrength = 10;

    private GestureManager gestureManager;
    public float zoominlimit = 10;
    public float zoomoutlimit = 14000;
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
        manipulatedCamera = (GameObject) GameObject.FindGameObjectWithTag("MainCamera");
        // get the gestures listener
        gestureManager = GetComponent<GestureManager>().instance;
        interactionManager = GetComponent<InteractionManager>().instance;

    }
    void Update ()
    {
        
        if (!gestureManager)
            return;
        

        if (gestureManager.IsZoomIn())
        {
            if (manipulatedCamera.transform.position.y < zoominlimit)
                manipulatedCamera.transform.position += Vector3.up * movementStrength * gestureManager.GetZoomFactor() / 10;
        }
        if (gestureManager.IsZoomOut())
        {
            if (manipulatedCamera.transform.position.y > zoomoutlimit)
                manipulatedCamera.transform.position += Vector3.down * movementStrength * gestureManager.GetZoomFactor() / 10;
        }
        
        
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
                   
                    screenPixelPos.x = (int) (screenNormalPos.x* manipulatedCamera.GetComponent<Camera>().pixelWidth);
                    screenPixelPos.y = (int)(screenNormalPos.y * manipulatedCamera.GetComponent<Camera>().pixelHeight);
                    screenPixelPos.z = 2;
                    Ray ray = manipulatedCamera.GetComponent<Camera>().ScreenPointToRay(screenPixelPos);

                    // check if there is an underlying objects
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit) && !cameraDrag)
                    {
                        if (!hit.collider.gameObject.tag.Equals("Waldo"))
                        {
                            //If we did not hit waldo
                            cameraDrag = true;

                            //draggedObjectDepth = draggedObject.transform.position.z - gameObject.GetComponent<Camera>().transform.position.z;
                            Vector3 vr = manipulatedCamera.GetComponent<Camera>().ScreenToWorldPoint(screenPixelPos);
                            draggedObjectOffset = manipulatedCamera.GetComponent<Camera>().ScreenToWorldPoint(screenPixelPos) - manipulatedCamera.transform.position;
                            draggedObjectOffset.y = 0; // don't change z-pos
                        }
                        else
                        {
                            //we did hit waldo!
                            GameObject.Find("Canvas").GetComponent<CanvasManager>().EnableCanvas();
                        }
                        
                    }
                    //We are dragging blank space
                    else
                    {
                        //If we did not hit waldo
                        cameraDrag = true;

                        //draggedObjectDepth = draggedObject.transform.position.z - gameObject.GetComponent<Camera>().transform.position.z;
                        Vector3 vr = manipulatedCamera.GetComponent<Camera>().ScreenToWorldPoint(screenPixelPos);
                        draggedObjectOffset = manipulatedCamera.GetComponent<Camera>().ScreenToWorldPoint(screenPixelPos) - manipulatedCamera.transform.position;
                        draggedObjectOffset.y = 0; // don't change z-pos

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

                    screenPixelPos.x = (int)(screenNormalPos.x * manipulatedCamera.GetComponent<Camera>().pixelWidth);
                    screenPixelPos.y = (int)(screenNormalPos.y * manipulatedCamera.GetComponent<Camera>().pixelHeight);
                    screenPixelPos.z = 2;

                    newObjectPos = manipulatedCamera.GetComponent<Camera>().ScreenToWorldPoint(screenPixelPos) - draggedObjectOffset;
                    newObjectPos.y = manipulatedCamera.transform.position.y;
                    manipulatedCamera.transform.position = Vector3.Lerp(manipulatedCamera.transform.position, newObjectPos,
                        dragSpeed*Time.deltaTime);

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
            }
        }
            
        

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
