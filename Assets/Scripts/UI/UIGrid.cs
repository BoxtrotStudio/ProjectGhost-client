using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGrid : MonoBehaviour
{

	public float speed;
	
	private List<GameObject> Grids = new List<GameObject>();
	private SpriteRenderer currentObject, nextObject;
	private int currentIndex = -1;
	private float t = 0f;
	
	// Use this for initialization
	void Start () {
		foreach (Transform child in transform)
		{
			Grids.Add(child.gameObject);
			child.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
		}

		NextGrid();
	}
	
	// Update is called once per frame
	void Update ()
	{
		float ca =  Mathf.Lerp(1f, 0f, t);
		float na = Mathf.Lerp(0f, 1f, t);
		currentObject.color = new Color(1f, 1f, 1f, ca);
		nextObject.color = new Color(1f, 1f, 1f, na);

		t += speed * Time.deltaTime;

		if (t >= 1f)
		{
			NextGrid();
		}
	}

	private void NextGrid()
	{
		currentIndex++;

		if (currentIndex >= Grids.Count)
		{
			currentIndex = 0;
		}

		int nextIndex = (currentIndex + 1 >= Grids.Count ? 0 : currentIndex + 1);

		currentObject = Grids[currentIndex].GetComponent<SpriteRenderer>();
		nextObject = Grids[nextIndex].GetComponent<SpriteRenderer>();
		t = 0f;
		
		currentObject.color = new Color(1f, 1f, 1f, 1f);
	}
}
