using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class TimeControls : MonoBehaviour
{
	[SerializeField] private Button _speedUpButton;
	[SerializeField] private Button _slowDownButton;
	[SerializeField] private Button _pauseButton;
	[SerializeField] private TMP_Text _timingText;

	void Start()
	{
		HandleClickPlay();
	}

	void SetTimingText()
	{
		if (Time.timeScale == 0)
		{
			_timingText.text = "Paused";
			return;
		}
		_timingText.text = $"x{Time.timeScale}";
	}

	void SetButtonsEnabled()
	{
		_slowDownButton.interactable = Time.timeScale > TimeManager.Instance.minSpeed;
		_speedUpButton.interactable = Time.timeScale < TimeManager.Instance.maxSpeed;

		_pauseButton.interactable = Time.timeScale != 0;
	}

	public void HandleClickPlay()
	{
		TimeManager.Instance.PlayTime();
		SetButtonsEnabled();
		SetTimingText();
	}
	public void HandleClickPause()
	{
		TimeManager.Instance.PauseTime();
		SetButtonsEnabled();
		SetTimingText();
	}
	public void HandleClickSpeedUp()
	{
		TimeManager.Instance.ChangeSpeed(1);

		SetButtonsEnabled();
		SetTimingText();
	}
	public void HandleClickSlowDown()
	{
		TimeManager.Instance.ChangeSpeed(-1);

		SetButtonsEnabled();
		SetTimingText();
	}
}