using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Editor : MonoBehaviour
{
	public GameObject MainMenuUI;
	public GameObject EditModeUI;
	public GameObject PlayModeUI;
	public TMP_Dropdown EnvironmentUI;
	public GameObject Block;
	public RectTransform GridUI;
	public GameObject GridLine;

	private static Editor _Instance = null;

	private Level _level = null;
	private int _blockId = 0;

	public static bool IsInstantiated()
	{
		return _Instance != null;
	}

	public static Editor Get()
	{
		return _Instance;
	}

	public bool IsEditing()
	{
		return _level != null && EditModeUI.activeSelf;
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

	public void OnPointerClick(BaseEventData eventData)
	{
		Vector2 position = (eventData as PointerEventData).position;
		RectTransform parentTrs = GridUI.parent as RectTransform;
		Vector2 scale = new Vector2(parentTrs.rect.width / Screen.width, parentTrs.rect.height / Screen.height);
		position *= scale;
		position -= GridUI.anchoredPosition;

		int x = Mathf.Clamp((int)(position.x * Settings.Width / GridUI.rect.width), 0, Settings.Width-1);
		int y = Mathf.Clamp((int)(position.y * Settings.Height / GridUI.rect.height), 0, Settings.Height - 1);
		_level.Grid[x, y] = _blockId;
		LevelManager.Get().LoadEnvironmentAndBlocks(_level, true);
	}

	private void OnBlock(int id)
	{
		_blockId = id;
	}

	void Awake()
	{
		_Instance = this;

		EditModeUI.SetActive(true);
		PlayModeUI.SetActive(false);

		List<string> environments = new List<string>();
		for (int i = 0; i < EnvironmentManager.Get().Environments.Length; ++i)
		{
			environments.Add(EnvironmentManager.Get().Environments[i].Node.name);
		}

		EnvironmentUI.AddOptions(environments);

		for (int i = 0; i < BlockManager.Get().Library.Length + 1; ++i)
		{
			GameObject block = Object.Instantiate<GameObject>(Block, EditModeUI.transform);
			RectTransform trs = block.GetComponent<RectTransform>();
			trs.anchoredPosition = new Vector2(trs.rect.width * i, 0.0f);

			string text = i == 0 ? "Empty" : BlockManager.Get().Library[i - 1].Node.name;
			block.GetComponentInChildren<TextMeshProUGUI>().text = text;

			int id = i;
			block.GetComponent<Button>().onClick.AddListener(delegate { OnBlock(id); });
		}

		CameraController.Get().SetEditorPosition();
		Camera camera = CameraController.Get().GetCamera();
		Vector2 minimum = camera.WorldToScreenPoint(Vector3.zero);
		Vector2 maximum = camera.WorldToScreenPoint(new Vector3(Settings.WorldWidth, Settings.WorldHeight, 0.0f));
		RectTransform parentTrs = GridUI.parent as RectTransform;
		Vector2 scale = new Vector2(parentTrs.rect.width / Screen.width, parentTrs.rect.height / Screen.height);
		minimum *= scale;
		maximum *= scale;
		GridUI.anchoredPosition = new Vector2(minimum.x, minimum.y);
		GridUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maximum.x - minimum.x);
		GridUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maximum.y - minimum.y);

		for (int j = 0; j < Settings.Height + 1; ++j)
		{
			GameObject line = Object.Instantiate<GameObject>(GridLine, GridUI);
			RectTransform lineTrs = line.transform as RectTransform;
			lineTrs.anchoredPosition = new Vector2(0.0f, j * GridUI.rect.height / Settings.Height);
			lineTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, GridUI.rect.width);
			lineTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1.0f);
		}

		for (int i = 0; i < Settings.Width + 1; ++i)
		{
			GameObject line = Object.Instantiate<GameObject>(GridLine, GridUI);
			RectTransform lineTrs = line.transform as RectTransform;
			lineTrs.anchoredPosition = new Vector2(i * GridUI.rect.width / Settings.Width, 0.0f);
			lineTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1.0f);
			lineTrs.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, GridUI.rect.height);
		}
	}

	private void OnEnable()
	{
		_level = new Level();
		LevelManager.Get().LoadEnvironmentAndBlocks(_level, true);
		_blockId = 0;
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
