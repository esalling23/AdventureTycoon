using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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

		public void HandleActivityChanged(Dictionary<string, object> data) {
			if (data.TryGetValue("type", out object activityType))
			{
				System.Enum.TryParse(activityType.ToString(), out TabType type);
				RefreshList(type);
			}
		}

		public void SetLocationData(MapLocation location) {
			_activeLocation = location;

			SetData(location.LocationData);

			RefreshList(TabType.Activities);
			RefreshList(TabType.Quests);
		}

		private void RefreshList(TabType tabType) {
			Debug.Log($"Displaying list of {tabType}");
			List<MapActivity> list = _activeLocation.activities
				.Where((MapActivity obj) => {
					if (tabType == TabType.Quests)
					{
						return obj.data is Quest;
					}
					return obj.data is Activity;
				}).ToList();

			GameObject container = activitiesListContainer;
			if (tabType == TabType.Quests)
			{
				container = questListContainer;
			}

			// to do - don't delete all, just replace data & remove extras
			foreach(Transform child in container.transform)
			{
				Destroy(child.gameObject);
			}

			Debug.Log(list.Count);

			foreach(MapActivity activity in list) {
				ActivityListItem item = Instantiate(
					activityListItemPrefab,
					Vector3.zero,
					Quaternion.identity,
					container.transform
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
			_activeLocation.AddRandomActivity(false);
		}

		public void HandleClickAddQuest() {
			// to do - show a generated set of activities & allow user to pick one
			_activeLocation.AddRandomQuest();
		}

		#endregion
}
