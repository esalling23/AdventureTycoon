using UnityEngine;
using TMPro;
using System.Collections;


public class LocationHUD : MonoBehaviour
{
    #region Fields

		private MapLocation _location;

		private RectTransform _transform;
		private Camera _mainCam;
		private float _initialOrthographicSize;
    private Vector3 _initialCanvasScale; 

		[SerializeField] private TMP_Text _hudName;
		[SerializeField] private TMP_Text _hudPopulationCounter;
		[SerializeField] private GameObject _hudAlerts;
		[SerializeField] private TMP_Text _hudAlertText;

		#endregion

		#region Properties

		#endregion

		#region Methods

		void Start()
		{
			_mainCam = Camera.main;
			_transform = GetComponent<RectTransform>();

			_initialOrthographicSize = CameraManager.Instance.OriginalSize;
			_initialCanvasScale = GetComponent<Canvas>().transform.localScale;
		}

		void Update()
		{
			float scaleFactor = _mainCam.orthographicSize / _initialOrthographicSize;

			_transform.localScale = _initialCanvasScale * scaleFactor;
		}

		public void SetData(MapLocation location)
		{
			_location = location;

			Refresh();
		}

		public void Refresh()
		{
			_hudName.text = _location.LocationData.name;
			_hudPopulationCounter.text = _location.TotalAdventurerCount.ToString();
			_hudAlertText.text = _location.AvailableActions.ToString();

			_hudAlerts.SetActive(_location.AvailableActions > 0);
		}

		#endregion
}
