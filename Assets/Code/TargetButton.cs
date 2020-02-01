using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code
{
	[RequireComponent(typeof(Button))]
	public class TargetButton : MonoBehaviour
	{
		private Button _button;
		private Text _label;

		private void Awake()
		{
			_button = GetComponent<Button>();
			_label = GetComponentInChildren<Text>();
		}

		public void InitializeButton(UnityAction callback, string displayText)
		{
			_button.onClick.AddListener(callback);
			_label.text = displayText;
		}
	}
}