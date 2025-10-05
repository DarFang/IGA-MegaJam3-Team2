using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenuController : BookMenuController
{
	public UnityEvent onResume = new();

	/// <summary>
	/// Resume the game
	/// </summary>
	public void ResumeButton()
	{
		onResume.Invoke();
	}

	/// <summary>
	/// Quit to main menu
	/// </summary>
	public void QuitButton()
	{
		SceneController.Instance.StartSceneSwitch(SceneType.MainMenu);
	}

	public void SetBookUpState(bool value)
	{
		bookAnimator.SetBool("book_up", value);
	}
}
