using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : BookMenuController
{
    /// <summary>
    /// Start scene switch to game
    /// </summary>
    public void BeginButton()
    {
        SceneController.Instance.StartSceneSwitch(SceneType.Level);
    }

    /// <summary>
    /// Start scene switch to credits
    /// </summary>
    public void CreditsButton()
    {
		SceneController.Instance.StartSceneSwitch(SceneType.Credits);
	}

    /// <summary>
    /// - Set book_up to false in animater
    /// - Wait for animation to finish + half a second
    /// - Close the game
    /// </summary>
    public void QuitButton()
    {
        bookAnimator.SetBool("book_up", false);
        StartCoroutine(WaitForClose());
    }

    private IEnumerator WaitForClose()
    {
        yield return new WaitForSecondsRealtime(1.5f);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
