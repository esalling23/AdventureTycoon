using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    #region Fields
		private static GameManager _instance;

		[SerializeField] private int _gold = 0;
		private int _currentDay = 1;
		private int _highestPopulation = 0;

		private bool _isForceQuit = false;

		[SerializeField] private float _minutesInDay = 0.2f;

		private GameMode _mode = GameMode.Run;
		private IEnumerator _activeCoroutine = null;

		#endregion

		#region Properties

		public static GameManager Instance { get { return _instance; }}

		public int HighestPopulation { get { return _highestPopulation; } }
		// public int TotalLocations { get { return _totalLocations; } }
		// public int TotalActivities { get { return _totalActivities; } }
		// public int TotalQuests { get { return _totalQuests; } }

		public float MinutesInDay { get { return _minutesInDay; } }
		public int Gold { get { return _gold; } }
		public int CurrentDay { get { return _currentDay; } }
		public GameMode Mode { get { return _mode; } }

		public bool IsForceQuit { get { return _isForceQuit; } }

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

		private void Start()
		{
			StartCoroutine(PlayTime());

			EventManager.StartListening(EventName.OnAdventurerGroupAdded, HandleOnAdventurerGroupAdded);
		}

		private void OnDestroy()
		{
			EventManager.StopListening(EventName.OnAdventurerGroupAdded, HandleOnAdventurerGroupAdded);
			ClearCoroutine();
		}

		private void ClearCoroutine()
		{
			if (_activeCoroutine != null) {
				StopCoroutine(_activeCoroutine);
				_activeCoroutine = null;
			}
		}

		private IEnumerator PlayTime()
		{
			while (true)
			{
				yield return new WaitForSeconds(MinutesInDay * 60f);

				_currentDay++;

				EventManager.TriggerEvent(EventName.OnDayChanged, null);
			}
		}

		public void UpdatePlayerGold(int gold)
		{
			_gold += gold;
			EventManager.TriggerEvent(EventName.OnPlayerGoldChanged, null);
		}

		public void SetMode(GameMode mode) {
			_mode = mode;
		}

		public void StartGame()
		{
			_highestPopulation = 0;
			SceneManager.LoadScene("MainScene");
		}
		public void GameOver(bool isForceQuit)
		{
			_isForceQuit = isForceQuit;
			SceneManager.LoadScene("EndGame");
		}

		#region Event Handlers

		private void HandleOnAdventurerGroupAdded(Dictionary<string, object> _data)
		{
			if (AdventurerManager.Instance.TotalAdventurers > _highestPopulation)
			{
				_highestPopulation = AdventurerManager.Instance.TotalAdventurers;
			}
		}

		#endregion

		#endregion
}
