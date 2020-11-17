using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Editor : MonoBehaviour
{
    public GameObject MainMenuUI;
    public GameObject EditModeUI;
    public GameObject PlayModeUI;
    public TMP_Dropdown EnvironmentUI;
    public GameObject Block;

    private static Editor _Instance = null;
    private Level _level = null;
    
    public static Editor Get()
	{
        return _Instance;
	}

    public bool IsEditing()
	{
        return _level!=null && EditModeUI.activeSelf;
    }

    public void NotifyEndLevel(bool win)
	{
        OnStop();
    }

    public void OnClose()
	{
        gameObject.SetActive(false);
	}

    public void OnPlay()
	{
        EditModeUI.SetActive(false);
        PlayModeUI.SetActive(true);
        LevelManager.Get().LoadLevel(_level);
    }

	public void OnStop()
	{
        EditModeUI.SetActive(true);
        PlayModeUI.SetActive(false);
        LevelManager.Get().LoadEnvironmentAndBlocks(_level, true);
    }

    public void OnRandomize()
	{
        _level.Randomize();
        LevelManager.Get().LoadEnvironmentAndBlocks(_level, true);
    }

    public void OnSetEnvironment(int environment)
	{
        _level.EnvironmentID = EnvironmentUI.value;
        LevelManager.Get().LoadEnvironmentAndBlocks(_level, true);
    }

    void Awake()
    {
        _Instance = this;

        EditModeUI.SetActive(true);
        PlayModeUI.SetActive(false);

        List<string> environments = new List<string>();
        for (int i=0 ; i<EnvironmentManager.Get().Environments.Length ; ++i)
        {
            environments.Add(EnvironmentManager.Get().Environments[i].Node.name);
        }
        
        EnvironmentUI.AddOptions(environments);

        for (int i=0 ; i<BlockManager.Get().Library.Length + 1 ; ++i)
		{
            GameObject block = Object.Instantiate<GameObject>(Block, EditModeUI.transform);
            RectTransform trs = block.GetComponent<RectTransform>();
            trs.anchoredPosition = new Vector2(trs.rect.width*i, 0.0f);

            string text = i==0?"Empty": BlockManager.Get().Library[i-1].Node.name;
            block.GetComponentInChildren<TextMeshProUGUI>().text = text;

            int id = i;
            block.GetComponent<Button>().onClick.AddListener(delegate { OnBlock(id); });
        }
    }

    private void OnBlock(int id)
	{
        Debug.Log(id);
	}

    private void OnEnable()
	{
        _level = new Level();
		LevelManager.Get().LoadEnvironmentAndBlocks(_level, true);
    }

	private void OnDisable()
	{
        _level = null;
        LevelManager.Get().UnloadLevel();
        MainMenuUI.SetActive(true);
    }

	void Update()
    {        
    }
}
