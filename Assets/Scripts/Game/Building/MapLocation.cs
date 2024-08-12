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

		public List<MapActivity> activities = new List<MapActivity>();

		// To do - load data from database somewhere
		private Location _locationData;

		private SpriteRenderer _spriteRenderer;

		#endregion

		#region Properties

		public System.Guid Id { get { return _id; } }
		public Location LocationData { get { return _locationData; } }

		#endregion

		#region Methods

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

    public MapActivity SearchActivities(ActivityType type)
    {
			MapActivity found;
			// try {
				found = activities.First(a => a.activityData.type == type);
				return found;
			// }
			// catch(Error)
			// {
			// 	return null;
			// }
    }

		// to do - make this work. currently not doing anything
		public void SetSpriteSize(float width, float height) {
			if (!_spriteRenderer) {
				_spriteRenderer = GetComponent<SpriteRenderer>();
				_spriteRenderer.sprite = _locationData.icon;
			}
			Debug.Log(_spriteRenderer.size);
			_spriteRenderer.size = new Vector3(width, height, 0f);
			Debug.Log(_spriteRenderer.size);
		}

		public MapActivity AddRandomActivity() {
			Debug.Log("Adding Map Location Activity");
			Activity randActivity = DataManager.Instance.GetRandomActivityData();
			MapActivity activeActivity = new MapActivity(randActivity, this);

			activities.Add(activeActivity);

			EventManager.TriggerEvent(EventName.OnActivityChanged, null);

			return activeActivity;
		}

		// public void RemoveActivity(Activity activity) {
		// 	Debug.Log("Removing Map Location Activity");
		// 	activities = activities.Where(act => act.name != activity.name).ToList();

		// 	EventManager.TriggerEvent(EventName.OnActivityChanged, null);
		// }

		#endregion
}
