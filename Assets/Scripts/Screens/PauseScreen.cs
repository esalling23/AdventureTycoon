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
			Time.timeScale = 1;

			gameObject.SetActive(false);
		}

		public void HandleClickExitGame() {
			SceneManager.LoadScene("StartScreen");
		}

		#endregion
}
