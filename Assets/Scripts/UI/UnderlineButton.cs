using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnderlineButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Button button;

    [SerializeField]
    private TextMeshProUGUI text;

	public void OnPointerEnter(PointerEventData eventData)
	{
		text.fontStyle = text.fontStyle | FontStyles.Underline;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		text.fontStyle = text.fontStyle & ~FontStyles.Underline;
	}
}
