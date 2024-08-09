using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class GameManager : MonoBehaviour
{
    #region Fields

		private int _gold;

		private GameMode _mode = GameMode.Run;

		// to do - load from json/database
		[SerializeField] private ActivityTypeData[] _activityTypeDatas;
		[SerializeField] private LocationTypeData[] _locationTypeDatas;

		#endregion

		#region Properties

		public int Gold { get { return _gold; } }
		public GameMode Mode { get { return _mode; } }
		public LocationTypeData[] LocationTypeDatas { get { return _locationTypeDatas; } }
		public ActivityTypeData[] ActivityTypeDatas { get { return _activityTypeDatas; } }

		#endregion

		#region Methods

		public LocationTypeData GetLocationTypeData(LocationType type) {
			LocationTypeData data;
			try
			{
				data = LocationTypeDatas.First(loc => loc.type == type);
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
				data = ActivityTypeDatas.First(act => act.type == type);
			}
			catch (System.InvalidOperationException)
			{
				data = new ActivityTypeData();
			}
			Debug.Log(data);
			return data;
		}

		public void SetMode(GameMode mode) {
			_mode = mode;
		}

		#endregion
}
