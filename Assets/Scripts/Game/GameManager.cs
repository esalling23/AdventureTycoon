using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class GameManager : MonoBehaviour
{
    #region Fields
		private static GameManager _instance;

		private int _gold;
		private int _currentDay;

		[SerializeField] private float _minutesInDay = 0.2f;

		private GameMode _mode = GameMode.Run;


		#endregion

		#region Properties

		public static GameManager Instance { get { return _instance; }}

		public float MinutesInDay { get { return _minutesInDay; } }
		public int Gold { get { return _gold; } }
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

		public void SetMode(GameMode mode) {
			_mode = mode;
		}

		#endregion
}
