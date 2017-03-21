using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraManager : MonoBehaviour, KinectGestures.GestureListenerInterface
{
    [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
    public int playerIndex = 0;

    public int maxPlayers = 2;

    private int playerCount = 0;
    // singleton instance of the class

    /// <summary>
    /// Invoked when a new user is detected. Here you can start gesture tracking by invoking KinectManager.DetectGesture()-function.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="userIndex">User index</param>
    public void UserDetected(long userId, int userIndex)
    {
        // the gestures are allowed for the primary user only
        KinectManager manager = KinectManager.Instance;
        if (!manager)
            return;
        if (playerCount == (maxPlayers))
        {
            return;
        }
        else
        {
            playerCount++;
        }
        
        //Initialize the new player controller
        GameObject playerController = (GameObject)Instantiate(Resources.Load("Player"));
        playerController.GetComponent<GestureManager>().UserDetected(userId,userIndex);
        manager.refreshGestureListeners();
    }
    

    /// <summary>
    /// Invoked when a user gets lost. All tracked gestures for this user are cleared automatically.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="userIndex">User index</param>
    public void UserLost(long userId, int userIndex)
    {
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("PlayerController"))
        {
            if (g.GetComponent<GestureManager>().playerIndex == userIndex)
            {
                playerCount--;
                Destroy(g);
                break;
            }
        }
        KinectManager.Instance.refreshGestureListeners();
    }
    
    void Awake()
    {
        
    }

    void Update()
    {
        
    }

    public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
    {
        return;
    }

    public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint, Vector3 screenPos)
    {
        return false;
    }

    public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint)
    {
        return false;
    }
}
