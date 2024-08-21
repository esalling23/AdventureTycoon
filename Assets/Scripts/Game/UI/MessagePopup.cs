using UnityEngine;
using TMPro;


public class MessagePopup : MonoBehaviour
{
    #region Fields

		[SerializeField] private TMP_Text _messageText;
		private float _timer = 0f;
		/// <summary>
		/// Time after this object spawns that it will auto-hide itself
		/// </summary>
		private float _autoHideTime = 3f;
		private bool _isActive = false;


		#endregion

		#region Properties

		#endregion

		#region Methods

		private void Update()
		{
			if (!_isActive) return;

			_timer += Time.deltaTime;

			if (_timer >= _autoHideTime)
			{
				CloseMessage();
			}
		}

		private void Start()
		{
			_timer = 0f;
		}

		public void OnClickHideMessage()
		{
			CloseMessage();
		}

		private void CloseMessage()
		{
			gameObject.SetActive(false);
			_isActive = false;
			MessageManager.Instance.CloseMessage(this);
		}

		public void ShowMessage(string message)
		{
			_messageText.text = message;
		}

		#endregion
}
