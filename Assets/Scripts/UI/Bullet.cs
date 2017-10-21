using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class Bullet : MonoBehaviour
{
	private Vector3 origin, originScale;
	public Vector3 speed = new Vector3(0f, 0f, 2.5f);
	
	// Use this for initialization
	void Start ()
	{
		origin = transform.position;
		originScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(speed);
	}

	public void DoTheThing(GameObject obj)
	{
		
		transform.position = obj.transform.position;

		if (obj.name == "Exit" || obj.name == "Options")
		{
			transform.localScale = originScale / 2f;
		}
		else
		{
			transform.localScale = originScale;
		}
	}

	public void UndoTheThing()
	{
		transform.position = origin;
	}
}
