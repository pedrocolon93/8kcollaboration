using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

//using Windows.Kinect;

public class GestureManager : MonoBehaviour, KinectGestures.GestureListenerInterface
{
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;

    public long playerid = 0;
	[Tooltip("GUI-Text to display gesture-listener messages and gesture information.")]
	public Text gestureInfo;

	// singleton instance of the class
	public GestureManager instance = null;

	// internal variables to track if progress message has been displayed
	private bool progressDisplayed;
	private float progressGestureTime;

	// whether the needed gesture has been detected or not
	private bool swipeLeft;
	private bool swipeRight;
	private bool swipeUp;
    private bool swipeDown;
    private bool push;
    private bool pull;
    private bool psi;
    private bool raiserighthand;
    private bool zoomin;
    private bool zoomout;
    private bool wheel;
    private float zoomFactor;
    private float wheelAngle;
    private bool raiselefthand;

    /// <summary>
    /// Gets the singleton GestureManager instance.
    /// </summary>
    /// <value>The GestureManager instance.</value>
    public GestureManager Instance
	{
		get
		{
			return instance;
		}
	}
	
	/// <summary>
	/// Determines whether swipe left is detected.
	/// </summary>
	/// <returns><c>true</c> if swipe left is detected; otherwise, <c>false</c>.</returns>
	public bool IsSwipeLeft()
	{
		if(swipeLeft)
		{
			swipeLeft = false;
			return true;
		}
		
		return false;
	}

	/// <summary>
	/// Determines whether swipe right is detected.
	/// </summary>
	/// <returns><c>true</c> if swipe right is detected; otherwise, <c>false</c>.</returns>
	public bool IsSwipeRight()
	{
		if(swipeRight)
		{
			swipeRight = false;
			return true;
		}
		
		return false;
	}
    public bool IsPsi()
    {
        if (psi)
        {
            psi = false;
            return true;
        }

        return false;
    }
    /// <summary>
    /// Determines whether swipe up is detected.
    /// </summary>
    /// <returns><c>true</c> if swipe up is detected; otherwise, <c>false</c>.</returns>
    public bool IsSwipeUp()
	{
		if(swipeUp)
		{
			swipeUp = false;
			return true;
		}
		
		return false;
	}
    public bool IsSwipeDown()
    {
        if (swipeDown)
        {
            swipeDown = false;
            return true;
        }

        return false;
    }
    public bool IsPush()
    {
        if (push)
        {
            push = false;
            return true;
        }

        return false;
    }
    public bool IsPull()
    {
        if (pull)
        {
            pull = false;
            return true;
        }

        return false;
    }
    public float GetZoomFactor()
    {
        return zoomFactor;
    }

    /// <summary>
    /// Invoked when a new user is detected. Here you can start gesture tracking by invoking KinectManager.DetectGesture()-function.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="userIndex">User index</param>
    public void UserDetected(long userId, int userIndex)
    {
       
		// the gestures are allowed for the primary user only
		KinectManager manager = KinectManager.Instance;
		if(!manager || (userIndex != playerIndex))
			return;
        playerid = userId;
        userId = userIndex;
        // detect these user specific gestures
        /*manager.DetectGesture(userId, KinectGestures.Gestures.SwipeLeft);
		manager.DetectGesture(userId, KinectGestures.Gestures.SwipeRight);
		manager.DetectGesture(userId, KinectGestures.Gestures.SwipeUp);
        manager.DetectGesture(userId, KinectGestures.Gestures.SwipeDown);*/
        /*        manager.DetectGesture(userId,KinectGestures.Gestures.Push);
                manager.DetectGesture(userId, KinectGestures.Gestures.Pull);*/
        manager.DetectGesture(userId,KinectGestures.Gestures.Psi);
        manager.DetectGesture(userId, KinectGestures.Gestures.RaiseRightHand);
        manager.DetectGesture(userId, KinectGestures.Gestures.RaiseLeftHand);
        manager.DetectGesture(userId, KinectGestures.Gestures.ZoomIn);
        manager.DetectGesture(userId, KinectGestures.Gestures.ZoomOut);
        manager.DetectGesture(userId,KinectGestures.Gestures.Wheel);
        if (gestureInfo != null)
		{
			gestureInfo.text = "Swipe left, right or up to change the slides.";
		}
	}
    public bool IsRaiseLeftHand()
    {
        if (raiselefthand)
        {
            raiselefthand = false;
            return true;
        }

        return false;
    }
    public bool IsRaiseRightHand()
    {
        if (raiserighthand)
        {
            raiserighthand = false;
            return true;
        }

        return false;
    }

