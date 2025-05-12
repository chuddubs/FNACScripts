using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DollSchizo : ImageCollider
{
	public float framerate;
	public float speed;
	private float elapsedTime;
	private Vector3 direction = new Vector3();

    private KeyCode upKey;
    private KeyCode downKey;
    private KeyCode leftKey;
    private KeyCode rightKey;

    private void Awake()
    {
        upKey = SoySettings.Instance.upKey;
        downKey = SoySettings.Instance.downKey;
        leftKey = SoySettings.Instance.leftKey;
        rightKey = SoySettings.Instance.rightKey;
    }
	
    protected void FixedUpdate()
    {
		elapsedTime += Time.deltaTime;
        direction = Vector3.zero;
		if (elapsedTime < framerate) 
			return;
		elapsedTime = 0;
		
		RectTransform mainCollider = GetMainCollider();
		
		if (Input.GetKey(upKey))
        {
			direction.y = +speed;
        }
		else if (Input.GetKey(downKey))
        {
			direction.y = -speed;
        }
		
		if (Input.GetKey(leftKey))
		{
			direction.x = -speed;
            mainCollider.localScale = new Vector3(-0.5f, 0.5f, 1);
		}
		else if (Input.GetKey(rightKey))
		{
			direction.x = +speed;
			mainCollider.localScale = new Vector3(+0.5f, 0.5f, 1);
		}
		
        RectTransform collidedWith = base.Move(direction);
		// if (collidedWith != null)
		// {
		// 	Debug.Log("Collided with: " + collidedWith);
		// }
    }
   
}
