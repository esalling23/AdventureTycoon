using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapVIP : MonoBehaviour 
{
    #region Fields

		private System.Guid _id = System.Guid.NewGuid();

		private VIP _vip;
		// private int _level;

		private int _age = 0;
		private MapLocation _currentLocation;

		private List<MapActivity> _mapQuests = new();

		#endregion

		#region Properties

		public System.Guid Id { get { return _id; } }

		public VIP Data { get { return _vip; } }
		public List<MapActivity> MapQuests { get { return _mapQuests; } }
		public int Age { get { return _age; } }


		#endregion

		#region Methods

		void Start()
		{
			EventManager.StartListening(EventName.OnDayChanged, HandleOnDayChanged);
		}
		void OnDestroy()
		{
			EventManager.StopListening(EventName.OnDayChanged, HandleOnDayChanged);
		}

		public void Init(VIP vip, MapLocation location)
		{
			_vip = vip;
			_age = 0;
			_currentLocation = location;

			foreach (Quest quest in _vip.quests)
			{
				MapActivity activity = new(quest, location);
				_mapQuests.Add(activity);
			}
		}

		void HandleOnDayChanged(Dictionary<string, object> data)
		{			
			_age++;

			if (_age >= _vip.lifetime)
			{
				// kick out adventurers?
				EventManager.TriggerEvent(EventName.OnVIPLeft, new Dictionary<string, object>() {
					{ "vipId", Id },
					{ "activities", _mapQuests }
				});

				MessageManager.Instance.ShowMessage($"{_vip.name} has left the map");
				
				Destroy(this);
			}
		}

		#endregion
}
