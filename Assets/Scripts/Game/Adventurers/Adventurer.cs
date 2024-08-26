using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : MonoBehaviour
{
    #region Fields

		private Map _map;

		private System.Guid _id = System.Guid.NewGuid();
		private int _gold = 100;
		private int _happiness = 50;
		private int _currentHealth;
		[SerializeField] private int _maxHealth;
		private int _level = 1;
		private int _currentExperience = 0;

		private MapLocation _currentLocation = null;
		// private List<string> _inventory = new List<string>();
		private bool _isIdle = false;

		// public AdventurerEffect activeEffects;

		private Coroutine _activeCoroutine = null;

		private Dictionary<System.Guid, HistoryLog> _historyLog = new Dictionary<System.Guid, HistoryLog>();

		private Renderer _renderer;

		#endregion

		#region Properties

		public int Gold { get { return _gold; } }
		public int Level { get { return _level; } }
		public int Happiness { get { return _happiness; } }
		public int Health { get { return _currentHealth; } }
		public System.Guid Id { get { return _id; } }
		public bool NeedsRest { get { return _currentHealth < _maxHealth; } }

		#endregion

		#region Methods

    void Start()
    {
			_renderer = GetComponent<Renderer>();
			EventManager.StartListening(EventName.OnActivityChanged, HandleOnActivityChanged);
			EventManager.StartListening(EventName.OnDayChanged, HandleOnDayChanged);
		}

		public void OnDestroy()
		{
			EventManager.StopListening(EventName.OnActivityChanged, HandleOnActivityChanged);
			EventManager.StopListening(EventName.OnDayChanged, HandleOnDayChanged);
		}

    public void Init()
    {
			// Debug.Log($"Initing adventurer {Id}");

			_map = GameObject.FindWithTag("Map").GetComponent<Map>();

			_maxHealth = Mathf.FloorToInt(Random.Range(
				AdventurerManager.Instance.initHealthRange.x, 
				AdventurerManager.Instance.initHealthRange.y
			));
			_currentHealth = _maxHealth;

			_happiness = AdventurerManager.Instance.maxHappiness / 2;

			EventManager.TriggerEvent(EventName.OnAdventurerStatChanged, null);
    }

		#region Activity Loop

		void ClearCoroutine()
		{
			if (_activeCoroutine != null) {
				StopCoroutine(_activeCoroutine);
				_activeCoroutine = null;
			}
		}

		public void Loop()
		{
			_activeCoroutine = StartCoroutine(LoopCoroutine());
		}

		/// <summary>
		/// Force adventurer to stop current activity and find a new one
		/// </summary>
		public void KickOut()
		{
			ClearCoroutine();
			Loop();
		}

		private IEnumerator LoopCoroutine()
		{
			while (true)
			{
				// Debug.Log($"Looping adventurer {Id}");
								
				ClearCoroutine();

				// If no location - find one
				if (!_currentLocation)
				{
					yield return StartCoroutine(ChangeLocation());
				}

				// Attempt to do anything possible at the current location
				FindActivityAtLocation(_currentLocation, out MapActivity nextActivity);

				// If there's no activity found at the current location, change locations
				if (nextActivity == null) {
					yield return StartCoroutine(ChangeLocation());
				}
				else 
				{
					_isIdle = false;
					// Debug.Log($"Adventurer {_id} chose {nextActivity?.Type} activity at {nextActivity?.locationParent}");

					GetOrCreateLocationLog(_currentLocation, out HistoryLog log);
					log.LogAttemptActivity(nextActivity);

					nextActivity.adventurersPresent.Add(this);
					EventManager.TriggerEvent(EventName.OnActivityChanged, new Dictionary<string, object>() {
						{ "type", nextActivity.Type }
					});

					// Adventurer does the activity
					yield return StartCoroutine(PerformActivity(nextActivity)); // Wait for the activity to complete

					nextActivity.adventurersPresent.Remove(this);
					EventManager.TriggerEvent(EventName.OnActivityChanged, new Dictionary<string, object>() {
						{ "type", nextActivity.Type }
					});
				}

				// Possibility for death during quest activities
				if (_currentHealth <= 0) {
					Debug.Log("Adventurer died");
					AdventurerManager.Instance.KillAdventurer(this);
					yield break;
				}

				// Debug.Log($"Adventurer {Id} ready for next loop");
				yield return new WaitForSeconds(1f);
			}
		}

		private IEnumerator PerformActivity(MapActivity mapActivity)
		{
			_gold -= mapActivity.data.CostToUse;
			GameManager.Instance.UpdatePlayerGold(mapActivity.data.CostToUse);

			float timeForActivity = Random.Range(mapActivity.data.MinTimeToUse, mapActivity.data.MaxTimeToUse);
			Debug.Log($"Attempting {mapActivity.Type} Activity -- ETA {timeForActivity} minutes");
			yield return new WaitForSeconds(timeForActivity * 60f);

			// success is 100% by default - only Quest activities can be failed
			bool isSuccessful = true;
			if (mapActivity.data is Quest quest)
			{
				isSuccessful = quest.Attempt(this);
			}
			else if (mapActivity.data is Activity activity)
			{
				activity.Attempt(this);
			}

			mapActivity.LogAttempt(Id, isSuccessful);
			Debug.Log($"{mapActivity.Type} Activity Attempt {(isSuccessful ? "Success" : "Failure")}");

			CapStats();
			EventManager.TriggerEvent(EventName.OnAdventurerStatChanged, null);
			Debug.Log($"Adventurer stats -- \nGold: {_gold}\nLevel: {_level} & XP:{_currentExperience}\nHappiness: {_happiness}\nHealth: {_currentHealth} / {_maxHealth}");
		}

		#endregion

		#region Stats

		/// <summary>
		/// Caps stats at maximum or minimum values
		/// </summary>
		private void CapStats()
		{
			if (_happiness > AdventurerManager.Instance.maxHappiness)
			{
				_happiness = AdventurerManager.Instance.maxHappiness;
			}
			else if (_happiness < 0) _happiness = 0;

			if (_currentHealth > _maxHealth)
			{
				_currentHealth = _maxHealth;
			}
		}

		public void ChangeExperience(int xp)
		{
			_currentExperience += xp;
			int nextLevelXp = AdventurerManager.Instance.GetAdventurerNextLevelXp(_level);
			if (nextLevelXp <= _currentExperience)
			{
				_currentExperience -= nextLevelXp;
				_level++;
				_maxHealth++;
			}
		}
		public void ChangeReward(int gold)
		{
			_gold += gold;
		}
		public void ChangeHappiness(int change)
		{
			_happiness += change;
		}
		public void ChangeHealth(int change)
		{
			_currentHealth += change;
		}

		#endregion

		#region Locations

		/// <summary>
		/// Finds a suitable activity at a provided location
		/// </summary>
		/// <param name="location">The location to search</param>
		/// <param name="chosenActivity">The activity selected</param>
		private void FindActivityAtLocation(MapLocation location, out MapActivity chosenActivity)
		{
			chosenActivity = null;

			if (!location) {
				// Debug.Log("Adventurer not at a location - find one!");
				return;
			};

			List<MapActivity> available = new();
			foreach (MapActivity activity in location.activities) {
				// Debug.Log($"Checking activity {activity.data.Name}. Attempted already? {activity.AttemptLog.ContainsKey(Id)}");
				if (
					activity.adventurersPresent.Count >= activity.data.Capacity
					|| activity.data.CostToUse > Gold
				)
				{
					continue;
				}
				
				if ((activity.Type == ActivityType.Rest && NeedsRest)
						|| (activity.data is Quest quest && 
								!activity.AttemptLog.ContainsKey(Id) && 
								quest.minLevel <= _level)
				) {
					chosenActivity = activity;
					break;
				} 
				else if (activity.Type == ActivityType.Rest) {
					// only perform rest activities if we need rest (checked above)
					continue;
				}
				if (activity.data.CostToUse <= Gold)
				{
					available.Add(activity);
				}
			}

			if (chosenActivity == null && available.Count > 0) {
				chosenActivity = Utils.GetRandomFromList(available);
			}

			Debug.Log($"Found Activity Match: {chosenActivity?.data.Name}");
		}

		private void GetOrCreateLocationLog(MapLocation location, out HistoryLog log)
		{
			if (!_historyLog.TryGetValue(location.Id, out log))
			{
				log = new HistoryLog(location);
				_historyLog.Add(location.Id, log);
			}
		}

		/// <summary>
		/// Finds best location to move to next based on location & last visited timestamp
		/// </summary>
		/// <returns>The closest & oldest visited location on the map</returns>
		public MapLocation FindBestLocationMatch()
    {
			MapLocation bestMatch = null;

			float bestScore = float.MaxValue;

			float x0 = transform.position.x;
			float y0 = transform.position.y;

			// Debug.Log($"Searching through all {_map.LocationsOnMap.Count} locations");

			foreach (MapLocation loc in _map.LocationsOnMap)
			{
				// Debug.Log($"Looking at location {loc.LocationData.name}");
				if (loc.WorldPosition == null) continue;

				float x = loc.WorldPosition.x;
				float y = loc.WorldPosition.y;
				float distance = Mathf.Sqrt(Mathf.Pow(x - x0, 2) + Mathf.Pow(y - y0, 2));

				System.DateTime lastVisited;
				if (_historyLog.TryGetValue(loc.Id, out HistoryLog lastVisitedLog))
				{
					lastVisited = lastVisitedLog.timeLastVisit;
				} else {
					// If the location was never visited, treat it as very old
					lastVisited = System.DateTime.MinValue;
				}
				// Debug.Log($"Location {loc.Id} last visited at {lastVisited}");

				float timeSinceLastVisit = (float)(System.DateTime.Now - lastVisited).TotalSeconds;
				
				float score = distance - timeSinceLastVisit * 0.001f;

				// Debug.Log($"Location {loc.LocationData.name} match score is {score}. Best score is {bestScore}");

				if (score < bestScore)
				{
					bestScore = score;
					bestMatch = loc;
				}
			}

			// Debug.Log($"Found Location Match: {bestMatch?.LocationData.name}");

			return bestMatch;
    }

		private IEnumerator ChangeLocation()
		{			
			if (!_renderer) _renderer = GetComponent<Renderer>();
			_renderer.enabled = true;

			_currentLocation = FindBestLocationMatch();

			if (!_currentLocation)
			{
				_isIdle = true;
				// Debug.Log($"Adventurer {_id} could not find anywhere to go :(");
				yield break;
			}

			GetOrCreateLocationLog(_currentLocation, out HistoryLog log);
			log.LogVisitLocation();

			// Physically move to the new location
			_activeCoroutine = StartCoroutine(Utils.LerpObject(
				gameObject.transform, 
				_currentLocation.transform.position, 
				1f
			));
			yield return _activeCoroutine;

			_renderer.enabled = false;
		}

		#endregion

		#region Event Handlers

		private void HandleOnActivityChanged(Dictionary<string, object> msg)
		{
			// Attempt loop
			if (_isIdle)
			{
				Loop();
			}
		}

		private void HandleOnDayChanged(Dictionary<string, object> msg)
		{
			_happiness--;
		}

		#endregion
		
		#endregion
}
