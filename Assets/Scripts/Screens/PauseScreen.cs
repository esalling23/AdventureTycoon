using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    #region Fields

		#endregion

		#region Properties

		#endregion

		#region Methods

    public void HandleClickReturnToGame() {
			gameObject.SetActive(false);
			TimeManager.Instance.PlayTime();
		}

		public void HandleClickExitGame() {
			GameManager.Instance.GameOver(true);
		}

		#endregion
}
