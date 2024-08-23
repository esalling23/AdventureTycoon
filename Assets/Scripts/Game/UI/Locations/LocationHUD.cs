using UnityEngine;
using TMPro;
using System.Collections;


public class LocationHUD : MonoBehaviour
{
    #region Fields

		private RectTransform _transform;
		private Camera _mainCam;
		private float _initialOrthographicSize;
    private Vector3 _initialCanvasScale; 

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

		#endregion
}
