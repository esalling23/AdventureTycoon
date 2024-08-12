using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class MapActionsPanel : MonoBehaviour
{
    #region Fields

		[SerializeField] private Map _map;
		[SerializeField] private GameManager _manager;

		[SerializeField] private int _locationGenerationCount = 3;

		[SerializeField] private GameObject _buildShelf;
		[SerializeField] private LocationDetailItem _locationDetailPrefab;


		#endregion

		#region Properties

		#endregion

		#region Methods

    void Start()
    {
			ClearBuildShelf();
			HideBuildShelf();

			EventManager.StartListening(EventName.OnBuildLocationSelected, HandleBuildLocationSelected);
    }

		void HandleBuildLocationSelected(Dictionary<string, object> data) 
		{
			HideBuildShelf();
		}

		void ClearBuildShelf() {
			foreach (Transform child in _buildShelf.transform) {
				Destroy(child.gameObject);
			}
		}

		public void HideBuildShelf() {
			_buildShelf.SetActive(false);
		}
		public void HandleClickBuildMode() {
			ClearBuildShelf();
			// generate # of locations for player
			Location[] options = new Location[_locationGenerationCount];
			List<Location> availableLocations = new List<Location>(DataManager.Instance.WorldLocations);

			for (int i = 0; i < _locationGenerationCount; i++) 
			{
				Location newLocation = _map.GetRandomLocationData(availableLocations.ToArray());

				availableLocations.Remove(newLocation);

				// and display them
				LocationDetailItem item = Instantiate(
					_locationDetailPrefab,
					Vector3.zero,
					Quaternion.identity, 
					_buildShelf.transform
				);
				item.gameObject.transform.position = Vector3.zero;
				item.SetData(newLocation);
			}

			_buildShelf.SetActive(true);
		}

		// public void HandleClickTimingButton(TimeChangeType type) {
			// if (_manager.Mode == GameMode.Build) {
			// 	_manager.SetMode(GameMode.Run);
			// };
			// _manager.SetMode(GameMode.Build);
		// }



		#endregion
}
