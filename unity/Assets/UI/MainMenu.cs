using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject EditorUI;
    
    public void OnLogin()
    {
    }

    public void OnEditor()
	{
        gameObject.SetActive(false);
        EditorUI.SetActive(true);
	}

    void Start()
    {        
    }

    void Update()
    {        
    }
}
