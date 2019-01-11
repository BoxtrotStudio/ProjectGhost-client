using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu]
public class EntityManager : IManager {

	[Serializable]
	public struct QueuedSpawn
	{
		public short type;
		public short id;
		public float x;
		public float y;
		public float width;
		public float height;
		public float rotation;
		public string name;
		public bool hasLighting;
	}

	[Serializable]
	public struct EntityID
	{
		public GameObject prefab;
		public short id;
	}

	public EntityID[] prefabs;
	public CharacterList characters;
	
	[Header("This variable is optional. A default world will be used if left Missing")]
	public World world;

	public BattleManager Battle
	{
		get { return Game.Manager<BattleManager>(); }
	}
	
	private Dictionary<short, GameObject> prefabs_cache = new Dictionary<short, GameObject>();

	public static List<QueuedSpawn> QueuedSpawns = new List<QueuedSpawn>();
	
	public static void SpawnEntity(short type, short id, float x, float y, float width, float height, float rotation,
		string name, bool hasLighting)
	{
		var em = Game.Manager<EntityManager>();
		if (em != null)
		{
			Game.instance.InvokeOnNextUpdate(delegate
			{	
				em.Spawn(type, id, x, y, width, height, rotation, name, hasLighting);
			});
		}
		else
		{
			QueuedSpawns.Add(
				new QueuedSpawn
				{
					type = type,
					height = height,
					id = id,
					name = name,
					rotation = rotation,
					hasLighting = hasLighting,
					width = width,
					x = x,
					y = y
				}
			);
		}
	}
	
	void OnEnable()
	{
		if (prefabs == null)
			return;

		if (world == null)
		{
			world = CreateInstance<World>();
		}
		
		foreach (var id in prefabs)
		{
			prefabs_cache.Add(id.id, id.prefab);
		}
	}

	private Character FindCharacter(int type)
	{
		return characters.Value.FirstOrDefault(c => c.Id == type);
	}

	private GameObject CreateEntity(short type, short id, float x, float y, float width, float height, float rotation, string name)
	{
		var prefab = prefabs_cache[type];

		var obj = Instantiate(prefab, new Vector3(x, y), Quaternion.Euler(new Vector3(0f, 0f, rotation)));

		if (width != -1 && height != -1 && type != -3)
		{
			//TODO Set width and height (default is 100x100)
			obj.transform.localScale = new Vector3(width, height, obj.transform.localScale.z);
		}

		obj.name = name;

		obj.transform.Rotate(0f, 0f, Mathf.Rad2Deg * rotation);

		var lfps = obj.GetComponent<LoadFromPathSprite>();
		if (lfps != null)
		{
			lfps.spriteName = name;
		}

		world.AddEntityToGameObject(obj, id);

		return obj;
	}

	public Entity FindEntityById(short id)
	{
		if (id == 0)
		{
			id = Battle.info.SelfID;
		}

		return world[id];
	}

	public GameObject FindObjectById(short id)
	{
		if (id == 0)
		{
			id = Battle.info.SelfID;
		}

		return world.FindObjectById(id);
	}

	public void Spawn(short type, short id, float x, float y, float width, float height, float rotation,
		string name, bool hasLighting)
	{
		if (type == 0 || type == 1)
		{
			int ctype = type == 0 ? Battle.info.Allies[name] : Battle.info.Enemies[name];

			var character = FindCharacter(ctype);

			var prefab = character.Prefab.Value;
			
			var entity = Instantiate(prefab, new Vector3(x, y), Quaternion.Euler(new Vector3(0f, 0f, rotation)));

			entity.name = name;

			world.AddEntityToGameObject(entity, id);

			if (id == Battle.info.SelfID)
				entity.AddComponent<NetworkInput>();
		}
		else
		{
			var entity = CreateEntity(type, id, x, y, width, height, rotation, name);

			if (type == -3)
			{
				entity.transform.position += new Vector3(0f, 0f, -1f); //Move Z index up
			}
		}
	}

	public override void OnStart()
	{
		foreach (var q in QueuedSpawns)
		{
			Spawn(q.type, q.id, q.x, q.y, q.width, q.height, q.rotation, q.name, q.hasLighting);
		}
		
		QueuedSpawns.Clear();
	}

	public override void OnUpdate()
	{
	}

	public override void OnStop()
	{
	}

	public void Despawn(short id)
	{
		var gameObject = world.Delete(id);

		if (gameObject == null)
			return;
		
		Game.instance.InvokeOnNextUpdate(delegate
		{
			Destroy(gameObject);
		});
	}
}
