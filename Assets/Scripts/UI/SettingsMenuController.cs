using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Creates and saves a SettingsObject as JSON to PlayerPrefs
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
	private Slider masterSlider;

	[SerializeField]
	private TMP_Text masterText;

	[SerializeField]
	private Slider musicSlider;

	[SerializeField]
	private TMP_Text musicText;

	[SerializeField]
	private Slider soundSlider;

	[SerializeField]
	private TMP_Text soundText;

	private BookMenuController referer;
	private SettingsObject settings;
	private bool blockSaving = false;

	/// <summary>
	/// Grab settings from PlayerPrefs/default
	/// </summary>
	private void Awake()
	{
		string settingsJSON = PlayerPrefs.GetString("settings", string.Empty);
		settings = new SettingsObject();
		if (settingsJSON == string.Empty)
		{
			return;
		}
		JsonUtility.FromJsonOverwrite(settingsJSON, settings);
	}

	/// <summary>
	/// Populate settings menu, and register callbacks
	/// </summary>
	private void Start()
	{
		masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
		musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
		soundSlider.onValueChanged.AddListener(OnSoundVolumeChanged);

		blockSaving = true;

		masterSlider.value = settings.masterVolume;
		musicSlider.value = settings.musicVolume;
		soundSlider.value = settings.soundVolume;

		blockSaving = false;
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

	public void OnMasterVolumeChanged(float linearVolume)
	{
		settings.masterVolume = linearVolume;
		SetVolume(linearVolume, "MasterVolume");
		masterText.text = Mathf.Round(linearVolume * 100).ToString() + "%";
		SaveSettings();
	}

	public void OnMusicVolumeChanged(float linearVolume)
	{
		settings.musicVolume = linearVolume;
		SetVolume(linearVolume, "MusicVolume");
		musicText.text = Mathf.Round(linearVolume * 100).ToString() + "%";
		SaveSettings();
	}

	public void OnSoundVolumeChanged(float linearVolume)
	{
		settings.soundVolume = linearVolume;
		SetVolume(linearVolume, "SFXVolume");
		soundText.text = Mathf.Round(linearVolume * 100).ToString() + "%";
		SaveSettings();
	}

	private void SetVolume(float linearVolume, string mixerVariable)
	{
		//clamp the value so it never reaches zero to prevent bugs
		linearVolume = Mathf.Clamp(linearVolume, 0.00001f, 1);
		audioMixer.SetFloat(mixerVariable, Mathf.Log10(linearVolume) * 20f);
	}

	private void SaveSettings()
	{
		if (blockSaving)
		{
			return;
		}
		PlayerPrefs.SetString("settings", JsonUtility.ToJson(settings));
	}
}
