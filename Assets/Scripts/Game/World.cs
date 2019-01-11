using System;
using System.Collections.Generic;
using UnityEngine;

public class World : ScriptableObject
{
    public EntityDataDictionary EntityDatas = new EntityDataDictionary();
    private Dictionary<short, GameObject> entityObjects = new Dictionary<short, GameObject>();
    private Dictionary<short, Entity> entities = new Dictionary<short, Entity>();

    public Entity this[short id]
    {
        get
        {
            if (!entities.ContainsKey(id))
                return null;
            
            return entities[id];
        }
    }

    public Entity AddEntityToGameObject(GameObject obj, short id)
    {
        var entity = obj.GetComponent<Entity>();
        if (entity == null)
            entity = obj.AddComponent<Entity>();

        var entityData = CreateInstance<EntityData>();
        entity.data = entityData;
        
        //Assign some defaults
        entity.data.position = obj.transform.position;
        entity.data.alpha = 255f;
        entity.data.id = id;
        
        entityObjects.Add(id, obj);
        entities.Add(id, entity);
        EntityDatas.Add(id, entityData);

        return entity;
    }
    
    public GameObject FindObjectById(short id)
    {	
        return entityObjects.ContainsKey(id) ? entityObjects[id] : null;
    }

    public GameObject Delete(short id)
    {
        if (!entityObjects.ContainsKey(id))
            return null;
        
        var obj = entityObjects[id];
        
        entities.Remove(id);
        entityObjects.Remove(id);
        EntityDatas.Remove(id);

        return obj;
    }
}

[Serializable]
public class EntityDataDictionary : SerializableDictionary<short, EntityData>
{
}