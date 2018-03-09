using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectSlowField : MonoBehaviour
{

	public int width, height;

	public GameObject leaf;
	// Use this for initialization
	void Start ()
	{
		var direction = Vector3.right;
		if (width > height) {
			if (Random.Range(0, 100) < 50)
			{
				direction = Vector3.left;
			}
		} else if (height > width) {
			if (Random.Range(0, 100) < 50)
			{
				direction = Vector3.up;
			}
			else
			{
				direction = Vector3.down;
			}
		} 
		else 
		{
			if (Random.Range(0, 100) < 50)
			{
				direction = Vector3.left;
			}
		}

		var minBounds = new Vector3(0f, 0f);
		var maxBounds = new Vector3(0f, 0f);

		minBounds.x = transform.position.x - (width / 2f);
		minBounds.y = transform.position.y - (height / 2f);

		maxBounds.x = transform.position.x + (width / 2f);
		maxBounds.y = (transform.position.y) + (height / 2f);

		var count = Random.Range(10, 20);

		for (int i = 0; i < count; i++)
		{
			//TODO Spawn leaf prefab

			var newLeaf = Instantiate(leaf);
			var leafLogic = newLeaf.GetComponent<Leaf>();

			leafLogic.direction = direction;
			leafLogic.minBounds = minBounds;
			leafLogic.maxBounds = maxBounds;
		}
	}
}
