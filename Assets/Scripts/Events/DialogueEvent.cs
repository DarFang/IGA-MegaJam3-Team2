using PixelCrushers.DialogueSystem;
using System;
using UnityEngine;
using UnityEngine.Events;

public class DialogueEvent : Event
{
	[field: SerializeField] public override UnityEvent OnEventStart { get; set; }
	[field: SerializeField] public override UnityEvent OnEventEnd { get; set; }

	[SerializeField]
	private DialogueSystemEvents _dialogueSystemEvents;

	[SerializeField]
	private string _conversationName;

	private UnityAction<Transform> endConversationAction;

	public override void StartEvent()
	{
		endConversationAction = (transform) => EndEvent();
		_dialogueSystemEvents.conversationEvents.onConversationEnd.AddListener(endConversationAction);
		DialogueManager.StartConversation(_conversationName);
		OnEventStart?.Invoke();
		Debug.Log($"Started dialogue event with conversation {_conversationName}");
	}

	public override void EndEvent()
	{
		_dialogueSystemEvents.conversationEvents.onConversationEnd.RemoveListener(endConversationAction);
		OnEventEnd?.Invoke();
		Debug.Log("Ended dialogue event");
	}

	public override string GetType() => "Dialogue Event";
}
