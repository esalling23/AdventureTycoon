using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    #region Fields

		private static MessageManager _instance;

		[SerializeField] private MessagePopup _messageObj;
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

		void Start()
		{
			EventManager.StartListening(EventName.OnMessageBroadcast, HandleOnMessageBroadcast);
		}

		public void CloseMessage(MessagePopup _message)
		{
			// future work - close only this message if multiple showing
			_isMessageActive = false;
		}


		#region EventHandlers

		private void HandleOnMessageBroadcast(Dictionary<string, object> data)
		{
			if (data.TryGetValue("message", out object message))
			{
				// Future work may include more than one message? 
				// Instantiate(
				// 	_messageObj.gameObject,
				// 	Vector3.zero,
				// 	Quaternion.identity,
				// 	gameObject.transform
				// );

				// implementing a single-at-a-time message for now
				_messageObj.ShowMessage(message.ToString());
				_messageObj.gameObject.SetActive(true);
				_isMessageActive = true;
			}
		}

		#endregion

		#endregion
}
