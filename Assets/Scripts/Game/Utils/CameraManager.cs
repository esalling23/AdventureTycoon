using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour 
{
	// Scroll to zoom
	[SerializeField] private float _scrollSpeed = 10;
	[SerializeField] private float minZoom = 5f;
	[SerializeField] private float maxZoom = 100f;

	// Arrow Key & WASD movement
	private float _horizontalInput;
	private float _verticalInput;
	private Coroutine _movementCoroutine;

	void Start() 
	{

	}

	void Update() 
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return;

		
		float size = Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed;
		if (Camera.main.orthographic)
		{
			Camera.main.orthographicSize -= size;
			if (Camera.main.orthographicSize < minZoom)
			{
				Camera.main.orthographicSize = minZoom;
			} else if (Camera.main.orthographicSize > maxZoom)
			{
				Camera.main.orthographicSize = maxZoom;
			}
		}
		else
		{
			Camera.main.fieldOfView -= size;
			if (Camera.main.fieldOfView < minZoom)
			{
				Camera.main.fieldOfView = minZoom;
			} else if (Camera.main.fieldOfView > maxZoom)
			{
				Camera.main.fieldOfView = maxZoom;
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
		_horizontalInput = Input.GetAxis("Horizontal");
		_verticalInput = Input.GetAxis("Vertical");

		if (_horizontalInput != 0 ^ _verticalInput != 0) 
		{
			Camera.main.transform.Translate(new Vector3(_horizontalInput, _verticalInput, 0f));
		}
	}
}