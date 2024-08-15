using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


// Location GameObject that is displayed on the map
public class MapLocation : MapObject
{
    #region Fields

		private System.Guid _id = System.Guid.NewGuid();
		private GameManager _manager;

		public Vector2Int coordinates;
		public List<MapActivity> activities = new List<MapActivity>();

		// To do - load data from database somewhere
		private Location _locationData;

		#endregion

		#region Properties

		public System.Guid Id { get { return _id; } }
		public Location LocationData { get { return _locationData; } }
		public LocationType Type { get { return _locationData.type; } }
		public Vector3 WorldPosition { get { 
			Debug.Log(gameObject.transform.position);
			return gameObject.transform.position; 
		} }

		#endregion

		#region Methods

		public override string ToString()
		{
			return "Map Location ID " + _id.ToString();
		}

		public void SetData(Location data) {
			_locationData = data;

			_manager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
			
			LocationTypeData defaultData = DataManager.Instance.GetLocationTypeData(data.type);

			_spriteRenderer = GetComponent<SpriteRenderer>();
			if (_locationData.icon) {
				_spriteRenderer.sprite = _locationData.icon;
			} else {
				_spriteRenderer.sprite = defaultData.icon;
			}

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
			catch(System.Exception err)
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
			catch(System.Exception err)
			{
				foundActivity = null;
				return false;
			}
    }

		public MapActivity AddRandomActivity(bool isBaseActivity)
		{
			// Debug.Log($"Adding Activity to Location Type {location.type}");
			Activity randActivity;
			if (isBaseActivity)
			{
				LocationTypeData typeDefaults = DataManager.Instance.LocationTypeDefaults.FirstOrDefault(data => data.type == Type);
				ActivityType baseActivityType = typeDefaults.baseActivityType;

				randActivity = DataManager.Instance.GetRandomActivityData(baseActivityType);
			}
			else
			{
				randActivity = DataManager.Instance.GetRandomActivityData();
			}

			MapActivity activeActivity = new MapActivity(randActivity, this);
			activities.Add(activeActivity);

			EventManager.TriggerEvent(EventName.OnActivityChanged, new Dictionary<string, object>() {
				{ "type", TabType.Activities }
			});

			return activeActivity;
		}

		public MapActivity AddRandomQuest()
		{
			Debug.Log($"Adding Quest to {Type} Location");

			// LocationTypeData typeDefaults = DataManager.Instance.LocationTypeDefaults.FirstOrDefault(data => data.type == location.type);
			// ActivityType baseActivityType = typeDefaults.baseActivityType;

			Quest quest = DataManager.Instance.GetRandomQuestData();
			MapActivity activeActivity = new MapActivity((IActivity) quest, this);

			activities.Add(activeActivity);

			EventManager.TriggerEvent(EventName.OnActivityChanged, new Dictionary<string, object>() {
				{ "type", TabType.Quests }
			});

			return activeActivity;
		}

		// public void RemoveActivity(Activity activity) {
		// 	Debug.Log("Removing Map Location Activity");
		// 	activities = activities.Where(act => act.name != activity.name).ToList();

		// 	EventManager.TriggerEvent(EventName.OnActivityChanged, null);
		// }

		#endregion
}
