using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PauseMenuBridge : MonoBehaviour
{
	private InputSystem_Actions _inputActions;

	[SerializeField]
	[Tooltip("Please ensure that the blur canvas is first :)")]
	private CanvasGroup[] canvases;

	[SerializeField]
	private PauseMenuController pauseController;

	[SerializeField]
	private GameObject bookObject;

	public bool isPaused = false;
	public bool isWorking = false;

	private void Awake()
	{
		_inputActions = new();
		_inputActions.Enable();
	}

	private void Start()
	{
		pauseController.onResume.AddListener(() => StartCoroutine(TogglePauseMenu()));
		pauseController.SetBookUpState(false);
	}

	private void OnDestroy()
	{
		_inputActions?.Disable();
		_inputActions?.Dispose();
	}

	private void Update()
	{
		if (_inputActions == null)
		{
			return;
		}

		if (_inputActions.Player.Pause.WasPressedThisFrame())
		{
			StartCoroutine(TogglePauseMenu());
		}
	}

	public IEnumerator TogglePauseMenu()
	{
		if (isWorking)
		{
			yield break;
		}

		isWorking = true;
		isPaused = !isPaused;
		
		foreach (CanvasGroup group in canvases)
		{
			group.interactable = isPaused;
			group.blocksRaycasts = isPaused;
			group.DOFade(isPaused ? 1 : 0, 1);
		}

		pauseController.SetBookUpState(isPaused);

		//TODO: Clean this
		if (isPaused)
		{
			bookObject.SetActive(isPaused);
		}

		yield return new WaitForSecondsRealtime(1f);

		//TODO: Clean this
		if (!isPaused)
		{
			bookObject.SetActive(isPaused);
		}

		isWorking = false;
	}
}
