using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code
{
	public class MenuController : MonoBehaviour, IController
	{
		public TargetButton targetButtonPrefab;
		public RectTransform targetButtonRoot;

		private readonly List<TargetButton> _buttons = new List<TargetButton>();
		private Action _finishedTurn;
		private Entity _myEntity;

		public void SetControlledEntity(Entity entity)
		{
			_myEntity = entity;
		}

		public void AddTargetEntity(Entity entity)
		{
			var targetButton = Instantiate(targetButtonPrefab, targetButtonRoot, false);
			targetButton.InitializeButton(() => HealTarget(entity), $"Repair {entity.displayName}");
			_buttons.Add(targetButton);
		}

		private void HealTarget(Entity target)
		{
			// TODO: wait for animations.
			_myEntity.HealTarget(target);
			FinishTurn();
		}

		public void TakeTurn(Action finishedTurn)
		{
			foreach (var button in _buttons)
			{
				button.gameObject.SetActive(true);
			}

			_finishedTurn = finishedTurn;
		}

		private void FinishTurn()
		{
			foreach (var button in _buttons)
			{
				button.gameObject.SetActive(false);
			}

			_finishedTurn();
		}
	}
}