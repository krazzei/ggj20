using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code
{
	public enum EnemyAttackType
	{
		Single,
		AreaOfEffect,
	}

	public class AttackType
	{
		public EnemyAttackType Type;
		public float DamageModifier;
	}
	
	public class AiController : MonoBehaviour, IController
	{
		private Action _finishedTurn;
		private Entity _myEntity;

		private readonly List<Entity> _allEntities = new List<Entity>();
		

		public void TakeTurn(Action finishedTurn)
		{
			_finishedTurn = finishedTurn;
			DoTurn();
		}

		public void SetControlledEntity(Entity entity)
		{
			_myEntity = entity;
		}

		public void AddTargetEntity(Entity target)
		{
			_allEntities.Add(target);
		}

		private void DoTurn()
		{
			StartCoroutine(TurnAnimation());
			_myEntity.DamageTarget(_myEntity);
			// TODO: for AOE go through the _allEntities collection.
		}

		private IEnumerator TurnAnimation()
		{
			// TODO: use animation time
			yield return new WaitForSeconds(1.0f);
			_finishedTurn();
		}
	}
}