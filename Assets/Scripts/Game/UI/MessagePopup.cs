using UnityEngine;
using TMPro;
using System.Collections;


public class MessagePopup : MonoBehaviour
{
    #region Fields

		[SerializeField] private TMP_Text _messageText;
		private Coroutine _activeCoroutine = null;
		/// <summary>
		/// Time after this object spawns that it will auto-hide itself
		/// </summary>
		public float autoHideTime = 2f;

		#endregion

		#region Properties

		#endregion

		#region Methods

		private void ClearCoroutine()
		{
			if (_activeCoroutine != null) {
				StopCoroutine(_activeCoroutine);
				_activeCoroutine = null;
			}
		}

		private IEnumerator StayVisible()
		{
			yield return new WaitForSeconds(autoHideTime);

			CloseMessage();
		}

		public void OnClickHideMessage()
		{
			CloseMessage();
		}

		private void CloseMessage()
		{
			ClearCoroutine();

			gameObject.SetActive(false);
			MessageManager.Instance.CloseMessage(this);
		}

		public void ShowMessage(string message)
		{
			_messageText.text = message;
			gameObject.SetActive(true);

			_activeCoroutine = StartCoroutine(StayVisible());
		}

		#endregion
}
