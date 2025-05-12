using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ImageCollider : MonoBehaviour
{
	
	public RectTransform mainCollider;
	public RectTransform[] otherColliders;
	
    protected void Start()
    {
    }

	protected RectTransform Move(Vector3 direction)
	{
		float x = direction.x;
		float y = direction.y;
		if (x == 0 && y == 0)
		{
			return null;
		}
		
		Collision noneCollision = TestCollisions(0, 0, false, Direction.None);
		if (noneCollision.Collider == null)
		{
			Move(x, y);
			return null;
		}
		
		float rayLength = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
		//Collision topCollision = TestCollisions(0, rayLength, Direction.Up);
		//Collision bottomCollision = TestCollisions(0, -rayLength, Direction.Down);
		//Collision leftCollision = TestCollisions(-rayLength, 0, Direction.Left);
		//Collision rightCollision = TestCollisions(rayLength, -rayLength, Direction.Right);
		
		if (x != 0)
		{
			if (x > 0)
			{ 
				Collision rightCollision = TestCollisions(rayLength, 0, true, Direction.Right);
				Collision upRightCollision = TestCollisions(rayLength, +rayLength, false, Direction.Right);
				Collision downRightCollision = TestCollisions(rayLength, -rayLength, false, Direction.Right);
				//Debug.Log(rayLength);
				//Debug.Log("Right: " + rightCollision + "\tUpRight: " + upRightCollision + "\tDownRight: " + downRightCollision);
				MoveRight(x, y, rightCollision, upRightCollision, downRightCollision);
			}
			else if (x < 0)
			{
				Collision leftCollision = TestCollisions(-rayLength, 0, true, Direction.Right);
				Collision upLeftCollision = TestCollisions(-rayLength, +rayLength, false, Direction.Right);
				Collision downLeftCollision = TestCollisions(-rayLength, -rayLength, false, Direction.Right);
				MoveLeft(x, y, leftCollision, upLeftCollision, downLeftCollision);
			}
			
		}
		if (y != 0)
		{
			if (y > 0)
			{
				Collision topCollision = TestCollisions(0, rayLength, true, Direction.Up);
				Collision leftTopCollision = TestCollisions(-rayLength, rayLength, false, Direction.Up);
				Collision rightTopCollision = TestCollisions(+rayLength, rayLength, false, Direction.Up);
				MoveUp(x, y, topCollision, leftTopCollision, rightTopCollision);
			}
			else if (y < 0)
			{
				Collision downCollision = TestCollisions(0, -rayLength, true, Direction.Down);
				Collision leftDownCollision = TestCollisions(-rayLength, -rayLength, false, Direction.Down);
				Collision rightDownCollision = TestCollisions(rayLength, -rayLength, false, Direction.Down);
				MoveDown(x, y, downCollision, leftDownCollision, rightDownCollision);
			}
		}
		
		return noneCollision.Collider;
	}
	
	private Collision TestCollisions(float x, float y, bool line, Direction direction)
	{
		RectTransform collidedWith = null;
        foreach (RectTransform otherCollider in otherColliders)
		{
			Vector3 mainColliderPosition = mainCollider.localPosition;
			Vector3 mainColliderScale = mainCollider.localScale;
			float m_width = mainCollider.rect.width * Mathf.Abs(mainColliderScale.x);
			float m_height = mainCollider.rect.height * Mathf.Abs(mainColliderScale.y);
			float m_halfWidth = line ? 1f : (m_width * 0.5f);
			float m_halfHeight = line ? 1f : (m_height * 0.5f);
			float m_left = mainColliderPosition.x - m_halfWidth + x;
			float m_bottom = mainColliderPosition.y - m_halfHeight + y;
			float m_right = mainColliderPosition.x + m_halfWidth + x;
			float m_top = mainColliderPosition.y + m_halfHeight + y;
			
			Vector3 otherColliderPosition = otherCollider.localPosition;
			Vector3 otherColliderScale = otherCollider.localScale;
			float o_width = otherCollider.rect.width * otherColliderScale.x;
			float o_height = otherCollider.rect.height * otherColliderScale.y;
			float o_halfWidth = o_width * 0.5f;
			float o_halfHeight = o_height * 0.5f;
			float o_left = otherColliderPosition.x - o_halfWidth;
			float o_bottom = otherColliderPosition.y - o_halfHeight;
			float o_right = otherColliderPosition.x + o_halfWidth;
			float o_top = otherColliderPosition.y + o_halfHeight;
			
			if (o_left > m_right || o_bottom > m_top || m_left > o_right || m_bottom > o_top)
			{
				continue;
			}
			
			collidedWith = otherCollider;
			break;
		}
		
		return new Collision(collidedWith, direction);
	}
	
	protected void MoveUp(float x, float y, Collision top, Collision left, Collision right)
	{
		RectTransform topCollider = top.Collider;
		if (topCollider == null)
		{
			Move(0, y);
			return;
		}
		RectTransform leftCollider = left.Collider;
		RectTransform rightCollider =  right.Collider;
		
		Vector3 mainColliderPosition = mainCollider.localPosition;
		Vector3 mainColliderScale = mainCollider.localScale;
		float m_width = mainCollider.rect.width * Mathf.Abs(mainColliderScale.x);
		float m_height = mainCollider.rect.height * Mathf.Abs(mainColliderScale.y);
		float m_halfWidth = m_width * 0.5f;
		float m_halfHeight = m_height * 0.5f;
		float m_left = mainColliderPosition.x - m_halfWidth + x;
		float m_bottom = mainColliderPosition.y - m_halfHeight + y;
		float m_right = mainColliderPosition.x + m_halfWidth + x;
		float m_top = mainColliderPosition.y + m_halfHeight + y;
		
		Vector3 otherColliderPosition = topCollider.localPosition;
		Vector3 otherColliderScale = topCollider.localScale;
		float o_width = topCollider.rect.width * otherColliderScale.x;
		float o_height = topCollider.rect.height * otherColliderScale.y;
		float o_halfWidth = o_width * 0.5f;
		float o_halfHeight = o_height * 0.5f;
		float o_left = otherColliderPosition.x - o_halfWidth;
		float o_bottom = otherColliderPosition.y - o_halfHeight;
		float o_right = otherColliderPosition.x + o_halfWidth;
		float o_top = otherColliderPosition.y + o_halfHeight;
		 
		float offsetY = 0;
		if (y > 0f && m_top > o_bottom)
		{
			offsetY = m_top - o_bottom - (y * 2f);
		}
		
		Move(0, y - offsetY);
	}
	
	protected void MoveDown(float x, float y, Collision bottom, Collision left, Collision right)
	{
		RectTransform bottomCollider = bottom.Collider;
		if (bottomCollider == null)
		{
			Move(0, y);
			return;
		}
		RectTransform leftCollider = left.Collider;
		RectTransform rightCollider =  right.Collider;
		
		Vector3 mainColliderPosition = mainCollider.localPosition;
		Vector3 mainColliderScale = mainCollider.localScale;
		float m_width = mainCollider.rect.width * Mathf.Abs(mainColliderScale.x);
		float m_height = mainCollider.rect.height * Mathf.Abs(mainColliderScale.y);
		float m_halfWidth = m_width * 0.5f;
		float m_halfHeight = m_height * 0.5f;
		float m_left = mainColliderPosition.x - m_halfWidth + x;
		float m_bottom = mainColliderPosition.y - m_halfHeight + y;
		float m_right = mainColliderPosition.x + m_halfWidth + x;
		float m_top = mainColliderPosition.y + m_halfHeight + y;
		
		Vector3 otherColliderPosition = bottomCollider.localPosition;
		Vector3 otherColliderScale = bottomCollider.localScale;
		float o_width = bottomCollider.rect.width * otherColliderScale.x;
		float o_height = bottomCollider.rect.height * otherColliderScale.y;
		float o_halfWidth = o_width * 0.5f;
		float o_halfHeight = o_height * 0.5f;
		float o_left = otherColliderPosition.x - o_halfWidth;
		float o_bottom = otherColliderPosition.y - o_halfHeight;
		float o_right = otherColliderPosition.x + o_halfWidth;
		float o_top = otherColliderPosition.y + o_halfHeight;
		
		float offsetY = 0;
		if (y < 0f && m_bottom < o_top)
		{
			offsetY = m_bottom - o_top - (y * 2f);
		}
	
		Move(0, y - offsetY);
	}
	
	protected void MoveRight(float x, float y, Collision right, Collision top, Collision down)
	{
		RectTransform rightCollider = right.Collider;
		if (rightCollider == null)
		{
			Move(x, 0);
			return;
		}
		RectTransform topCollider = top.Collider;
		RectTransform downCollider =  down.Collider;
 
		Vector3 mainColliderPosition = mainCollider.localPosition;
		Vector3 mainColliderScale = mainCollider.localScale;
		float m_width = mainCollider.rect.width * Mathf.Abs(mainColliderScale.x);
		float m_height = mainCollider.rect.height * Mathf.Abs(mainColliderScale.y);
		float m_halfWidth = m_width * 0.5f;
		float m_halfHeight = m_height * 0.5f;
		float m_left = mainColliderPosition.x - m_halfWidth + x;
		float m_bottom = mainColliderPosition.y - m_halfHeight;
		float m_right = mainColliderPosition.x + m_halfWidth + x;
		float m_top = mainColliderPosition.y + m_halfHeight;
		
		Vector3 otherColliderPosition = rightCollider.localPosition;
		Vector3 otherColliderScale = rightCollider.localScale;
		float o_width = rightCollider.rect.width * otherColliderScale.x;
		float o_height = rightCollider.rect.height * otherColliderScale.y;
		float o_halfWidth = o_width * 0.5f;
		float o_halfHeight = o_height * 0.5f;
		float o_left = otherColliderPosition.x - o_halfWidth;
		float o_bottom = otherColliderPosition.y - o_halfHeight;
		float o_right = otherColliderPosition.x + o_halfWidth;
		float o_top = otherColliderPosition.y + o_halfHeight;
		
		float offsetX = 0;
		if (x > 0 && m_right > o_left)
		{
			//Debug.Log("Up " + topCollider + "\tCollider " + otherCollider); 
			offsetX = m_right - o_left - (x * 2f);
			//Debug.Log("OffsetX " + offsetX);
			//offsetX = offsetX > x ? x : offsetX;
		}
			
		Move(x - offsetX, 0);
	}
	
	protected void MoveLeft(float x, float y, Collision left, Collision top, Collision down)
	{
		RectTransform leftCollider = left.Collider;
		if (leftCollider == null)
		{
			Move(x, 0);
			return;
		}
		RectTransform topCollider = top.Collider;
		RectTransform downCollider =  down.Collider;
		
		Vector3 mainColliderPosition = mainCollider.localPosition;
		Vector3 mainColliderScale = mainCollider.localScale;
		float m_width = mainCollider.rect.width * Mathf.Abs(mainColliderScale.x);
		float m_height = mainCollider.rect.height * Mathf.Abs(mainColliderScale.y);
		float m_halfWidth = m_width * 0.5f;
		float m_halfHeight = m_height * 0.5f;
		float m_left = mainColliderPosition.x - m_halfWidth + x;
		float m_bottom = mainColliderPosition.y - m_halfHeight + y;
		float m_right = mainColliderPosition.x + m_halfWidth + x;
		float m_top = mainColliderPosition.y + m_halfHeight + y;
		
		Vector3 otherColliderPosition = leftCollider.localPosition;
		Vector3 otherColliderScale = leftCollider.localScale;
		float o_width = leftCollider.rect.width * otherColliderScale.x;
		float o_height = leftCollider.rect.height * otherColliderScale.y;
		float o_halfWidth = o_width * 0.5f;
		float o_halfHeight = o_height * 0.5f;
		float o_left = otherColliderPosition.x - o_halfWidth;
		float o_bottom = otherColliderPosition.y - o_halfHeight;
		float o_right = otherColliderPosition.x + o_halfWidth;
		float o_top = otherColliderPosition.y + o_halfHeight;
		
		float offsetX = 0;
		if (x < 0 && m_left < o_right)
		{
			offsetX = m_left - o_right - (x * 2f); 	// Magic number
			//offsetX = offsetX < x ? x : offsetX;
		}
			
		Move(x - offsetX, 0);
	}
	
	private void Move(float x, float y)
	{
		Vector3 localPosition = mainCollider.transform.localPosition;
		localPosition.Set(localPosition.x + x, localPosition.y + y, localPosition.z);
		mainCollider.transform.localPosition = localPosition;
	}
	
	protected RectTransform GetMainCollider()
	{
		return mainCollider;
	}
	
	protected RectTransform[] GetOtherColliders()
	{
		return otherColliders;
	}
	
	public void SetColliders(RectTransform[] colliders)
	{
		this.otherColliders = colliders;
	}
	
}

public struct Collision
{
    public Collision(RectTransform collider, Direction direction)
    {
        Collider = collider;
        Direction = direction;
    }

    public RectTransform Collider
	{ 
		get; 
	}
	
    public Direction Direction
	{ 
		get; 
	}
	
	public override string ToString() => $"({Direction} >>> {Collider})";
	
}

public enum Direction 
{
	None,
	Up,
	Down,
	Left,
	Right
}