using UnityEngine;

public abstract class BookMenuController : MonoBehaviour
{
	[SerializeField]
	protected CanvasGroup canvasGroup;

	[SerializeField]
	protected Animator bookAnimator;

	[SerializeField]
	protected SettingsMenuController settingsMenu;

	public void EnableMenu()
	{
		canvasGroup.interactable = true;
	}

	/// <summary>
	/// - Disable interactions with these buttons, 
	/// - Set book_open to true in animater
	/// - Enable settings controller
	/// </summary>
	public void SettingsButton()
	{
		bookAnimator.SetBool("book_open", true);
		canvasGroup.interactable = false;
		settingsMenu.EnableSettings(this);
	}
}