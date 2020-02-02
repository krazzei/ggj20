using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Code
{
	[RequireComponent(typeof(RectTransform))]
	public class StatusEffectIcons : MonoBehaviour
	{
		private enum IconType
		{
			Buff,
			Debuff,
			Shield,
			Stun
		}
		public Image buffIconPrefab;
		public Image debuffIconPrefab;
		public Image shieldIconPrefab;
		public Image stunIconPrefab;

		private RectTransform _iconRoot; 
			
		private readonly List<Entity.StatusEffect> _currentEffects = new List<Entity.StatusEffect>();
		private readonly Dictionary<IconType, List<Image>> _icons = new Dictionary<IconType, List<Image>>();

		private void Awake()
		{
			_iconRoot = GetComponent<RectTransform>();
		}

		public void SetPosition(Vector2 minAnchor, Vector2 maxAnchor, Vector2 position)
		{
			var rectTransform = _iconRoot.GetComponent<RectTransform>();
			rectTransform.anchorMin = minAnchor;
			rectTransform.anchorMax = maxAnchor;
			rectTransform.anchoredPosition = position;
		}
		
		public void AddStatusEffect(Entity.StatusEffect effect)
		{
			_currentEffects.Add(effect);

			switch (effect.EffectType)
			{
				case Entity.StatusType.Attacking:
				case Entity.StatusType.Healing:
					CreateIcon(effect.amount > 0 ? buffIconPrefab : debuffIconPrefab,
								effect.amount > 0 ? IconType.Buff : IconType.Debuff);
					break;
				case Entity.StatusType.Mitigation:
					// shield
					CreateIcon(shieldIconPrefab, IconType.Shield);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void CreateIcon(Image iconPrefab, IconType type)
		{
			var icon = Instantiate(iconPrefab, _iconRoot, false);
			if (!_icons.ContainsKey(type))
			{
				_icons.Add(type, new List<Image>(1));
			}
			var iconList = _icons[type];
			iconList.Add(icon);
		}
		
		public void RemoveStatusEffect(Entity.StatusEffect effect)
		{
			_currentEffects.Remove(effect);
			switch (effect.EffectType)
			{
				case Entity.StatusType.Attacking:
				case Entity.StatusType.Healing:
					DestroyIcon(effect.amount > 0 ? IconType.Buff : IconType.Debuff);
					break;
				case Entity.StatusType.Mitigation:
					DestroyIcon(IconType.Shield);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void DestroyIcon(IconType type)
		{
			var iconList = _icons[type];
			var icon = iconList.First();
			if (icon != null)
			{
				Destroy(icon.gameObject);
				iconList.Remove(icon);
			}
		}
	}
}