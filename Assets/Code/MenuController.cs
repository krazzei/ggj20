using UnityEngine;
using UnityEngine.Events;

namespace Code
{
	public class MenuController : MonoBehaviour
	{
		public TargetButton targetButtonPrefab;
		public RectTransform targetButtonRoot;

		public void AddTargetButton(UnityAction callback, string displayText)
		{
			var targetButton = Instantiate(targetButtonPrefab, targetButtonRoot, false);
			targetButton.InitializeButton(callback, displayText);
		}
	}
}