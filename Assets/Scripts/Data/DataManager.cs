using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataManager : MonoBehaviour
{
    #region Fields

		private static DataManager _instance;

		// to do - load from json/database
		[Header("Base settings for activities, locations based on type")]
		[SerializeField] private ActivityTypeData[] _activityTypeDatas;
		[SerializeField] private LocationTypeData[] _locationTypeDatas;

		[Header("All possible activities, locations")]
		[SerializeField] private Activity[] _worldActivityData;
		[SerializeField] private Location[] _worldLocationData;
		[SerializeField] private Quest[] _worldQuestData;

		#endregion

		#region Properties

		public static DataManager Instance { get { return _instance; }}

		public LocationTypeData[] LocationTypeDefaults { get { return _locationTypeDatas; } }
		public ActivityTypeData[] ActivityTypeDefaults { get { return _activityTypeDatas; } }
		
		public Activity[] WorldActivities { get { return _worldActivityData; } }
		public Location[] WorldLocations { get { return _worldLocationData; } }
		public Quest[] WorldQuests { get { return _worldQuestData; } }

		#endregion

		#region Methods

		/// <summary>
    /// Manages singleton wakeup/destruction
    /// </summary>
    private void Awake()
    {
        // Singleton management
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


		public void LoadData(Data data) 
		{
			_worldLocationData = data.locations;
			_locationTypeDatas = data.locationTypeDefaults;
			_worldActivityData = data.activities;
			_activityTypeDatas = data.activityTypeDefaults;
			_worldQuestData = data.quests;
		}

		public LocationTypeData GetLocationTypeData(LocationType type) {
			LocationTypeData data;
			try
			{
				data = LocationTypeDefaults.First(loc => loc.type == type);
			}
			catch (System.InvalidOperationException)
			{
				data = new LocationTypeData();
			}
			// Debug.Log(data.type);
			return data;
		}

		public ActivityTypeData GetActivityTypeData(ActivityType type) {
			ActivityTypeData data;
			try
			{
				data = ActivityTypeDefaults.First(act => act.type == type);
			}
			catch (System.InvalidOperationException)
			{
				data = new ActivityTypeData();
			}
			// Debug.Log(data);
			return data;
		}

		public Activity GetActivityData(Activity[] activities, int index) 
		{
			return activities[index];
		}

		public Activity GetRandomActivityData(Activity[] activities)
		{
			if (activities.Length == 0) {
				activities = WorldActivities;
			}
			int index = Random.Range(0, activities.Length);
			return GetActivityData(activities, index);
		}

		public Activity GetRandomActivityData(Activity[] activities, ActivityType type)
		{
			Activity[] available = System.Array.FindAll(activities, a => a.type == type);
			if (available.Length == 0) {
				available = WorldActivities;
			}
			int index = Random.Range(0, available.Length);
			return GetActivityData(activities, index);
		}

		public Activity GetRandomActivityData(Location location)
		{
			Activity[] available = System.Array.FindAll(_worldActivityData, a => {
				return System.Array.IndexOf(a.appearsInLocationTypes, location.type) > -1;
			});
			if (available.Length == 0) {
				available = WorldActivities;
			}
			int index = Random.Range(0, available.Length);
			return GetActivityData(available, index);
		}

		public Location GetLocationData(int index) 
		{
			return _worldLocationData[index];
		}

		public Location GetRandomLocationData()
		{
			int index = Random.Range(0, _worldLocationData.Length);
			return GetLocationData(index);
		}

		public Location GetRandomLocationData(LocationType type)
		{
			Location[] available = System.Array.FindAll(_worldLocationData, loc => loc.type == type);
			int index = Random.Range(0, available.Length);
			return GetLocationData(index);
		}

		public Quest GetQuestData(int index) 
		{
			return _worldQuestData[index];
		}

		public Quest GetRandomQuestData()
		{
			int index = Random.Range(0, _worldQuestData.Length);
			return GetQuestData(index);
		}

		#endregion
}
