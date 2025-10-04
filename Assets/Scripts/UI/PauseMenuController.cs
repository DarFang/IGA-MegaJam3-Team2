using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class PauseMenuController : BookMenuController
{
	/// <summary>
	/// Resume the game
	/// </summary>
	public void ResumeButton()
	{

	}

	/// <summary>
	/// Quit to main menu
	/// </summary>
	public void QuitButton()
	{
		SceneController.Instance.StartSceneSwitch(SceneType.MainMenu);
	}
}
