using System;
using UnityEngine;
using UnityEngine.UI;

namespace Code
{
	[RequireComponent(typeof(Slider))]
	public class HealthBar : MonoBehaviour
	{
		private Slider _slider;

		private void Awake()
		{
			_slider = GetComponent<Slider>();
		}

		public void SetPosition(Vector2 minAnchor, Vector2 maxAnchor, Vector2 position)
		{
			var rectTransform = _slider.GetComponent<RectTransform>();
			rectTransform.anchorMin = minAnchor;
			rectTransform.anchorMax = maxAnchor;
			rectTransform.anchoredPosition = position;
		}

		public void UpdateHealthPercent(float percent)
		{
			_slider.value = percent;
		}
	}
}