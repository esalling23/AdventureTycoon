using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    #region Fields

		

		#endregion

		#region Properties

		public string Property { get; set; }

		#endregion

		#region Methods

    public void HandleClickReturnToGame() {
			gameObject.SetActive(false);
			TimeManager.Instance.PlayTime();
		}

		public void HandleClickExitGame() {
			SceneManager.LoadScene("StartScreen");
		}

		#endregion
}
