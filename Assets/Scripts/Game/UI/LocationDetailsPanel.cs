using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public enum TabType {
	Activities,
	Quests
}

public class LocationDetailsPanel : LocationDetailItem
{
    #region Fields

		private MapLocation _activeLocation;

		public ActivityListItem activityListItemPrefab;

		public TabType lastTabOpen = TabType.Activities;

		public GameObject activitiesListContainer;
		public DetailsTab activitiesTab;
		// public GameObject activitiesPanel;

		public GameObject questListContainer;
		public DetailsTab questTab;
		// public GameObject questPanel;

		#endregion

		#region Properties

		#endregion

		#region Methods

    void Start()
    {
			gameObject.SetActive(false);

			EventManager.StartListening(EventName.OnActivityChanged, HandleActivityChanged);
    }

		public void ToggleOpen(bool isOpen) 
		{
			this.gameObject.SetActive(isOpen);
			ToggleTab(lastTabOpen);
		}

		public void HandleActivityChanged(Dictionary<string, object> _data = null) {
			DisplayActivities();
		}

		public void SetLocationData(MapLocation location) {
			_activeLocation = location;

			SetData(location.LocationData);

			DisplayActivities();
		}

		private void DisplayActivities() {
			// to do - don't delete all, just replace data & remove extras
			foreach(Transform child in activitiesListContainer.transform)
			{
				Destroy(child.gameObject);
			}

			Debug.Log(_activeLocation.activities);

			foreach(MapActivity activity in _activeLocation.activities) {
				ActivityListItem item = Instantiate(
					activityListItemPrefab,
					Vector3.zero,
					Quaternion.identity,
					activitiesListContainer.transform
				);

				item.SetData(activity);
			}
		}

		public void ToggleTab(TabType type)
		{
			if (type == TabType.Activities)
			{
				questTab.SetSelected(false);
			}
			else if (type == TabType.Quests)
			{
				activitiesTab.SetSelected(false);
			}

			lastTabOpen = type;
		}

		public void HandleClickAddActivity() {
			// to do - show a generated set of activities & allow user to pick one
			_activeLocation.AddRandomActivity();
		}

		#endregion
}
