using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapActionsPanel : MonoBehaviour
{
    #region Fields

		[SerializeField] private Map _map;
		[SerializeField] private GameManager _manager;


		#endregion

		#region Properties

		#endregion

		#region Methods

    void Start()
    {
    }

		// public void Toggle

		public void HandleClickBuildMode() {
			if (_manager.Mode == GameMode.Build) {
				_manager.SetMode(GameMode.Run);
			};
			_manager.SetMode(GameMode.Build);
		}

		// public void HandleClickTimingButton(TimeChangeType type) {
			// if (_manager.Mode == GameMode.Build) {
			// 	_manager.SetMode(GameMode.Run);
			// };
			// _manager.SetMode(GameMode.Build);
		// }

		#endregion
}
