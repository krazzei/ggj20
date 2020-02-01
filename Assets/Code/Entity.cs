using UnityEngine;

namespace Code
{
	public class Entity : MonoBehaviour
	{
		private float _health;
		
		[SerializeField]
		private byte maxHealth = 100;
		
		[SerializeField]
		private byte healAmount = 10;
		
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
			Debug.Log($"{displayName} is healing {target.displayName}");
			target.TakeHeal(healAmount);
		}

		public void DamageTarget(Entity target)
		{
			Debug.Log($"{displayName} is damaging {target.displayName}");
			target.TakeDamage(damageAmount);
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
