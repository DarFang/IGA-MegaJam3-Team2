using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text _buttonTextTMP;

    private Animator _targetAnimator;
    private TestingActionSounds _actionSounds;

    public void Initialize(string name , Animator targetAnimator, TestingActionSounds sounds)
    {
        _buttonTextTMP.text = name;
        _targetAnimator = targetAnimator;
        _actionSounds = sounds;
        Vector2 sizeDelta = ((RectTransform)transform).sizeDelta;
        sizeDelta.x = name.Length * 20;
        ((RectTransform)transform).sizeDelta = sizeDelta;
    }

    public void OnPointerClick(PointerEventData _) => Click();

    private void Click()
    {
        _targetAnimator.SetTrigger(_buttonTextTMP.text);
        _actionSounds.PlaySound(_buttonTextTMP.text);
    }
}