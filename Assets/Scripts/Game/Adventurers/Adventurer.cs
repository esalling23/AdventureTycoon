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

		#endregion

		#region Properties

		public int Gold { get { return _gold; } }
		public int Happiness { get { return _happiness; } }
		public int Health { get { return _currentHealth; } }
		public System.Guid Id { get { return _id; } }
		public bool NeedsRest { get { return _currentHealth < _maxHealth; } }

		#endregion

		#region Methods

    void Start()
    {
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

			_maxHealth = Mathf.FloorToInt(Random.Range(1, AdventurerManager.Instance.initHealthRoll));
			_currentHealth = _maxHealth;

			_happiness = AdventurerManager.Instance.maxHappiness / 2;

			EventManager.TriggerEvent(EventName.OnAdventurerStatChanged, null);
    }

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

		private IEnumerator LoopCoroutine()
		{
			while (true)
			{
				// Debug.Log($"Looping adventurer {Id}");
								
				ClearCoroutine();

				// If no location - find one
				if (!_currentLocation)
				{
					_activeCoroutine = StartCoroutine(ChangeLocation());
					yield return _activeCoroutine;
				}

				MapActivity nextActivity;
				// Attempt to do anything possible at the current location
				FindActivityAtLocation(_currentLocation, out nextActivity);

				// If there's no activity found at the current location, change locations
				if (nextActivity == null) {
					_activeCoroutine = StartCoroutine(ChangeLocation());
					yield return _activeCoroutine;
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
					_activeCoroutine = StartCoroutine(PerformActivity(nextActivity));
					yield return _activeCoroutine; // Wait for the activity to complete

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

				// Wait some time before looping again, if needed
				yield return new WaitForSeconds(1f);
			}
		}

		// To do - activities should have time ranges for attempts
		private IEnumerator PerformActivity(MapActivity activity)
		{
			// Adventurers pay upfront for the activity?
			_gold -= activity.data.CostToUse;
			GameManager.Instance.UpdatePlayerGold(activity.data.CostToUse);

			float timeForActivity = Random.Range(GameManager.Instance.MinutesInDay / 2f, (float) GameManager.Instance.MinutesInDay);
			Debug.Log($"Attempting {activity.Type} Activity -- ETA {timeForActivity} minutes");
			yield return new WaitForSeconds(timeForActivity * 60f);

			// // success is 100% by default - only Quest activities can be failed
			// to do - set success probability
			// float successProbability = 100f;
			bool isSuccessful = true;

			Debug.Log($"{activity.Type} Activity Attempt {(isSuccessful ? "Success" : "Failure")}");

			UpdateStats(isSuccessful, activity);

			activity.LogAttempt(Id, isSuccessful);
		}

		private void UpdateStats(bool isSuccess, MapActivity mapActivity)
		{
			if (isSuccess)
			{
				if (mapActivity.data is Quest quest)
				{
					_gold += quest.reward;
					_currentExperience += 50;

					int nextLevelXp = AdventurerManager.Instance.GetAdventurerNextLevelXp(_level);
					if (nextLevelXp <= _currentExperience)
					{
						_currentExperience -= nextLevelXp;
						_level++;
					}
				}
				else if (mapActivity.data is Activity activity)
				{
					if (activity.Type == ActivityType.Rest)
					{
						_currentHealth += 1;
					}
				}

				_happiness += 1;
			}
			else 
			{
				int loss = Mathf.FloorToInt(Random.Range(1, 3));
				_happiness -= loss;
				_currentHealth -= loss;
			}

			if (_happiness > AdventurerManager.Instance.maxHappiness)
			{
				_happiness = AdventurerManager.Instance.maxHappiness;
			}
			if (_currentHealth > _maxHealth)
			{
				_currentHealth = _maxHealth;
			}

			Debug.Log($"Adventurer stats -- \nGold: {_gold}\nLevel: {_level} & XP:{_currentExperience}\nHappiness: {_happiness}\nHealth: {_currentHealth} / {_maxHealth}");
			EventManager.TriggerEvent(EventName.OnAdventurerStatChanged, null);
		}

		private void FindActivityAtLocation(MapLocation location, out MapActivity chosenActivity)
		{
			chosenActivity = null;

			if (!location) {
				Debug.Log("Adventurer not at a location - find one!");
				return;
			};

			List<MapActivity> available = new List<MapActivity>();
			foreach (MapActivity activity in location.activities) {
				// Debug.Log($"Checking activity {activity.data.Name}. Attempted already? {activity.AttemptLog.ContainsKey(Id)}");
				if (
					activity.adventurersPresent.Count > activity.data.Capacity
					|| activity.data.CostToUse > Gold
				)
				{
					continue;
				}
				
				if ((activity.Type == ActivityType.Rest && NeedsRest)
						|| (activity.Type == ActivityType.Quest && !activity.AttemptLog.ContainsKey(Id))
				) {
					chosenActivity = activity;
					break;
				} else if (activity.Type == ActivityType.Rest) {
					// only perform rest activities if we need rest (checked above)
					continue;
				}
				if (activity.data.CostToUse < Gold)
				{
					available.Add(activity);
				}
			}

			if (chosenActivity == null && available.Count > 0) {
				chosenActivity = Utils.GetRandomFromList(available);
			}

			// Debug.Log($"Found Activity Match: {chosenActivity?.data.Name}");
		}

		private void GetOrCreateLocationLog(MapLocation location, out HistoryLog log)
		{
			if (!_historyLog.TryGetValue(location.Id, out log))
			{
				log = new HistoryLog(location);
				_historyLog.Add(location.Id, log);
			}
		}

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

			Debug.Log($"Found Location Match: {bestMatch?.LocationData.name}");

			return bestMatch;
    }

		private IEnumerator ChangeLocation()
		{
			_currentLocation = FindBestLocationMatch();

			if (!_currentLocation)
			{
				_isIdle = true;
				Debug.Log($"Adventurer {_id} could not find anywhere to go :(");
				yield break;
			}

			GetOrCreateLocationLog(_currentLocation, out HistoryLog log);
			log.LogVisitLocation();

			// Move to the new location
			_activeCoroutine = StartCoroutine(Utils.LerpObject(
				gameObject.transform, 
				_currentLocation.transform.position, 
				1f
			));
			yield return _activeCoroutine;
		}

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
