using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TestingActionSounds))]
public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private CameraLookAt _lookAt;
    [SerializeField] private HorizontalLayoutGroup _actionsHorizontalLayout;
    [SerializeField] private GameObject _pfActionTestButton;
    private TestingActionSounds _actionSounds;

    private GameObject _selected;
    private Animator _targetAnimator;
    private List<TestButton> _testButtons;

    private void Awake() => _testButtons = new();

    private void Start()
    {
        _actionSounds = gameObject.GetComponent<TestingActionSounds>();
    }

    public void SelectVisual(GameObject targetVisual)
    {
        if(targetVisual != null)
        {
            if(_selected != null && _selected != targetVisual)
                _selected.SetActive(false);

            _selected = targetVisual;
        }
        else
            Debug.LogWarning("Boss not implemented yet!");
    }

    public void SelectTarget(Transform targetHips)
    {
        if(targetHips == null)
            return;

        _selected.SetActive(true);
        _targetAnimator = targetHips.GetComponentInParent<Animator>();

        if (_testButtons.Count > 0)
            ClearButtons();

        foreach (AnimationClip clip in _targetAnimator.runtimeAnimatorController.animationClips)
        {
            _testButtons.Add(CreateButton(clip));
        }

        _lookAt.Target = targetHips;
    }

    private void ClearButtons()
    {
        foreach(TestButton button in _testButtons)
            Destroy(button.gameObject);

        _testButtons.Clear();
    }

    private TestButton CreateButton(AnimationClip clip)
    {
        TestButton button = Instantiate(_pfActionTestButton, _actionsHorizontalLayout.transform).GetComponent<TestButton>();
        button.Initialize(clip.name, _targetAnimator, _actionSounds);
        return button;
    }

    public void SetTrigger(string trigger)
    {
        if (_targetAnimator != null)
        {
            _targetAnimator.SetTrigger(trigger);
            _actionSounds.PlaySound(trigger);
        }
        else
            Debug.LogWarning($"Couldn't set trigger \"{trigger}\" because no target is selected", this);
    }
}
