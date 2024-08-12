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

		#endregion

		#region Properties

		public DataManager Instance { get { return _instance; }}

		public LocationTypeData[] LocationTypeDefaults { get { return _locationTypeDatas; } }
		public ActivityTypeData[] ActivityTypeDefaults { get { return _activityTypeDatas; } }
		
		public Activity[] WorldActivities { get { return _worldActivityData; } }
		public Location[] WorldLocations { get { return _worldLocationData; } }

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
			Debug.Log(data);
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
			Debug.Log(data);
			return data;
		}

		public Activity GetActivityData(int index) 
		{
			return _worldActivityData[index];
		}

		public Activity GetRandomActivityData()
		{
			int index = Random.Range(0, _worldActivityData.Length);
			return GetActivityData(index);
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

		#endregion
}
