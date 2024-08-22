using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;


// Location GameObject that is displayed on the map
public class MapLocation : MapObject
{
    #region Fields

		private System.Guid _id = System.Guid.NewGuid();
		public Vector2Int coordinates;
		public List<MapActivity> activities = new();

		// To do - load data from database somewhere
		private Location _locationData;

		[SerializeField] private TMP_Text _hudName;
		[SerializeField] private TMP_Text _hudCounter;

		#endregion

		#region Properties

		public System.Guid Id { get { return _id; } }
		public Location LocationData { get { return _locationData; } }
		public LocationType Type { get { return _locationData.type; } }
		public Vector3 WorldPosition { get { 
			return gameObject.transform.position; 
		} }

		#endregion

		#region Methods

		public override void Start()
		{
			base.Start();
			EventManager.StartListening(EventName.OnActivityChanged, HandleOnActivityChanged);
		}

		private void OnDestroy() {
			EventManager.StopListening(EventName.OnActivityChanged, HandleOnActivityChanged);
		}

		public override string ToString()
		{
			return "Map Location ID " + _id.ToString();
		}

		public void SetData(Location data) {
			_locationData = data;
			
			LocationTypeData defaultData = DataManager.Instance.GetLocationTypeData(data.type);

			_spriteRenderer = GetComponent<SpriteRenderer>();
			if (_locationData.icon) {
				_spriteRenderer.sprite = _locationData.icon;
			} else {
				_spriteRenderer.sprite = defaultData.icon;
			}

			_hudName.text = data.name;
			_hudCounter.text = "0";

			_locationData.activitySlotCount = defaultData.baseActivitySlots;
		}

		public MapActivity[] GetAvailableActivities(Adventurer adventurer)
		{
			return activities.FindAll(activity => {
				// Don't repeat quests
				if (activity.Type == ActivityType.Quest && activity.AttemptLog.ContainsKey(adventurer.Id)) { return false; }
				
				// Can only use activities the adventurer has gold to use
				if (activity.data.CostToUse < adventurer.Gold) {
					if (!adventurer.NeedsRest && activity.Type == ActivityType.Rest) { return false; }
					
					return true;
				};

				return false;
			}).ToArray();
		}

    public bool SearchActivities(out MapActivity foundActivity, ActivityType type)
    {
			try {
				foundActivity = activities.First(a => a.data.Type == type);
				return true;
			}
			catch (System.Exception)
			{
				foundActivity = null;
				return false;
			}
    }

		public bool SearchActivities(out MapActivity foundActivity, ActivityType type, System.Guid adventurerId)
    {
			try {
				foundActivity = activities.First((a) => {
					return a.data.Type == type && !a.AttemptLog.ContainsKey(adventurerId);
				});
				return true;
			}
			catch(System.Exception)
			{
				foundActivity = null;
				return false;
			}
    }

		public MapActivity AddRandomActivity(bool isBaseActivity = false)
		{
			// Debug.Log($"Adding Activity to Location Type {location.type}");
			System.Guid[] activityDataIds = activities.Select(a => a.data.Id).ToArray();
			Activity[] unusedActivities = DataManager.Instance.WorldActivities.Where(a => {
				return !activityDataIds.Any(id => id == a.Id);
			}).ToArray();

			Activity randActivity;
			if (isBaseActivity)
			{
				LocationTypeData typeDefaults = DataManager.Instance.LocationTypeDefaults.FirstOrDefault(data => data.type == Type);
				ActivityType baseActivityType = typeDefaults.baseActivityType;

				randActivity = DataManager.Instance.GetRandomActivityData(unusedActivities, baseActivityType);
			}
			else
			{
				randActivity = DataManager.Instance.GetRandomActivityData(unusedActivities);
			}

			MapActivity activeActivity = new MapActivity(randActivity, this);
			activities.Add(activeActivity);

			EventManager.TriggerEvent(EventName.OnActivityChanged, new Dictionary<string, object>() {
				{ "type", activeActivity.Type }
			});

			return activeActivity;
		}

		public MapActivity AddRandomQuest()
		{
			// Debug.Log($"Adding Quest to {Type} Location");

			// LocationTypeData typeDefaults = DataManager.Instance.LocationTypeDefaults.FirstOrDefault(data => data.type == location.type);
			// ActivityType baseActivityType = typeDefaults.baseActivityType;

			Quest quest = DataManager.Instance.GetRandomQuestData();
			MapActivity activeActivity = new(quest, this);

			activities.Add(activeActivity);

			EventManager.TriggerEvent(EventName.OnActivityChanged, new Dictionary<string, object>() {
				{ "type", ActivityType.Quest }
			});

			return activeActivity;
		}

		// public void RemoveActivity(Activity activity) {
		// 	Debug.Log("Removing Map Location Activity");
		// 	activities = activities.Where(act => act.name != activity.name).ToList();

		// 	EventManager.TriggerEvent(EventName.OnActivityChanged, null);
		// }

		private void HandleOnActivityChanged(Dictionary<string, object> data)
		{
			int totalAdventurers = 0;

			foreach (MapActivity activity in activities)
			{
				totalAdventurers += activity.adventurersPresent.Count;
			}

			_hudCounter.text = totalAdventurers.ToString();
		}

		#endregion
}
