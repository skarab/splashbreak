using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Environment
{
    public GameObject Node;
}

public class EnvironmentManager : MonoBehaviour
{
    public Environment[] Environments;

    private static EnvironmentManager _Instance = null;

    public static EnvironmentManager Get()
	{
        return _Instance;
	}

    public void LoadEnvironment(int id)
	{
        Environments[id].Node.SetActive(true);
    }

    public void UnloadEnvironment(int id)
    {
        Environments[id].Node.SetActive(false);
    }

    void Awake()
    {        
        _Instance = this;
    }

    
    void Update()
    {        
    }
}