    public bool IsZoomOut()
    {
        if (zoomout)
        {
            zoomout = false;
            return true;
        }

        return false;
    }

    public bool IsWheel()
    {
        if (wheel)
        {
            wheel = false;
            return true;
        }

        return false;
    }

    public float GetWheelAngle()
    {
        return wheelAngle;
    }
    public bool IsZoomIn()
    {
        if (zoomin)
        {
            zoomin = false;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Invoked when a user gets lost. All tracked gestures for this user are cleared automatically.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="userIndex">User index</param>
    public void UserLost(long userId, int userIndex)
	{
		// the gestures are allowed for the primary user only
		if(userIndex != playerIndex)
			return;
		
		if(gestureInfo != null)
		{
			gestureInfo.text = string.Empty;
		}
	}

	/// <summary>
	/// Invoked when a gesture is in progress.
	/// </summary>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	/// <param name="gesture">Gesture type</param>
	/// <param name="progress">Gesture progress [0..1]</param>
	/// <param name="joint">Joint type</param>
	/// <param name="screenPos">Normalized viewport position</param>
	public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              float progress, KinectInterop.JointType joint, Vector3 screenPos)
	{
		// the gestures are allowed for the primary user only
		if(userIndex != playerIndex)
			return;
        if (gesture == KinectGestures.Gestures.ZoomOut)
        {
            if (progress > 0.5f)
            {
                zoomout = true;
                zoomFactor = screenPos.z;

                if (gestureInfo != null)
                {
                    string sGestureText = string.Format("{0} - {1:F0}%", gesture, screenPos.z * 100f);
                    gestureInfo.text = sGestureText;

                    progressDisplayed = true;
                    progressGestureTime = Time.realtimeSinceStartup;
                }
            }
            else
            {
                zoomout = false;
            }
        }
        else if (gesture == KinectGestures.Gestures.ZoomIn)
        {
            if (progress > 0.5f)
            {
                zoomin = true;
                zoomFactor = screenPos.z;

                if (gestureInfo != null)
                {
                    string sGestureText = string.Format("{0} factor: {1:F0}%", gesture, screenPos.z * 100f);
                    gestureInfo.text = sGestureText;

                    progressDisplayed = true;
                    progressGestureTime = Time.realtimeSinceStartup;
                }
            }
            else
            {
                zoomin = false;
            }
        }
        else if (gesture == KinectGestures.Gestures.Wheel)
        {
            if (progress > 0.5f)
            {
                wheel = true;
                wheelAngle = screenPos.z;

                if (gestureInfo != null)
                {
                    string sGestureText = string.Format("Wheel angle: {0:F0} degrees", screenPos.z);
                    gestureInfo.text = sGestureText;

                    progressDisplayed = true;
                    progressGestureTime = Time.realtimeSinceStartup;
                }
            }
            else
            {
                wheel = false;
            }
        }
        if ((gesture == KinectGestures.Gestures.ZoomOut || gesture == KinectGestures.Gestures.ZoomIn) && progress > 0.5f)
		{
			if(gestureInfo != null)
			{
				string sGestureText = string.Format ("{0} - {1:F0}%", gesture, screenPos.z * 100f);
				gestureInfo.text = sGestureText;
				
				progressDisplayed = true;
				progressGestureTime = Time.realtimeSinceStartup;
			}
		}
		else if((gesture == KinectGestures.Gestures.Wheel || gesture == KinectGestures.Gestures.LeanLeft || 
		         gesture == KinectGestures.Gestures.LeanRight) && progress > 0.5f)
		{
			if(gestureInfo != null)
			{
				string sGestureText = string.Format ("{0} - {1:F0} degrees", gesture, screenPos.z);
				gestureInfo.text = sGestureText;
				
				progressDisplayed = true;
				progressGestureTime = Time.realtimeSinceStartup;
			}
		}
		else if(gesture == KinectGestures.Gestures.Run && progress > 0.5f)
		{
			if(gestureInfo != null)
			{
				string sGestureText = string.Format ("{0} - progress: {1:F0}%", gesture, progress * 100);
				gestureInfo.text = sGestureText;
				
				progressDisplayed = true;
				progressGestureTime = Time.realtimeSinceStartup;
			}
		}
	}

	/// <summary>
	/// Invoked if a gesture is completed.
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	/// <param name="gesture">Gesture type</param>
	/// <param name="joint">Joint type</param>
	/// <param name="screenPos">Normalized viewport position</param>
	public bool GestureCompleted (long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectInterop.JointType joint, Vector3 screenPos)
	{
		// the gestures are allowed for the primary user only
		if(userIndex != playerIndex)
			return false;
		
		if(gestureInfo != null)
		{
			string sGestureText = gesture + " detected";
			gestureInfo.text = sGestureText;
		}
		
		if(gesture == KinectGestures.Gestures.SwipeLeft)
			swipeLeft = true;
		else if(gesture == KinectGestures.Gestures.SwipeRight)
			swipeRight = true;
		else if(gesture == KinectGestures.Gestures.SwipeUp)
			swipeUp = true;
        else if (gesture == KinectGestures.Gestures.SwipeDown)
            swipeDown = true;
        else if (gesture == KinectGestures.Gestures.Push)
		    push = true;
        else if (gesture == KinectGestures.Gestures.Pull)
		    pull = true;
        else if (gesture == KinectGestures.Gestures.Psi)
		    psi = true;
        else if (gesture == KinectGestures.Gestures.ZoomOut)
            zoomout = true;
        else if (gesture == KinectGestures.Gestures.ZoomIn)
            zoomin = true;
        else if (gesture == KinectGestures.Gestures.RaiseRightHand)
            raiserighthand = true;
        else if (gesture == KinectGestures.Gestures.RaiseLeftHand)
            raiselefthand = true;
        return true;
	}

	/// <summary>
	/// Invoked if a gesture is cancelled.
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	/// <param name="gesture">Gesture type</param>
	/// <param name="joint">Joint type</param>
	public bool GestureCancelled (long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectInterop.JointType joint)
	{
		// the gestures are allowed for the primary user only
		if(userIndex != playerIndex)
			return false;
        if (gesture == KinectGestures.Gestures.ZoomOut)
        {
            zoomout = false;
        }
        else if (gesture == KinectGestures.Gestures.ZoomIn)
        {
            zoomin = false;
        }
        else if (gesture == KinectGestures.Gestures.Wheel)
        {
            wheel = false;
        }


        if (progressDisplayed)
		{
			progressDisplayed = false;
			
			if(gestureInfo != null)
			{
				gestureInfo.text = String.Empty;
			}
		}
		
		return true;
	}

	
	void Awake()
	{
		instance = this;
	}

	void Update()
	{
		if(progressDisplayed && ((Time.realtimeSinceStartup - progressGestureTime) > 2f))
		{
			progressDisplayed = false;
			gestureInfo.text = String.Empty;

			Debug.Log("Forced progress to end.");
		}
	}

}
