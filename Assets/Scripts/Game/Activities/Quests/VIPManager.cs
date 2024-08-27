using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class VIPManager : MonoBehaviour
{
    #region Fields

		private static VIPManager _instance;

		[SerializeField] private MapVIP _mapVIPPrefab;
		private int _lastVIPDay = 1;
		
		private List<VIP> unusedVIPs = new();

		[SerializeField] private VIPDisplay _vipDisplay;
		private VIP _currentAvailableVIP;

		#endregion

		#region Properties

		public static VIPManager Instance { get { return _instance; }}
		
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

		void Start()
		{
			unusedVIPs = DataManager.Instance.WorldVIPs.ToList();
			EventManager.StartListening(EventName.OnDayChanged, HandleOnDayChanged);
		}
		void OnDestroy()
		{
			EventManager.StopListening(EventName.OnDayChanged, HandleOnDayChanged);
		}

		void CreateVIP()
		{
			VIP vip = Utils.GetRandomFromList(unusedVIPs);

			_currentAvailableVIP = vip;

			_vipDisplay.ShowVIP(_currentAvailableVIP);
		}

		public void RejectVIP()
		{
			_currentAvailableVIP = null;
		}

		public void PlaceCurrentVIP(MapLocation location)
		{
			if (_currentAvailableVIP == null) return;

			MapVIP newVIP = Instantiate(
				_mapVIPPrefab,
				Vector3.zero,
				Quaternion.identity,
				location.transform
			);
			unusedVIPs.Remove(_currentAvailableVIP);
			newVIP.Init(_currentAvailableVIP, location);
			location.vips.Add(newVIP);

			EventManager.TriggerEvent(EventName.OnLocationChanged, null);

			_vipDisplay.Close();

		}

		#region EventHandlers


		private void HandleOnDayChanged(Dictionary<string, object> _data = null)
		{
			if (GameManager.Instance.CurrentDay - _lastVIPDay < 5) return;
			if (unusedVIPs.Count == 0) return;

			// Randomly spawn VIP if it's been long enough since the last one
			int rand = Random.Range(0, 101);
			if (rand < 50)
			{
				_lastVIPDay = GameManager.Instance.CurrentDay;
				CreateVIP();
			}
		}

		#endregion

		#endregion
}
