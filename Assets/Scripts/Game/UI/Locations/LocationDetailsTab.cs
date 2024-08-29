using UnityEngine;
using UnityEngine.UI;

public class LocationDetailsTab : MonoBehaviour
{
	[SerializeField] private LocationDetailsPanel _detailsPanel;
	public TabType tabType = TabType.Activities;

	// To do - show/hide different UI to show that the tab is selected
	public Button tabButton;
	public Sprite tabButtonSpriteBase;
	public Sprite toggledTabButtonSprite;

	public void SetSelected(bool isSelected) {
		Sprite currentSprite = isSelected ? tabButtonSpriteBase : toggledTabButtonSprite;
		
		tabButton.GetComponent<Image>().sprite = currentSprite;
		gameObject.SetActive(isSelected);
	}

	public void HandleClickTab()
	{
		_detailsPanel.ToggleTab(tabType);
		SetSelected(true);
	}
}