using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Doll : MonoBehaviour
{
	public float framerate;
	public float speed;
	private float elapsedTime;
	private Vector3 direction;
    private static Vector3 faceleft => new Vector3(-0.5f, 0.5f, 1f);
    private static Vector3 faceRight => new Vector3(0.5f, 0.5f, 1f);
    public Soyjak[] soyjaks;
    private KeyCode upKey;
    private KeyCode downKey;
    private KeyCode leftKey;
    private KeyCode rightKey;
	public RectTransform room;
    private RectTransform myRt;
    private Rect me;
    private Rect inside;

    private void Awake()
    {
        upKey = SoySettings.Instance.upKey;
        downKey = SoySettings.Instance.downKey;
        leftKey = SoySettings.Instance.leftKey;
        rightKey = SoySettings.Instance.rightKey;
    }

    private void Start()
    {
        inside = room.rect;
        myRt = GetComponent<RectTransform>();
        me = myRt.rect;
		direction = new Vector3();
    }

    private float GetDollEdge(Direction direction = Direction.None)
    {
        switch (direction)
        {
            case Direction.Up:
                return myRt.anchoredPosition.y + speed + me.height/2;
            case Direction.Down:
                return myRt.anchoredPosition.y - speed - me.height/2;
            case Direction.Right:
                return myRt.anchoredPosition.x + speed + me.width/2;
            case Direction.Left:
                return myRt.anchoredPosition.x - speed - me.width/2;
        }
        return 0f;
    }

    private bool WillDollBeInRect(Rect r, Direction direction = Direction.None)
    {
        float dollEdge = GetDollEdge(direction);
        float boundToCheck;
        switch (direction)
        {
            case Direction.Right:
                boundToCheck = room.anchoredPosition.x + r.width / 2;
                return dollEdge < boundToCheck;
            case Direction.Left:
                boundToCheck = room.anchoredPosition.x - r.width / 2;
                return dollEdge > boundToCheck;
            case Direction.Up:
                boundToCheck = room.anchoredPosition.y + r.height / 2;
                return dollEdge < boundToCheck;
            case Direction.Down:
                boundToCheck = room.anchoredPosition.y - r.height / 2;
                return dollEdge > boundToCheck;
        }
        return true;
    }

    bool IsPointInRT(Vector2 point, RectTransform rt)
    {
        Rect rect = rt.rect;

        float leftSide = rt.anchoredPosition.x - rect.width / 2;
        float rightSide = rt.anchoredPosition.x + rect.width / 2;
        float topSide = rt.anchoredPosition.y + rect.height / 2;
        float bottomSide = rt.anchoredPosition.y - rect.height / 2;

        if (point.x > leftSide &&
            point.x < rightSide &&
            point.y > bottomSide &&
            point.y < topSide)
        {
            return true;
        }
        return false;
    }


    private Soyjak CollideWithSoyjak(Direction direction = Direction.None)
    {
        Vector2 nextPos;
        if (direction == Direction.Left || direction == Direction.Right)
            nextPos = new Vector2(GetDollEdge(direction), myRt.anchoredPosition.y);
        else if (direction == Direction.Up || direction == Direction.Down)
            nextPos = new Vector2(myRt.anchoredPosition.x, GetDollEdge(direction));
        else 
            nextPos = myRt.anchoredPosition; 
        foreach (Soyjak jak in soyjaks)
        {
            if (IsPointInRT(nextPos, jak.GetComponent<RectTransform>()))
                return jak;
        }
        return null;
    }

    private void Move(Vector3 dir)
    {
        if (dir.x == 0 && dir.y == 0)
			return;
        myRt.anchoredPosition = new Vector2(myRt.anchoredPosition.x + dir.x, myRt.anchoredPosition.y + dir.y);
    }

    protected void FixedUpdate()
    {
		elapsedTime += Time.deltaTime;
        direction = Vector3.zero;
		if (elapsedTime < framerate) 
			return;
		elapsedTime = 0;
		Soyjak collidedWith = null;
		if (Input.GetKey(upKey) && WillDollBeInRect(inside, Direction.Up))
        {
            if ((collidedWith = CollideWithSoyjak(Direction.Up)) == null)
			    direction.y = +speed;
        }
		else if (Input.GetKey(downKey) && WillDollBeInRect(inside, Direction.Down))
        {
            if ((collidedWith = CollideWithSoyjak(Direction.Down)) == null)
			    direction.y = -speed;
        }
		else if (Input.GetKey(leftKey) && WillDollBeInRect(inside, Direction.Left))
		{
            if ((collidedWith = CollideWithSoyjak(Direction.Left)) == null)
			    direction.x = -speed;
            myRt.localScale = faceleft;
		}
		else if (Input.GetKey(rightKey) && WillDollBeInRect(inside, Direction.Right))
		{
            if ((collidedWith = CollideWithSoyjak(Direction.Right)) == null)
			    direction.x = +speed;
			myRt.localScale = faceRight;
		}
        Move(direction);
        if (collidedWith != null)
        {
            // Debug.Log("colliding with " + collidedWith.gameObject.name);
            collidedWith.GiveSoylent();
        }
    }
    public enum Direction 
    {
        None,
        Up,
        Down,
        Left,
        Right
    }
}
