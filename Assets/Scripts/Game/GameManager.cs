using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class GameManager : MonoBehaviour
{
    #region Fields

		private int _gold;
		private int _currentDay;

		private GameMode _mode = GameMode.Run;

		private float _xpIncreaseRate = 0.3f;

		#endregion

		#region Properties

		public int Gold { get { return _gold; } }
		public GameMode Mode { get { return _mode; } }

		#endregion

		#region Methods


		public int GetAdventurerLevelXp(int level = 1)
		{
			return Mathf.FloorToInt(100 * Mathf.Pow(1 + _xpIncreaseRate, level));
		}

		public void SetMode(GameMode mode) {
			_mode = mode;
		}

		#endregion
}
