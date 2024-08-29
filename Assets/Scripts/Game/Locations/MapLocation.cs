using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ActivityRewardType
{
	None,
	Reroll,
	Remove,
	Slot
}


// Location GameObject that is displayed on the map
public class MapLocation : MapObject
{
    #region Fields

		private System.Guid _id = System.Guid.NewGuid();
		public Vector2Int coordinates;
		public List<MapActivity> activities = new();
		public List<MapVIP> vips = new();
		private int _slotCount = 1;
		private int _activityRemovesAvailable = 0;
		private int _activityRollsAvailable = 0;

		private Dictionary<int, ActivityRewardType> _levelRewards = new() {
			{ 5, ActivityRewardType.Slot },
			{ 10, ActivityRewardType.Slot },
			{ 15, ActivityRewardType.Reroll },
			{ 20, ActivityRewardType.Remove },
			{ 30, ActivityRewardType.Slot },
			{ 35, ActivityRewardType.Reroll },
			{ 45, ActivityRewardType.Slot },
			{ 60, ActivityRewardType.Reroll },
			{ 70, ActivityRewardType.Remove },
			{ 90, ActivityRewardType.Slot },
			{ 100, ActivityRewardType.Slot },
		};

		// To do - load data from database somewhere
		private Location _locationData;
		private int _daysAlive = 0;

		[SerializeField] private LocationHUD _hud;

		#endregion

		#region Properties

		public System.Guid Id { get { return _id; } }
		public Location LocationData { get { return _locationData; } }
		public LocationType Type { get { return _locationData.type; } }
		public Vector3 WorldPosition { get { 
			return gameObject.transform.position; 
		} }

		public int ActivitySlotCount { get { return _slotCount; } }
		public bool IsActivitySlotsFull { get { 
			return activities.Where(a => a.data is Activity).ToList().Count >= _slotCount; 
		} }
		public int ActivityRollsAvailable { get { return _activityRollsAvailable; } }
		public int ActivityRemovesAvailable { get { return _activityRemovesAvailable; } }

		public int AvailableActions { get {
			return ActivityRemovesAvailable + ActivityRollsAvailable + (_slotCount - activities.Where(a => a.data is Activity).ToList().Count);
		}}

		public int TotalAdventurerCount { get {
			int totalAdventurers = activities.Sum(activity => activity.adventurersPresent.Count);
			return totalAdventurers;
		}}

		public MapActivity[] Quests { get { 
			MapActivity[] quests = vips.SelectMany(v => v.MapQuests).ToArray();
			return quests;
		}}

		#endregion

		#region Methods

		public override void Start()
		{
			base.Start();
			EventManager.StartListening(EventName.OnActivityChanged, HandleOnActivityChanged);
			EventManager.StartListening(EventName.OnDayChanged, HandleOnDayChanged);
		}

		private void OnDestroy() {
			EventManager.StopListening(EventName.OnActivityChanged, HandleOnActivityChanged);
			EventManager.StopListening(EventName.OnDayChanged, HandleOnDayChanged);
		}

		public override string ToString()
		{
			return "Map Location ID " + _id.ToString();
		}

		public void SetData(Location data, GridCell cell) {
			_locationData = data;
			this.cell = cell;
			
			LocationTypeData defaultData = DataManager.Instance.GetLocationTypeData(data.type);

			_spriteRenderer = GetComponent<SpriteRenderer>();
			if (data.icon) {
				_spriteRenderer.sprite = data.icon;
			} else {
				_spriteRenderer.sprite = defaultData.icon;
			}

			_slotCount = data.activitySlotCount > 0 ? data.activitySlotCount : defaultData.baseActivitySlots;
		
			_hud.SetData(this);
		}

		private Activity[] GetUnusedActivities()
		{
			System.Guid[] activityDataIds = activities.Select(a => a.data.Id).ToArray();
			Activity[] unusedActivities = DataManager.Instance.WorldActivities.Where(a => {
				return !activityDataIds.Any(id => id == a.Id);
			}).ToArray();
			return unusedActivities;
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

		/// <summary>
		/// Gets an unused random activity for this location based on ActivityType
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public Activity GetNewRandomActivity(ActivityType type)
		{
			// Debug.Log($"Adding Activity to Location Type {type}");
			Activity[] unusedActivities = GetUnusedActivities();
			Activity randActivity = DataManager.Instance.GetRandomActivityData(unusedActivities, type);

			return randActivity;
		}
		public MapActivity AddRandomActivity(bool isBaseActivity = false)
		{
			if (IsActivitySlotsFull) return null;

			// Debug.Log($"Adding Activity to Location Type {location.type}");
			Activity[] unusedActivities = GetUnusedActivities();

			IActivity randActivity;
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

			return CreateMapActivity(randActivity);
		}

		public MapActivity AddRandomActivity(ActivityType type)
		{
			if (IsActivitySlotsFull) return null;	
			// Debug.Log($"Adding Activity to Location Type {type}");
			Activity[] unusedActivities = GetUnusedActivities();

			Activity randActivity = DataManager.Instance.GetRandomActivityData(unusedActivities, type, LocationData);

			return CreateMapActivity(randActivity);
		}

		private MapActivity CreateMapActivity(IActivity activity)
		{
			if (IsActivitySlotsFull) return null;
			MapActivity activeActivity = new(activity, this);

			activities.Add(activeActivity);

			EventManager.TriggerEvent(EventName.OnActivityChanged, new Dictionary<string, object>() {
				{ "id", activeActivity.Id },
				{ "event", ActivityChangeEvent.Create }
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
				{ "id", activeActivity.Id },
				{ "event", ActivityChangeEvent.Create }
			});

			return activeActivity;
		}

		public void RemoveActivity(System.Guid mapActivityId) {
			Debug.Log("Removing Map Location Activity");
			MapActivity mapActivity = activities.First(m => m.Id == mapActivityId);
			activities.Remove(mapActivity);

			EventManager.TriggerEvent(EventName.OnActivityChanged, new Dictionary<string, object>() {
				{ "id", mapActivity.Id },
				{ "event", ActivityChangeEvent.Delete }
			});
		}

		public void AddActivityReroll()
		{
			_activityRollsAvailable++;
		}
		public void AddActivitySlot()
		{
			_slotCount++;
		}

		public void AddActivityRemove()
		{
			_activityRemovesAvailable++;
		}

		#region Event Handlers

		private void HandleOnActivityChanged(Dictionary<string, object> data)
		{
			_hud.Refresh();
		}

		private void HandleOnDayChanged(Dictionary<string, object> data)
		{
			_daysAlive++;
			int rewardInterval = Mathf.FloorToInt(Mathf.Sqrt(_daysAlive) * 5);

			if (_daysAlive % rewardInterval != 0) return; // No reward for this level

			// Determine reward type based on weighted probability
			int chance = Random.Range(1, 101);

			if (chance <= 50) AddActivitySlot();
			else if (chance <= 80) AddActivityReroll();
			else AddActivityRemove();

			EventManager.TriggerEvent(EventName.OnLocationChanged, new Dictionary<string, object>() {});

			MessageManager.Instance.ShowMessage($"Location Rewards Available at {_locationData.name}");
			// to do - on click of message, pan to location & select
		}
		
		#endregion
		#endregion
}
