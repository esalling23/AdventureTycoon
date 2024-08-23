using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{

	#region Fields

	private static TimeManager _instance;

	public int maxSpeed = 3;
	public int minSpeed = 1;
	private int _lastSpeed = 1;
	
	#endregion

	#region Properties

	public static TimeManager Instance { get { return _instance; }}
	public int LastSpeed { get { return _lastSpeed; }}
	
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

	public void ChangeSpeed(int change)
	{
		Time.timeScale += change;

		if (Time.timeScale < minSpeed)
		{
			Time.timeScale = minSpeed;
		}
		else if (Time.timeScale > maxSpeed)
		{
			Time.timeScale = maxSpeed;
		}

		SetLastSpeed();
	}
	public void SetLastSpeed()
	{
		_lastSpeed = Mathf.FloorToInt(Time.timeScale);
	}

	public void PlayTime()
	{
		Time.timeScale = Mathf.FloorToInt(_lastSpeed);
	}

	public void PauseTime()
	{
		if (Time.timeScale == 0) return;

		SetLastSpeed();
		Time.timeScale = 0;
	}
	#endregion
}