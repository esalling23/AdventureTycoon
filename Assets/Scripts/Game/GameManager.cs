using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class GameManager : MonoBehaviour
{
    #region Fields
		private static GameManager _instance;

		[SerializeField] private int _gold = 0;
		private int _currentDay = 1;

		[SerializeField] private float _minutesInDay = 0.2f;

		private GameMode _mode = GameMode.Run;
		private IEnumerator _activeCoroutine = null;

		[SerializeField] private GameObject _pauseMenu;


		#endregion

		#region Properties

		public static GameManager Instance { get { return _instance; }}

		public float MinutesInDay { get { return _minutesInDay; } }
		public int Gold { get { return _gold; } }
		public int CurrentDay { get { return _currentDay; } }
		public GameMode Mode { get { return _mode; } }

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
		}

		private void OnDestroy()
		{
			ClearCoroutine();
		}

		void Update()
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				Time.timeScale = 0;
				_pauseMenu.SetActive(true);
			}
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

		#endregion
}
