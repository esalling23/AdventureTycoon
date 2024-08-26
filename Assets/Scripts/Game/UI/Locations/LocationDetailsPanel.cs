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

		private List<ActivityListItem> _listItems = new();
		private MapLocation _activeLocation;

		public ActivityListItem activityListItemPrefab;

		public TabType lastTabOpen = TabType.Activities;

		public GameObject activitiesListContainer;
		public LocationDetailsTab activitiesTab;
		public GameObject activitiesActions;

		public GameObject questListContainer;
		public LocationDetailsTab questTab;

		private ActivityType[] _activityTypes = new ActivityType[] { ActivityType.PassTime, ActivityType.Trade, ActivityType.Rest };
		public GameObject activitiesTypeShelf;
		public ActivityTypeItem activitiesTypeItemPrefab;

		#endregion

		#region Properties

		#endregion

		#region Methods

    void Start()
    {
			gameObject.SetActive(false);
			
			SetupActivitiesShelf();

			EventManager.StartListening(EventName.OnActivityChanged, HandleOnActivityChanged);
			EventManager.StartListening(EventName.OnActivityTypeSelected, HandleOnActivityTypeSelected);
			EventManager.StartListening(EventName.OnLocationChanged, HandleOnLocationChanged);
    }

		void OnDestroy()
		{
			EventManager.StopListening(EventName.OnActivityChanged, HandleOnActivityChanged);
			EventManager.StopListening(EventName.OnActivityTypeSelected, HandleOnActivityTypeSelected);
			EventManager.StopListening(EventName.OnLocationChanged, HandleOnLocationChanged);
    }

		private void SetupActivitiesShelf()
		{
			activitiesTypeShelf.SetActive(false);

			foreach(ActivityType type in _activityTypes)
			{
				ActivityTypeData data = DataManager.Instance.GetActivityTypeData(type);

				ActivityTypeItem item = Instantiate(
					activitiesTypeItemPrefab,
					Vector3.zero,
					Quaternion.identity,
					activitiesTypeShelf.transform
				);

				item.SetData(data);
			}

		}

		public void ToggleOpen(bool isOpen) 
		{
			this.gameObject.SetActive(isOpen);
			if (isOpen && _activeLocation != null)
			{
				RefreshList(TabType.Activities);
				RefreshList(TabType.Quests);

				ToggleTab(lastTabOpen);
			}
			else
			{
				_activeLocation = null;
			}
		}

		public void SetLocationData(MapLocation location) {
			_activeLocation = location;

			SetData(location.LocationData);
		}

		private void RefreshList(TabType tabType) {
			if (!_activeLocation) return;
			
			// Debug.Log($"Refreshing list of {tabType}");
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

			_listItems = new();
			// to do - don't delete all, just replace data & remove extras
			foreach(Transform child in container.transform)
			{
				Destroy(child.gameObject);
			}

			activitiesActions.SetActive(!_activeLocation.IsActivitySlotsFull);

			for(int i = 0; i < _activeLocation.ActivitySlotCount; i++) {
				ActivityListItem item = Instantiate(
					activityListItemPrefab,
					Vector3.zero,
					Quaternion.identity,
					container.transform
				);
				if (i >= list.Count)
				{
					item.DisplayFilled(false);
				}
				else
				{
					item.SetData(list[i]);
				}

				_listItems.Add(item);
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
			activitiesTypeShelf.SetActive(true);
		}

		public void HandleClickAddQuest() {
			_activeLocation.AddRandomQuest();
		}

		#region Event Handlers

		private void HandleOnActivityTypeSelected(Dictionary<string, object> data) {
			if (data.TryGetValue("type", out object activityType))
			{
				System.Enum.TryParse(activityType.ToString(), out ActivityType type);
				
				_activeLocation.AddRandomActivity(type);
				activitiesTypeShelf.SetActive(false);

				RefreshList(lastTabOpen);
			}
		}

		private void HandleOnActivityChanged(Dictionary<string, object> data) {
			if (data.TryGetValue("type", out object activityType))
			{
				System.Enum.TryParse(activityType.ToString(), out ActivityType type);
				
				if (type == ActivityType.Quest)
				{
					RefreshList(TabType.Quests);
				}
				else
				{
					RefreshList(TabType.Activities);
				}
			}
			else
			{
				RefreshList(lastTabOpen);
			}
		}

		private void HandleOnLocationChanged(Dictionary<string, object> data) {
			RefreshList(lastTabOpen);
		}

		#endregion

		#endregion
}
