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

		private List<ActivityListItem> _activityItems = new();
		private List<VIPListItem> _vipItems = new();
		private MapLocation _activeLocation;

		public VIPListItem vipListItemPrefab;
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

			ClearList(activitiesListContainer);
			ClearList(questListContainer);
			
			SetupActivitiesShelf();

			EventManager.StartListening(EventName.OnActivityChanged, HandleOnActivityChanged);
			EventManager.StartListening(EventName.OnActivityTypeSelected, HandleOnActivityTypeSelected);
			EventManager.StartListening(EventName.OnLocationChanged, HandleOnLocationChanged);
			EventManager.StartListening(EventName.OnVIPLeft, HandleOnVIPLeft);
    }

		void OnDestroy()
		{
			EventManager.StopListening(EventName.OnActivityChanged, HandleOnActivityChanged);
			EventManager.StopListening(EventName.OnActivityTypeSelected, HandleOnActivityTypeSelected);
			EventManager.StopListening(EventName.OnLocationChanged, HandleOnLocationChanged);
			EventManager.StopListening(EventName.OnVIPLeft, HandleOnVIPLeft);
    }

		private void SetupActivitiesShelf()
		{
			activitiesTypeShelf.SetActive(false);
			ClearList(activitiesTypeShelf);

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

		private void RefreshLastTab()
		{
			RefreshList(lastTabOpen);
		}

		private void RefreshList(TabType tabType) {
			if (!_activeLocation) return;
			
			if (tabType == TabType.Quests)
			{
				RefreshVIPs();
			}
			else
			{
				RefreshActivities();
			}
		}

		private void ClearList(GameObject container)
		{
			// to do - don't delete all, just replace data & remove extras
			foreach(Transform child in container.transform)
			{
				Destroy(child.gameObject);
			}
		}

		private void RefreshVIPs()
		{
			List<MapVIP> vipList = _activeLocation.vips;

			if (_vipItems.Count > vipList.Count)
			{
				for (int i = 0; i < _vipItems.Count - vipList.Count; i++)
				{
					Destroy(_vipItems[vipList.Count + i].gameObject);
				}
				// delete unneeded objects from list
				_vipItems.RemoveRange(vipList.Count, _vipItems.Count - vipList.Count);
			}
			else if (_vipItems.Count < vipList.Count)
			{
				// instantiate new ones if we have fewer items than VIPs
				for (int i = 0; i < vipList.Count - _vipItems.Count; i++)
				{
					VIPListItem item = Instantiate(
						vipListItemPrefab,
						Vector3.zero,
						Quaternion.identity,
						questListContainer.transform
					);

					_vipItems.Add(item);
				}
			}

			// update each item
			for (int i = 0; i < _vipItems.Count; i++)
			{
				_vipItems[i].SetData(vipList[i]);
			}
		}
		private void RefreshActivities()
		{
			List<MapActivity> activityList = _activeLocation.activities;

			activitiesActions.SetActive(!_activeLocation.IsActivitySlotsFull);

			if (_activityItems.Count > _activeLocation.ActivitySlotCount)
			{
				for (int i = 0; i < _activityItems.Count - _activeLocation.ActivitySlotCount; i++)
				{
					Destroy(_activityItems[_activeLocation.ActivitySlotCount + i].gameObject);
				}
				// delete unneeded objects from list
				_activityItems.RemoveRange(activityList.Count, _activityItems.Count - _activeLocation.ActivitySlotCount);
			}
			else if (_activityItems.Count < _activeLocation.ActivitySlotCount)
			{
				// instantiate new ones if we have fewer items than activities
				for (int i = 0; i < _activeLocation.ActivitySlotCount - _activityItems.Count; i++)
				{
					ActivityListItem item = Instantiate(
						activityListItemPrefab,
						Vector3.zero,
						Quaternion.identity,
						activitiesListContainer.transform
					);

					_activityItems.Add(item);
				}
			}
		
			for(int i = 0; i < _activityItems.Count; i++) {
				ActivityListItem item = _activityItems[i];
				if (i >= activityList.Count)
				{
					item.DisplayFilled(false);
				}
				else
				{
					item.SetData(activityList[i]);
				}
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

				RefreshLastTab();
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
				RefreshLastTab();
			}
		}

		private void HandleOnLocationChanged(Dictionary<string, object> data) {
			RefreshLastTab();
		}

		private void HandleOnVIPLeft(Dictionary<string, object> data)
		{
			RefreshLastTab();
		}

		#endregion

		#endregion
}
