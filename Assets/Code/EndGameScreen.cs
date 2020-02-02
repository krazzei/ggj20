using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code
{
	public class EndGameScreen : MonoBehaviour
	{
		public Text message;

		public void ReloadScene()
		{
			SceneManager.LoadScene(0);
		}

		public void Exit()
		{
			Application.Quit();
		}
	}
}