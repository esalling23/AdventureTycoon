using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    #region Fields

		[SerializeField] private GameObject _confirmExitPrompt;

		#endregion

		#region Properties

		public string Property { get; set; }

		#endregion

		#region Methods

    public void StartGame() {
			SceneManager.LoadScene("Game");
		}

		public void ExitGame() {
			_confirmExitPrompt.SetActive(true);
		}

		public void ConfirmExitGame() {
			Application.Quit();
		}

		#endregion
}
