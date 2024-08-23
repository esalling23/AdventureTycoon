using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private static Tooltip _tooltip;

	[SerializeField] private string _message = "";
	[SerializeField] private CanvasGroup _group;

	void Start()
	{
		if (_tooltip == null)
		{
			// _tooltip = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Tooltip")).GetComponent<Tooltip>();
			_tooltip = GameObject.FindGameObjectWithTag("MainTooltip").GetComponent<Tooltip>();
		}

		if (_group == null)
		{
			_group = gameObject.GetComponent<CanvasGroup>();
		}
	}

	void OnDisable()
	{
		HideTooltip();
	}

	void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus)
		{
			HideTooltip();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ShowTooltip();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HideTooltip();
	}

	public void SetMessage(string message)
	{
		_message = message;
	}
	
	public string GetMessage()
	{
		return _message;
	}

	private void ShowTooltip()
	{
		_tooltip.ShowMessage(_message);
	}

	private void HideTooltip()
	{
		_tooltip.Hide();
	}
}