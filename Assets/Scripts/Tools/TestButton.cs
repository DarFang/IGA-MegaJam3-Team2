using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text _buttonTextTMP;

    private Animator _targetAnimator;

    public void Initialize(string name , Animator targetAnimator)
    {
        _buttonTextTMP.text = name;
        _targetAnimator = targetAnimator;
        Vector2 sizeDelta = ((RectTransform)transform).sizeDelta;
        sizeDelta.x = name.Length * 20;
        ((RectTransform)transform).sizeDelta = sizeDelta;
    }

    public void OnPointerClick(PointerEventData _) => _targetAnimator.SetTrigger(_buttonTextTMP.text);
}