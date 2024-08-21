using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour 
{
	// Scroll to zoom
	[SerializeField] private float _scrollSpeed = 10;
	[SerializeField] private float _minZoom = 5f;
	[SerializeField] private float _maxZoom = 100f;

	// Arrow Key & WASD movement
	[SerializeField] private float _speed = 30f;
	private Vector3 _input;
	private float _horizontalInput;
	private float _verticalInput;
	private Coroutine _movementCoroutine;

	void Start() 
	{

	}

	void Update() 
	{
			_horizontalInput = Input.GetAxis("Horizontal");
			_verticalInput = Input.GetAxis("Vertical");

		if (EventSystem.current.IsPointerOverGameObject())
			return;

		
		float size = Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed;
		if (Camera.main.orthographic)
		{
			Camera.main.orthographicSize -= size;
			if (Camera.main.orthographicSize < _minZoom)
			{
				Camera.main.orthographicSize = _minZoom;
			} else if (Camera.main.orthographicSize > _maxZoom)
			{
				Camera.main.orthographicSize = _maxZoom;
			}
		}
		else
		{
			Camera.main.fieldOfView -= size;
			if (Camera.main.fieldOfView < _minZoom)
			{
				Camera.main.fieldOfView = _minZoom;
			} else if (Camera.main.fieldOfView > _maxZoom)
			{
				Camera.main.fieldOfView = _maxZoom;
			}
		}
	}

	public void AnimateTo(Vector3 newPos, float duration)
	{
		if (_movementCoroutine != null) StopCoroutine(_movementCoroutine);

		Vector3 newCameraPos = new Vector3(newPos.x, newPos.y, gameObject.transform.position.z);
		_movementCoroutine = StartCoroutine(Utils.LerpObject(Camera.main.transform, newCameraPos, duration));
	}


	private void FixedUpdate()
	{
		_input = new Vector3(
			_horizontalInput,
			_verticalInput,
			0f
		).normalized;
		_input = Vector3.ClampMagnitude(_input, 1);

		Camera.main.transform.Translate(_speed * Time.deltaTime * _input);
	}
}