using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls unloading and loading of scenes, call StartSwitchScene()
/// </summary>
public class SceneController : MonoBehaviour
{
	#region Static Parameters

	private static SceneController Instance;

	#endregion

	#region Public Events

	public UnityEvent OnCurrentSceneUnload = new();
	public UnityEvent OnSceneLoaded = new();

	#endregion

	#region Public Parameters

	/// <summary>
	/// Is a scene switch happening, if so lock all controls
	/// </summary> 
	public bool IsWorking { get; private set; } = false;

	/// <summary>
	/// The current scene type, if set to 0 (SceneManager) we can assume starting blacked out
	/// </summary>
	public SceneType CurrentScene { get; private set; } = SceneType.SceneManager;

	#endregion

	#region Private Fields

	/// <summary>
	/// The panel for fading to black (and unfading)
	/// </summary>
	[SerializeField]
	private Image transitionPanel;

	#endregion

	/// <summary>
	/// Set the SceneManager Singleton 
	/// </summary>
	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("[SceneManager] Already assigned! Overwriting", this);
		}
		Instance = this;
	}

	/// <summary>
	/// Load the main menu scene
	/// </summary>
	private void Start()
    {
		StartSceneSwitch(SceneType.MainMenu);
    }

	/// <summary>
	/// Fade to black, unload the current scene, then load the new scene and fade in
	/// NOTE: OnCurrentSceneUnload will be called directly before unload, after fade to black
	/// NOTE: OnSceneLoad will likely be called after Start but before fade from black
	/// </summary>
	/// <param name="targetSceneType"></param>
	public void StartSceneSwitch(SceneType targetSceneType)
	{
		if (IsWorking)
		{
			Debug.LogError("StartSceneSwitch() called when already working", this);
			return;
		}

		StartCoroutine(SwitchScene(targetSceneType));
	}

	private IEnumerator SwitchScene(SceneType targetSceneType)
    {
		// These are for logging :D
		string fromScene = Enum.GetName(typeof(SceneType), CurrentScene);
		string toScene = Enum.GetName(typeof(SceneType), targetSceneType);

		IsWorking = true;

		//Fade to black
		Debug.Log($"[SceneManager] Transitioning from {fromScene} to {toScene}");
		yield return StartTransition();

		//First load the new scene, so if it fails we can recover
		yield return AwaitAsyncOperation(SceneManager.LoadSceneAsync((int)targetSceneType));

		//Call OnCurrentSceneUnload and unload the current scene
		OnCurrentSceneUnload.Invoke();
		yield return AwaitAsyncOperation(SceneManager.UnloadSceneAsync((int)CurrentScene));

		//Now call OnSceneLoaded and we good
		CurrentScene = targetSceneType;
		OnSceneLoaded.Invoke();

		//Fade from black
		yield return EndTransition();
		Debug.Log($"[SceneManager] Transition complete, CurrentScene: {toScene}");

		IsWorking = false;
    }

	/// <summary>
	/// Yield while operation in progress then return when complete
	/// </summary>
	/// <param name="operation"></param>
	/// <returns></returns>
	private IEnumerator AwaitAsyncOperation(AsyncOperation operation)
	{
		//TODO: Exception handling
		while (!operation.isDone)
		{
			yield return null;
		}
	}

	/// <summary>
	/// Fade to black using dot tween
	/// </summary>
	/// <returns></returns>
	private IEnumerator StartTransition()
	{
		//TODO: Add DOTween
		transitionPanel.color = new Color(0, 0, 0, 1);
		yield return null;
	}

	/// <summary>
	/// Unfade from black using dot tween
	/// </summary>
	/// <returns></returns>
	private IEnumerator EndTransition()
	{
		//TODO: Add DOTween
		transitionPanel.color = new Color(0, 0, 0, 0);
		yield return null;
	}
}
