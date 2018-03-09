using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFactory : MonoBehaviour {

	[Serializable]
	public struct EntityID
	{
		public GameObject prefab;
		public short id;
	}

	public EntityID[] prefabs;
	
	private Dictionary<short, GameObject> prefabs_cache = new Dictionary<short, GameObject>();
	// Use this for initialization
	void Start () {
		foreach (var id in prefabs)
		{
			prefabs_cache.Add(id.id, id.prefab);
		}
	}

	public GameObject createEntity(short type, short id, float x, float y, float width, float height, float rotation,
		String name)
	{
		var prefab = prefabs_cache[type];

		var obj = Instantiate(prefab, new Vector3(x, y), Quaternion.Euler(new Vector3(0f, 0f, rotation)));

		if (width != -1 && height != -1)
		{
			//TODO Set width and height (default is 100x100)
		}

		obj.name = name;

		var lfps = obj.GetComponent<LoadFromPathSprite>();
		if (lfps != null)
		{
			lfps.spriteName = name;
		}

		return obj;
	}
}
