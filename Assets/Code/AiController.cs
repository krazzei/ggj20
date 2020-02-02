using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code
{
	// TODO: With the new ability types we might want to have some more logic for deciding what is best for the Ai.
	public class AiController : MonoBehaviour, IController
	{
		#region Supporting Types
		// TODO: The AiController can just know who the player is... if that matters.
		private enum EnemyAttackType
		{
			Single,
			AreaOfEffect,
			Others,
			Player
		}

		[Serializable]
		private class AttackType
		{
			public EnemyAttackType type = EnemyAttackType.Others;
			public float damageMultiplier = 1;
			public float weight = 0.1f;
		}
		#endregion Supporting Types
		
		private Action _finishedTurn;
		private Entity _myEntity;
		private Entity _player;

		private readonly List<Entity> _allEntities = new List<Entity>();
		[SerializeField]
		private List<AttackType> _attackTypes = new List<AttackType>();

		public void TakeTurn(Action finishedTurn)
		{
			_finishedTurn = finishedTurn;
			_myEntity.Upkeep();
			DoTurn();
		}

		public void SetControlledEntity(Entity entity)
		{
			_myEntity = entity;
			_allEntities.Add(entity);
		}

		public void SetPlayerEntity(Entity player)
		{
			_player = player;
			_allEntities.Add(player);
		}

		public void AddTargetEntity(Entity target)
		{
			_allEntities.Add(target);
		}

		private void DoTurn()
		{
			StartCoroutine(TurnAnimation());
			var randomWeight = Random.Range(0f, 1f);
			var totalWeight = 0f;
			AttackType selectedAttack = null;
			foreach (var attackType in _attackTypes)
			{
				if (randomWeight < attackType.weight + totalWeight)
				{
					selectedAttack = attackType;
					break;
				}

				totalWeight += attackType.weight;
			}

			switch (selectedAttack?.type)
			{
				case EnemyAttackType.Single:
					_myEntity.DamageTarget(_myEntity, selectedAttack.damageMultiplier);
					break;
				case EnemyAttackType.AreaOfEffect:
					foreach (var entity in _allEntities)
					{
						_myEntity.DamageTarget(entity, selectedAttack.damageMultiplier);
					}
					break;
				case EnemyAttackType.Others:
					foreach (var entity in _allEntities)
					{
						if (entity != _myEntity)
						{
							_myEntity.DamageTarget(entity, selectedAttack.damageMultiplier);
						}
					}
					break;
				case EnemyAttackType.Player:
					_myEntity.DamageTarget(_player, selectedAttack.damageMultiplier);
					break;
				case null:
					break;
				default:
					Debug.Log($"Unknown attack type {selectedAttack.type}");
					break;
			}
		}

		private IEnumerator TurnAnimation()
		{
			// TODO: use animation time
			yield return new WaitForSeconds(1.0f);
			_finishedTurn();
		}
	}
}