using System;
using System.Collections;
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
			CreateButton(_myEntity, BuffTargetHeal, "Buff {0}");
			CreateButton(_myEntity, ShieldTarget, "Shield {0}");
			CreateButton(_myEntity, HealTarget, "Heal {0}");
		}

		public void AddTargetEntity(Entity entity)
		{
			CreateButton(entity, HealTarget, "Heal {0}");
			CreateButton(entity, DebuffTargetAttack, "Debuff {0}");
		}

		private void CreateButton(Entity target, Action<Entity> action, string displayTextFormat)
		{
			var targetButton = Instantiate(targetButtonPrefab, targetButtonRoot, false);
			targetButton.InitializeButton(() => action(target), string.Format(displayTextFormat, target.displayName));
			_buttons.Add(targetButton);
		}

		private void HealTarget(Entity target)
		{
			// TODO: wait for animations.
			_myEntity.HealTarget(target);
			FinishTurn();
		}

		private void DebuffTargetAttack(Entity target)
		{
			_myEntity.DebuffTargetAttack(target);
			FinishTurn();
		}

		private void BuffTargetHeal(Entity target)
		{
			_myEntity.BuffTargetHealing(target);
			FinishTurn();
		}

		private void ShieldTarget(Entity target)
		{
			_myEntity.ShieldTarget(target);
			FinishTurn();
		}

		private void StunTarget(Entity target)
		{
			throw new NotImplementedException("Do this");
		}

		public void TakeTurn(Action finishedTurn)
		{
			foreach (var button in _buttons)
			{
				button.gameObject.SetActive(true);
			}
			_myEntity.Upkeep();

			_finishedTurn = finishedTurn;
		}

		private void FinishTurn()
		{
			foreach (var button in _buttons)
			{
				button.gameObject.SetActive(false);
			}

			StartCoroutine(TurnAnimation());
		}

		private IEnumerator TurnAnimation()
		{
			yield return new WaitForSeconds(1f);

			_finishedTurn();
		}
	}
}