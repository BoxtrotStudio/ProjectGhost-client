
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game instance;

    private List<Action> runNextUpdate = new List<Action>();
    private List<Action> toAddNextUpdate = new List<Action>();

    public ManagerList Source;
    
    public List<IManager> Managers
    {
        get { return Source.Managers; }
    }
    private bool isUpdating;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        foreach (var manager in Managers)
        {
            manager.OnStart();
        }
    }

    void Update()
    {
        isUpdating = true;
        foreach (var a in runNextUpdate)
        {
            if (a == null) continue;
            
            a();
        }
        
        runNextUpdate.Clear();
        isUpdating = false;
        
        runNextUpdate.AddRange(toAddNextUpdate);
        toAddNextUpdate.Clear();
        
        foreach (var manager in Managers)
        {
            manager.OnUpdate();
        }
    }

    void OnDestroy()
    {
        foreach (var manager in Managers)
        {
            manager.OnStop();
        }
    }

    public void InvokeOnNextUpdate(Action action)
    {
        if (isUpdating)
        {
            toAddNextUpdate.Add(action);
        }
        else
        {
            runNextUpdate.Add(action);
        }
    }

    public static T Manager<T>() where T : IManager
    {
        return instance.GetManager<T>();
    }

    public T GetManager<T>() where T : IManager
    {
        return Managers.OfType<T>().FirstOrDefault();
    }


    public Entity FindObjectById(short id)
    {
        return GetManager<EntityManager>().FindEntityById(id);
    }
}