using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// PlayerPrefs settings values are:
/// - setResolution (string)
/// - setFullscreen (boolean)
/// - setMasterVol (float 0-1)
/// - setMusicVol (float 0-1)
/// - setSoundVol (float 0-1)
/// </summary>
public class SettingsMenuController : MonoBehaviour
{
	[SerializeField]
	private Animator bookAnimator;

	[SerializeField]
	private CanvasGroup[] canvasGroups;

	[SerializeField]
	private AudioMixer audioMixer;

	[SerializeField]
	private TMP_Dropdown resolutionDropdown;

	[SerializeField]
	private Toggle fullscreenToggle;

	[SerializeField]
	private Slider masterSlider;

	[SerializeField]
	private Slider musicSlider;

	[SerializeField]
	private Slider soundSlider;

	private BookMenuController referer;

	/// <summary>
	/// Apply settings from PlayerPrefs/default
	/// </summary>
	private void Awake()
	{
		
	}

	/// <summary>
	/// Populate settings menu and register callbacks
	/// </summary>
	private void Start()
	{
			
	}

	public void EnableSettings(BookMenuController referer)
	{
		this.referer = referer;
		foreach (CanvasGroup group in canvasGroups) 
		{
			group.interactable = true;
		}
	}

	public void Back()
	{
		foreach (CanvasGroup group in canvasGroups)
		{
			bookAnimator.SetBool("book_open", false);
			group.interactable = true;
		}
		referer.EnableMenu();
	}
}
