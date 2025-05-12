using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ImageAnimation : MonoBehaviour {

	public Sprite[] sprites;
	public float framerate = 1f / 16f;
	public bool loop = true;
	public bool destroyOnEnd = false;
	public bool restartOnEnable = true;

	private Image image;
	private int index;
	private float elapsedTime;

	void Awake() {
		image = GetComponent<Image> ();
		index = 0;
		elapsedTime = 0;
	}

	void Update () {
		if (!loop && index == sprites.Length)
		{
			return;
		}
		
		elapsedTime += Time.deltaTime;
		if (elapsedTime < framerate) 
		{
			return;
		}
		elapsedTime = 0;
		
		image.sprite = sprites [index];
		index ++;
		if (index >= sprites.Length) 
		{
			if (loop) index = 0;
			if (destroyOnEnd) Destroy (gameObject);
		}
	}
	
	void OnDisable()
    {
        
    }

    void OnEnable()
    {
		if (restartOnEnable)
		{
			index = 0;
			elapsedTime = 0;
		}
		index = Mathf.Clamp(index, 0, sprites.Length - 1);
		image.sprite = sprites [index];
    }
	
	public bool IsLastFrame()
	{
		return index >= sprites.Length;
	}

}