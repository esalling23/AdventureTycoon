using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class AdventurerManager : MonoBehaviour
{
    #region Fields

		private static AdventurerManager _instance;
		[SerializeField] private Adventurer _adventurerPrefab;

		[Header("Group Sizing Min/Max Values for High & Low Avg. Happiness")]
		public int minLowGroupSize = 1;
		public int maxLowGroupSize = 3;
		public int minHighGroupSize = 10;
		public int maxHighGroupSize = 15;

		[Header("Stats")]
		public Vector2 initHealthRange = new(3, 5);
		[Tooltip("Minimum avg. happiness value to spawn new adventurers")]
		public int minHappinessForGroup = 50;
		public int maxHappiness = 100;

		private float _xpIncreaseRate = 0.3f;
		private List<Adventurer> _adventurersOnMap = new();

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
		public int IdleAdventurersCount { get { return _adventurersOnMap.Where(a => a.IsIdle).ToList().Count; } }

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
			int count = Mathf.FloorToInt(Random.Range(min, max));
			CreateGroup(count);
		}

		public void CreateGroup(int count) {
			for (int i = 0; i < count; i++) {
				CreateAdventurer();
			}
			EventManager.TriggerEvent(EventName.OnAdventurerGroupAdded, null);
			MessageManager.Instance.ShowMessage($"{count} Adventurers Joined Your Map!");
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
			if (AverageHappiness >= minHappinessForGroup)
			{
				float interpolationVal = (AverageHappiness - minHappinessForGroup) / 100f;
				int minRange = Mathf.RoundToInt(Mathf.Lerp(minLowGroupSize, minHighGroupSize, interpolationVal));
        int maxRange = Mathf.RoundToInt(Mathf.Lerp(maxLowGroupSize, maxHighGroupSize, interpolationVal));

        // Ensure minRange is not greater than maxRange
        minRange = Mathf.Clamp(minRange, minLowGroupSize, maxHighGroupSize);
        maxRange = Mathf.Clamp(maxRange, minRange, maxHighGroupSize);

				CreateGroup(minRange, maxRange);
			}
			else
			{
			}
		}

		#endregion

		#endregion
}
