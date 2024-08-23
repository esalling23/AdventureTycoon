using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
	private Canvas _parentCanvas;

	private float _mouseOffset = 25;

	[SerializeField] private TMP_Text _messageText;

	private RectTransform _rect;
	
	public bool Active
	{
		get
		{
			return gameObject.activeSelf;
		}
	}

	void Start()
	{
		_parentCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
		transform.SetParent(_parentCanvas.transform);
		
		_rect = transform as RectTransform;
	}

	void LateUpdate()
	{
		Vector2 pos = Input.mousePosition;
		Rect screen = _parentCanvas.GetComponent<RectTransform>().rect;

		// Offset from the moue
		pos.x += _mouseOffset;
		pos.y -= _mouseOffset;

		// Flip if necessary
		if (pos.x + _rect.rect.size.x > screen.width / 2f)
		{
			pos.x -= _rect.rect.size.x;
		}
		if (pos.y - _rect.rect.size.y < -screen.height / 2f)
		{
			pos.y += _rect.rect.size.y;
		}

		transform.position = _parentCanvas.transform.TransformPoint(pos);
		transform.SetAsLastSibling();
	}
	
	private void Update()
	{
		_rect.anchoredPosition = Input.mousePosition;
	}

	public void ShowMessage(string message)
	{
		_messageText.text = message;

		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}