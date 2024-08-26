using UnityEngine;
using TMPro;

public class Stat : MonoBehaviour
{
	[SerializeField] private TMP_Text _text;
	// [SerializeField] private Image _icon;

	public void SetStat(bool isVisible, string value)
	{
		gameObject.SetActive(isVisible);
		_text.text = value;
	}
}