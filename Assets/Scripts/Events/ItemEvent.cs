using UnityEngine;
using UnityEngine.Events;

public class ItemEvent : Event
{
    #region Serialized Properties

    [field: SerializeField] public override UnityEvent OnEventStart { get; set; }
    [field: SerializeField] public override UnityEvent OnEventEnd { get; set; }
    [SerializeField] private ItemData _itemToAdd;

    #endregion

    /// <summary>
    /// Call this method to start the item event.
    /// </summary>
    public override void StartEvent()
    {
        if (_itemToAdd != null)
        {
            Debug.Log($"Item event started!");
            Debug.Log($"Press (E) to add {_itemToAdd.itemName} to inventory.");
            // TODO: Implement item event start logic
            OnEventStart?.Invoke();
        }
        else
            Debug.LogError($"No item assigned to {name}.", this);
    }
    public override void EndEvent()
    {
        if(_itemToAdd != null)
        {
            Debug.Log($"Item event ended! Adding {_itemToAdd.itemName} to inventory...");
            // TODO: Implement item event end logic
            OnEventEnd?.Invoke();
        }
        else
            Debug.LogError($"No item assigned to {name}.", this);
    }

    public override string GetType() => "Item event";
}