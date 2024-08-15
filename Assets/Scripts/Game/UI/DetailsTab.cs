using UnityEngine;
using UnityEngine.UI;

public class DetailsTab : MonoBehaviour
{
	[SerializeField] private LocationDetailsPanel _detailsPanel;
	public TabType tabType = TabType.Activities;

	private bool _isSelected = false;

	// To do - show/hide different UI to show that the tab is selected
	public Button tabButton;
	// public GameObject tabContainer;

	public Sprite tabSprite;
	public Sprite tabButtonSpriteBase;
	public Sprite toggledTabButtonSprite;

	public void SetSelected(bool isSelected) {
		_isSelected = isSelected;
		// change UI

		this.gameObject.SetActive(isSelected);
	}

	public void HandleClickTab()
	{
		_detailsPanel.ToggleTab(tabType);
		SetSelected(true);
	}
}