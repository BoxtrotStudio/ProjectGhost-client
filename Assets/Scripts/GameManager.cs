
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }


    public GameObject FindObjectById(short id)
    {
        throw new System.NotImplementedException();
    }
}