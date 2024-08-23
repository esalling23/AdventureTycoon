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
			GameManager.Instance.StartGame();
		}

		public void ExitGame() {
			// _confirmExitPrompt .SetActive(true);
			ConfirmExitGame();
		}

		public void ConfirmExitGame() {
			#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
			#else
				UnityEngine.Application.Quit();
			#endif
		}

		#endregion
}
