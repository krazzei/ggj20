using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code
{
	public class Entity : MonoBehaviour
	{
		private Action _finishedTurn;
		private float _health;
		private bool _isTurn;

		[FormerlySerializedAs("MaxHealth")]
		[SerializeField]
		private byte maxHealth = 100;

		[FormerlySerializedAs("HealAmount")]
		[SerializeField]
		private byte healAmount = 10;

		[FormerlySerializedAs("DamageAmount")]
		[SerializeField]
		private byte damageAmount = 10;

		[SerializeField]
		private byte startingHealth = 0;

		public string displayName;

		private void Start()
		{
			_health = startingHealth;
		}

		public void HealTarget(Entity target)
		{
			if (!_isTurn)
			{
				return;
			}
			
			Debug.Log($"{displayName} is healing {target.displayName}");
			target.TakeHeal(healAmount);
			_finishedTurn();
			_isTurn = false;
		}

		public void DamageTarget(Entity target)
		{
			if (!_isTurn)
			{
				return;
			}
			
			Debug.Log($"{displayName} is damaging {target.displayName}");
			target.TakeDamage(damageAmount);
			_finishedTurn();
			_isTurn = false;
		}

		public void TakeTurn(Action finishedTurn)
		{
			Debug.Log($"{displayName} is doing a turn");
			_isTurn = true;
			_finishedTurn = finishedTurn;
			// We wait for a controller to tell us to do something
		}

		/// <summary>
		/// adds health, don't be dumb and pass a negative amount.
		/// </summary>
		/// <param name="amount"></param>
		private void TakeHeal(byte amount)
		{
			_health += amount;
			if (_health >= maxHealth)
			{
				_health = maxHealth;
				//HealthFull();
			}
			Debug.Log($"{displayName}'s health: {_health}");
		}

		/// <summary>
		/// Subtracts health, don't be dumb and pass a negative amount. 
		/// </summary>
		/// <param name="amount"></param>
		private void TakeDamage(byte amount)
		{
			_health -= amount;
			if (_health <= 0)
			{
				_health = 0;
				//DoDeath();
			}
			Debug.Log($"{displayName}'s health: {_health}");
		}
	}
}