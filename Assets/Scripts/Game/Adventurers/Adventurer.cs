using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : MonoBehaviour
{
    #region Fields

		private Map _map;

		private System.Guid _id = System.Guid.NewGuid();
		private int _gold = 100;
		private int _happiness;
		private int _currentHealth;
		[SerializeField] private int _maxHealth;
		private int _level = 1;
		private int _currentExperience = 0;

		private MapLocation _currentLocation = null;
		private List<MapLocation> _locationsVisited = new List<MapLocation>();

		private List<string> _inventory = new List<string>();
		// if this adventurer has tried to pass time since their last quest
		// will be true even if the adventurer was not able to find a pass time activity
		private bool _hasPassedTimeSinceLastQuest = false;
		private bool _isNew = true;


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
			
		}

    public void Init()
    {
			Debug.Log($"Initing adventurer {Id}");

			_map = GameObject.FindWithTag("Map").GetComponent<Map>();
			Debug.Log(_map);

			_maxHealth = Mathf.FloorToInt(Random.Range(1, AdventurerManager.Instance.initHealthRoll));
			_currentHealth = _maxHealth;

			_happiness = AdventurerManager.Instance.maxHappiness;
    }

		void ClearCoroutine()
		{
			if (_activeCoroutine != null) {
				StopCoroutine(_activeCoroutine);
				_activeCoroutine = null;
			}
		}

		private Coroutine _loopCoroutine;

		public void Loop()
		{
			_loopCoroutine = StartCoroutine(LoopCoroutine());
		}

		private IEnumerator LoopCoroutine()
		{
			while (true)
			{
				Debug.Log($"Looping adventurer {Id}");
								
				ClearCoroutine();

				MapActivity nextActivity = ChooseActivity();
				Debug.Log($"Adventurer {_id} chose {nextActivity?.Type} activity at {nextActivity?.locationParent}");

				if (nextActivity == null) {
					Debug.Log($"Adventurer {_id} could not find anything to do :(");
					yield break;  // Stop the loop if there's nothing to do
				}

				if (!_historyLog.ContainsKey(nextActivity.locationParent.Id)) {
					_historyLog.Add(nextActivity.locationParent.Id, new HistoryLog(nextActivity.locationParent));
				}

				if (nextActivity?.locationParent.Id != _currentLocation?.Id)
				{
					// Move to the new location
					_activeCoroutine = StartCoroutine(Utils.LerpObject(
						gameObject.transform, 
						nextActivity.locationParent.transform.position, 
						1f
					));
					yield return _activeCoroutine; // Wait for movement to complete
				}

				_currentLocation = nextActivity.locationParent;

				_historyLog.TryGetValue(nextActivity.locationParent.Id, out HistoryLog log);
				log.LogAttemptActivity(nextActivity);
				log.LogVisitLocation();

				nextActivity.adventurersPresent.Add(this);

				EventManager.TriggerEvent(EventName.OnActivityChanged, new Dictionary<string, object>() {
					{ "type", nextActivity.Type }
				});

				_activeCoroutine = StartCoroutine(PerformActivity(nextActivity));
				yield return _activeCoroutine; // Wait for the activity to complete

				nextActivity.adventurersPresent.Remove(this);

				EventManager.TriggerEvent(EventName.OnActivityChanged, new Dictionary<string, object>() {
					{ "type", nextActivity.Type }
				});

				Debug.Log($"Adventurer {Id} ready for next loop");

				// Wait some time before looping again, if needed
				yield return new WaitForSeconds(1f);
			}
		}

		public void StopLoop()
		{
				if (_loopCoroutine != null)
				{
						StopCoroutine(_loopCoroutine);
						_loopCoroutine = null;
				}
		}

		// To do - activities should have time ranges for attempts
		private IEnumerator PerformActivity(MapActivity activity)
		{
			float timeForActivity = Random.Range(GameManager.Instance.MinutesInDay / 2f, (float) GameManager.Instance.MinutesInDay);
			Debug.Log($"Attempting {activity.Type} Activity -- ETA {timeForActivity} minutes");
			yield return new WaitForSeconds(timeForActivity * 60f);

			// // success is 100% by default - only Quest activities can be failed
			// float successProbability = 100f;
			bool isSuccessful = true;

			Debug.Log($"{activity.Type} Activity Attempt {(isSuccessful ? "Success" : "Failure")}");

			// if (activityType == ActivityType.Quest) {
			// 	// set success probability
			// }
			if (isSuccessful) {
				_currentHealth -= 1;
				_currentExperience += 50;
				_gold += 50;
				
			// 	_historyLog.LogItemCompleted(nextActivity);
			}
			else
			{
			// 	_historyLog.LogItemFailed(nextActivity);
			}

			activity.LogAttempt(Id, isSuccessful);
		}

		// private ActivityType ChooseActivityType() 
		// {
		// 	if (_currentHealth < _maxHealth) {
		// 		// prioritize rest activities
		// 		return ActivityType.Rest;
		// 	}
		// 	else if (_inventory.Count > 0) {
		// 		// prioritize trade activities
		// 		return ActivityType.Trade;
		// 	}

		// 	if (!_hasPassedTimeSinceLastQuest) {
		// 		// pass time instead
		// 		_hasPassedTimeSinceLastQuest = true;
		// 		return ActivityType.PassTime;
		// 	}

		// 	return ActivityType.Quest;
		// }

		// private void ShuffleActivities(MapActivity[] activities)
    // {
    //     for (int t = 0; t < activities.Length; t++ )
    //     {
    //         MapActivity tmp = activities[t];
    //         int r = Random.Range(t, activities.Length);
    //         activities[t] = activities[r];
    //         activities[r] = tmp;
    //     }
    // }

		private void FindActivityAtLocation(MapLocation location, out MapActivity chosenActivity)
		{
			chosenActivity = null;

			if (!location) {
				Debug.Log("Adventurer not at a location - find one!");
				return;
			};

			List<MapActivity> available = new List<MapActivity>();
			foreach (MapActivity activity in location.activities) {
				Debug.Log($"Checking activity {activity.data.Name}. Attempted already? {activity.AttemptLog.ContainsKey(Id)}");
				if ((activity.Type == ActivityType.Rest && NeedsRest)
						|| (activity.Type == ActivityType.Trade && _inventory.Count > 0)
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
				int randIndex = Random.Range(0, available.Count);
				chosenActivity = available[randIndex];
			}

			Debug.Log($"Found Activity Match: {chosenActivity?.data.Name}");
		}

		public MapLocation FindBestLocationMatch()
    {
			MapLocation bestMatch = null;

			float bestScore = float.MaxValue;

			float x0 = transform.position.x;
			float y0 = transform.position.y;

			Debug.Log($"Searching through all {_map.LocationsOnMap.Count} locations");

			foreach (MapLocation loc in _map.LocationsOnMap)
			{
				Debug.Log($"Looking at location {loc.LocationData.name}");
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
				Debug.Log($"Location {loc.Id} last visited at {lastVisited}");

				float timeSinceLastVisit = (float)(System.DateTime.Now - lastVisited).TotalSeconds;
				
				float score = distance - timeSinceLastVisit * 0.001f;

				Debug.Log($"Location {loc.LocationData.name} match score is {score}. Best score is {bestScore}");

				if (score < bestScore)
				{
					bestScore = score;
					bestMatch = loc;
				}
			}

			Debug.Log($"Found Location Match: {bestMatch?.LocationData.name}");

			return bestMatch;
    }

		private MapActivity ChooseActivity() 
		{
			MapActivity chosenActivity = null;

			// Attempt to do anything possible at the current location
			FindActivityAtLocation(_currentLocation, out chosenActivity);

			// If there's nothing left to do at our location
			// Find another closeby location
			if (chosenActivity == null) {
				try {
					MapLocation matchedLocation = FindBestLocationMatch();
					FindActivityAtLocation(matchedLocation, out chosenActivity);
				} catch(System.Exception err) {
					Debug.Log(err);
					Debug.Log("Couldn't find location w. activity :((((");
				}
			}

			if (chosenActivity?.Type == ActivityType.PassTime) {
				_hasPassedTimeSinceLastQuest = true;
			}

			return chosenActivity;
		}

		#endregion
}
