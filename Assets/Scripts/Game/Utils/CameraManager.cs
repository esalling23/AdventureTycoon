using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour 
{
	// Scroll to zoom
	[SerializeField] private float _scrollSpeed = 10;

	// Arrow Key & WASD movement
	private float _horizontalInput;
	private float _verticalInput;
	private Coroutine _movementCoroutine;

	void Start() 
	{

	}

	void Update() 
	{
		float size = Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed;
		if (Camera.main.orthographic)
		{
			Camera.main.orthographicSize -= size;
		}
		else
		{
			Camera.main.fieldOfView -= size;
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