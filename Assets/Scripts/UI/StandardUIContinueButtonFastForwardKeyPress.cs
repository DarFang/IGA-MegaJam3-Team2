using PixelCrushers.DialogueSystem.Wrappers;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Pixel Crushers/Dialogue System/UI/Standard UI/UI Effects/Standard UI Continue Button Fast Forward Key Press")]
public class StandardUIContinueButtonFastForwardKeyPress : StandardUIContinueButtonFastForward
{
	[SerializeField]
	private Button _button;
	private InputSystem_Actions _inputActions;

	private void Awake()
	{
		_inputActions = new();
		_inputActions.Enable();
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

		if (_inputActions.Player.Jump.WasPressedThisFrame())
		{
			_button.onClick?.Invoke();
		}
	}
}
