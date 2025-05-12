using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : Singletroon<PlayerCam>
{
	
	private bool isTurning, canTurn;
	public float turningStartTime, turningSpeed, turningRange;
	
	private Vector3 target, previousTarget;
	public LookDirection currentLookDirection;
	
	private Vector2 mousePositionNormalized;
	private MouseDirection currentMouseDirection;
	
    public bool allowFullTurns = true;
    public bool frozen = false;

	void Start() 
	{
		isTurning = false;
		canTurn = false;
		// turningStartTime = 0f;
		// turningSpeed = 0.33f;
		// turningRange = 0.125f;
		
		target = new Vector3();
		previousTarget = new Vector3();
		currentLookDirection = LookDirection.Front;
		
		mousePositionNormalized = new Vector2();
		currentMouseDirection = MouseDirection.None;
	}
	
    void Update ()
    {
        if (frozen)
            return;
		if (!isTurning) 
		{
			UpdateMousePosition();
			if (mousePositionNormalized.x >= (1.0f - turningRange)) 
			{
				currentMouseDirection = MouseDirection.Right;
			}
			else if (mousePositionNormalized.x <= turningRange) 
			{
				currentMouseDirection = MouseDirection.Left;
			}
			else {
				// if (!canTurn) Log("Can now turn");
				currentMouseDirection = 0;
				canTurn = true;
			}
		}
		
		
		if (!isTurning && canTurn) 
		{
			if (currentMouseDirection == MouseDirection.Right) 
			{
				StartTurningRight();
			}
			else if (currentMouseDirection == MouseDirection.Left) 
			{
				StartTurningLeft();
			}
		}
		
		if (isTurning) 
		{
			float percentage = Turn();
			if (percentage >= 1.0) 
			{
				EndTurning();
                canTurn = allowFullTurns;
			}
		}
    }
	
	private void UpdateMousePosition() 
	{
		Vector3 mousePosition = Input.mousePosition;
		float mouseXPercentage = mousePosition.x / Camera.main.pixelWidth;
		float mouseYPercentage = mousePosition.y / Camera.main.pixelHeight;
		mousePositionNormalized.Set(mouseXPercentage, mouseYPercentage);
	}
	
	
	private void StartTurningRight() 
	{
		LookDirection nextLookDirection;
		switch (currentLookDirection) 
		{
			case LookDirection.Left:
				nextLookDirection = LookDirection.LeftToFront;
				break;
			case LookDirection.Front:
				nextLookDirection = LookDirection.FrontToRight;
				break;
			default:
				return;
		}
		StartTurning(-90f, nextLookDirection);
	}
	
	private void StartTurningLeft() 
	{
		LookDirection nextLookDirection;
		switch (currentLookDirection) 
		{
			case LookDirection.Right:
				nextLookDirection = LookDirection.RightToFront;
				break;
			case LookDirection.Front:
				nextLookDirection = LookDirection.FrontToLeft;
				break;
			default:
				return;
		}
		StartTurning(+90f, nextLookDirection);
	}
	
	
	private void StartTurning(float yawDifference, LookDirection targetLookDirection) 
	{
		isTurning = true;
		canTurn = false;
		currentLookDirection = targetLookDirection;
		turningStartTime = Time.time;
		previousTarget.Set(target.x, target.y, target.z);
		target.Set(0f, target.y + yawDifference, 0f);
		// Log("Started turning '" + targetLookDirection.ToString() + "'");
	}
	
	private float Turn() 
	{
		// Slerp ripped off of the official documentation
		// https://docs.unity3d.com/ScriptReference/Vector3.Slerp.html
        Vector3 center = previousTarget + target;
        Vector3 previousTargetRelCenter = previousTarget - center;
        Vector3 targetRelCenter = target - center;
        float fracComplete = (Time.time - turningStartTime) / turningSpeed;
        transform.localEulerAngles = Vector3.Slerp(targetRelCenter, previousTargetRelCenter, fracComplete);
		return fracComplete;
	}
	
	private void EndTurning() 
	{
		isTurning = false;
		switch (currentLookDirection)
		{
			case LookDirection.FrontToRight:
				currentLookDirection = LookDirection.Right;
				break;
			case LookDirection.FrontToLeft:
				currentLookDirection = LookDirection.Left;
				break;
			case LookDirection.LeftToFront:
			case LookDirection.RightToFront:
				currentLookDirection = LookDirection.Front;
				break;
		}
		// Log("Ended turning, looking at '" + currentLookDirection.ToString() + "'");
	}
	
	private void Log(string message) {
		Debug.Log("[PlayerCam] " + message);
	}
	
}

public enum MouseDirection
{
	None,
	Right,
	Left
}

public enum LookDirection
{
    Front,
	FrontToRight,
	FrontToLeft,
	Right,
	RightToFront,
	Left,
	LeftToFront
}

