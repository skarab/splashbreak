using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[System.Serializable]
public struct Environment
{
    public GameObject Node;
}

public class EnvironmentManager : MonoBehaviour
{
    public Environment[] Environments;
    public float PaniniDistance = 0.259f;

    private static EnvironmentManager _Instance = null;
    
    private int _id = -1;
    private bool _bypassEffects = false;

    public static EnvironmentManager Get()
	{
        return _Instance;
	}

    public void LoadEnvironment(int id, bool bypassEffects)
	{
        Environments[id].Node.SetActive(true);
        _id = id;
        _bypassEffects = bypassEffects;
    }

    public void UnloadEnvironment(int id)
    {
        Environments[id].Node.SetActive(false);
        _id = -1;
    }

    void Awake()
    {        
        _Instance = this;
    }

    
    void Update()
    {
        if (_id!=-1)
        { 
            Volume[] volumes = Environments[_id].Node.GetComponentsInChildren<Volume>(true);
            for (int i = 0; i < volumes.Length; ++i)
            {
                PaniniProjection panini;
                if (volumes[i].profile.TryGet<PaniniProjection>(out panini))
                {
                    panini.distance.value = Mathf.Lerp(panini.distance.value, _bypassEffects?0.0f:PaniniDistance, Time.deltaTime * 4.0f);
                }
            }
        }
    }
}
