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

		/// <summary>
		/// Updates a list of game objects in the scene from a list of data it should reflect
		/// </summary>
		/// <param name="objectList">The list of game objects</param>
		/// <param name="dataList">The list of data</param>
		/// <typeparam name="T">The type of data in the dataList</typeparam>
		/// <returns></returns>
		public List<ObjectType> UpdateGameObjectListFromData<ObjectType, DataType>(
			List<ObjectType> objectList, 
			List<DataType> dataList, 
			ObjectType prefab,
			Transform parentContainer
		) where ObjectType : MonoBehaviour
		{
			while (objectList.Count > dataList.Count)
			{
				Destroy(objectList.Last().gameObject);
				objectList.RemoveAt(objectList.Count - 1);
			}

			while (objectList.Count < dataList.Count)
			{
				ObjectType item = Instantiate(
					prefab,
					Vector3.zero,
					Quaternion.identity,
					parentContainer
				);

				objectList.Add(item);
			}
			return objectList;
		}

		private void RefreshVIPs()
		{
			List<MapVIP> vipList = _activeLocation.vips;

			_vipItems = UpdateGameObjectListFromData(
				_vipItems, 
				vipList,
				vipListItemPrefab,
				questListContainer.transform
			);

			for (int i = 0; i < _vipItems.Count; i++)
			{
				_vipItems[i].SetData(vipList[i]);
			}
		}
		private void RefreshActivities()
		{
			List<MapActivity> activityList = _activeLocation.activities;

			activitiesActions.SetActive(!_activeLocation.IsActivitySlotsFull);

			_activityItems = UpdateGameObjectListFromData(
				_activityItems, 
				activityList,
				activityListItemPrefab,
				activitiesListContainer.transform
			);
		
			for(int i = 0; i < _activeLocation.ActivitySlotCount; i++) {
				// Add Empty Slots
				if (i >= _activityItems.Count)
				{
					ActivityListItem item = Instantiate(
						activityListItemPrefab,
						Vector3.zero,
						Quaternion.identity,
						activitiesListContainer.transform
					);

					_activityItems.Add(item);
					item.DisplayFilled(false);
				}
				else
				{
					_activityItems[i].SetData(activityList[i]);
				}
			}
		}

		public void ToggleTab(TabType type)
		{
			RefreshList(type);
			
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
			if (_activeLocation == null) return;

			if (data.TryGetValue("event", out object activityEvent))
			{
				System.Enum.TryParse(activityEvent.ToString(), out ActivityChangeEvent evt);
				
				// Update is handled by ActivityListItem
				if (evt == ActivityChangeEvent.Update) return;

				RefreshLastTab();
			}
		}

		private void HandleOnLocationChanged(Dictionary<string, object> data) {
			RefreshLastTab();
		}

		#endregion

		#endregion
}
