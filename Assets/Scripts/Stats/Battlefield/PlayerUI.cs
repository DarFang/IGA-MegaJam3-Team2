using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
    [SerializeField] private Button attackButton;
    [SerializeField] private Button defenseButton;
    [SerializeField] private Button manaButton;
    [SerializeField] private Button healButton;

    public event Action<PlayerAction> OnActionSelected;

    private void Start() {
        attackButton?.onClick.AddListener(() => OnActionSelected?.Invoke(PlayerAction.Attack));
        defenseButton?.onClick.AddListener(() => OnActionSelected?.Invoke(PlayerAction.Defense));
        manaButton?.onClick.AddListener(() => OnActionSelected?.Invoke(PlayerAction.Mana));
        healButton?.onClick.AddListener(() => OnActionSelected?.Invoke(PlayerAction.Heal));

        SetButtonsInteractable(false);
    }

    public void SetButtonsInteractable(bool interactable) {
        if (attackButton != null) attackButton.interactable = interactable;
        if (defenseButton != null) defenseButton.interactable = interactable;
        if (manaButton != null) manaButton.interactable = interactable;
        if (healButton != null) healButton.interactable = interactable;
    }
}