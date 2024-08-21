using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class AdventurerManager : MonoBehaviour
{
    #region Fields

		private static AdventurerManager _instance;

		public int initHealthRoll = 4;
		public int minGroupSize = 5;
		public int maxGroupSize = 10;
		public int maxHappiness = 100;
		private float _xpIncreaseRate = 0.3f;

		private List<Adventurer> _adventurersOnMap = new List<Adventurer>();

		[SerializeField] private Adventurer _adventurerPrefab;

		#endregion

		#region Properties

		public static AdventurerManager Instance { get { return _instance; }}
		public int AverageHappiness { 
			get { 
				if (TotalAdventurers == 0) return 0;

				int total = (from obj in Instance._adventurersOnMap select obj.Happiness).Sum();
				return total / TotalAdventurers; 
			}
		}
		public int TotalAdventurers { get { return _adventurersOnMap.Count; } }

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
			EventManager.StartListening(EventName.OnDayChanged, HandleOnDayChanged);
		}
		void OnDestroy()
		{
			EventManager.StopListening(EventName.OnDayChanged, HandleOnDayChanged);
		}

		public void CreateGroup(int min, int max) {
			int count = Random.Range(min, max);
			CreateGroup(count);
		}

		public void CreateGroup(int count) {
			for (int i = 0; i < count; i++) {
				CreateAdventurer();
			}
			EventManager.TriggerEvent(EventName.OnMessageBroadcast, new Dictionary<string, object>() {
				{ "message", $"{count} Adventurers Joined Your Map!" }
			});
		}

		public void CreateAdventurer() 
		{
			Adventurer newAdventurer = Instantiate(
				_adventurerPrefab,
				Vector3.zero,
				Quaternion.identity,
				gameObject.transform
			).GetComponent<Adventurer>();
			_adventurersOnMap.Add(newAdventurer);
			newAdventurer.Init();
			newAdventurer.Loop();
		}

		public void KillAdventurer(Adventurer adventurer)
		{
			_adventurersOnMap.Remove(adventurer);
			Destroy(adventurer.gameObject);
		}

		public int GetAdventurerNextLevelXp(int level = 1)
		{
			return GetAdventurerLevelXp(level + 1);
		}

		public int GetAdventurerLevelXp(int level = 1)
		{
			return Mathf.FloorToInt(100 * Mathf.Pow(1 + _xpIncreaseRate, level));
		}

		#region EventHandlers

		private void HandleOnDayChanged(Dictionary<string, object> _data = null)
		{
			if (AverageHappiness > 25)
			{
				// examples: assuming maxGroupSize = 10, minGroupSize = 5
				// AH = 25  -> 3 - 5 advs
				// AH = 50  -> 5 - 10
				// AH = 80 -> 8 - 16
				// AH = 100 -> 10 - 20
				CreateGroup(AverageHappiness / maxGroupSize, AverageHappiness / minGroupSize);
			}
			else
			{
				Debug.Log("Happiness is low - no new adventurers wanted to come to your map.");
			}
		}

		#endregion

		#endregion
}
