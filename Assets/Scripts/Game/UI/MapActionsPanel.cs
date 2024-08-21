using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// Map action panel of buttons
/// - Build button & build type shelf
/// ???
/// </summary>
public class MapActionsPanel : MonoBehaviour
{
    #region Fields

		// Displays location types to build
		[SerializeField] private GameObject _buildTypeShelf;
		[SerializeField] private GameObject _locationTypeContainer;
		[SerializeField] private LocationTypeItem _locationTypeItemPrefab;
		[SerializeField] private List<LocationTypeItem> _typeItems = new List<LocationTypeItem>();
		[SerializeField] private LocationBuildModal _locationBuildModal;

		#endregion

		#region Properties

		#endregion

		#region Methods

    void Start()
    {
			HideBuildShelf();

			foreach (LocationTypeData def in DataManager.Instance.LocationTypeDefaults)
			{
				LocationTypeItem item = Instantiate(
					_locationTypeItemPrefab,
					Vector3.zero,
					Quaternion.identity,
					_locationTypeContainer.transform
				);
				item.SetData(def);
				_typeItems.Add(item);
			}

			EventManager.StartListening(EventName.OnBuildTypeSelected, HandleBuildTypeSelected);
			EventManager.StartListening(EventName.OnBuildLocationSelected, HandleBuildLocationSelected);
    }

		void OnDestroy()
		{
			EventManager.StopListening(EventName.OnBuildTypeSelected, HandleBuildTypeSelected);
			EventManager.StopListening(EventName.OnBuildLocationSelected, HandleBuildLocationSelected);
		}

		void HandleBuildLocationSelected(Dictionary<string, object> data) 
		{
			HideBuildShelf();
		}

		public void HideBuildShelf() {
			_buildTypeShelf.SetActive(false);
		}
		// public void HandleClickTimingButton(TimeChangeType type) {
			// if (_manager.Mode == GameMode.Build) {
			// 	_manager.SetMode(GameMode.Run);
			// };
			// _manager.SetMode(GameMode.Build);
		// }

		/// <summary>
		/// Shows the build type shelf of options
		/// </summary>
		public void HandleClickBuildButton()
		{
			if (_buildTypeShelf.activeInHierarchy)
			{
				HideBuildShelf();
				return;
			}
			
			Debug.Log(_typeItems.Count);
			foreach (LocationTypeItem item in _typeItems)
			{
				item.CheckIsEnabled();
			}
			_buildTypeShelf.SetActive(true);

			// todo: update buttons to be disabled if they can't be built (ex player doesnt have enough money)
		}

		private void HandleBuildTypeSelected(Dictionary<string, object> data)
		{
			HideBuildShelf();
		}


		#endregion
}
