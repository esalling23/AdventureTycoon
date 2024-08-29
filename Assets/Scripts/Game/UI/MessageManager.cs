using System.Collections.Generic;
using UnityEngine;

public class Message : UniqueObject
{
	


}

public class MessageManager : MonoBehaviour
{
    #region Fields

		private static MessageManager _instance;

		[SerializeField] private MessagePopup _messageObj;

		private List<string> _allMessages = new();
		private bool _isMessageActive = false;

		#endregion

		#region Properties

		public static MessageManager Instance { get { return _instance; }}
		
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

		public void CloseMessage(MessagePopup _message)
		{
			// future work - close only this message if multiple showing
			_isMessageActive = false;
		}

		public void ShowMessage(string message)
		{
			Debug.Log($"Message Displayed: {message.ToString()}");
			_allMessages.Add(message.ToString());

			// implementing a single-at-a-time message for now
			_messageObj.ShowMessage(message.ToString());
			_isMessageActive = true;
		}

		#endregion
}
