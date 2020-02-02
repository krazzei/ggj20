using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code
{
	[RequireComponent(typeof(AudioSource))]
	public class Entity : MonoBehaviour
	{
		#region Supporting Types
		public enum StatusType
		{
			Attacking,
			Healing,
			Mitigation,
		}
	
		[Serializable]
		public class StatusEffect
		{
			[NonSerialized]
			public StatusType EffectType;
			public float amount;
			public int duration;
		}
		#endregion Supporting Types
		
		private float _health;
		private AudioSource _source;

		private float _damageMultiplier = 1;
		private float _healMultiplier = 1;
		private float _damageMitigation = 1;

		private List<StatusEffect> _activeEffects = new List<StatusEffect>();

		[SerializeField]
		private float maxHealth = 100;

		[SerializeField]
		private float healAmount = 10;

		[SerializeField]
		private float damageAmount = 10;

		[SerializeField]
		private float startingHealth = 0;

		[SerializeField]
		private StatusEffect debuffDamage;

		[SerializeField]
		private StatusEffect buffDamage;

		[SerializeField]
		private StatusEffect buffHeal;

		[SerializeField]
		private StatusEffect shield;
		
		[SerializeField]
		private List<AudioClip> _healSounds = new List<AudioClip>();

		[SerializeField]
		private List<AudioClip> _attackSounds = new List<AudioClip>();
		
		[SerializeField]
		private List<AudioClip> _damageSounds = new List<AudioClip>();

		public string displayName;

		public event Action OnDeath;
		public event Action OnFullHealth;
		public event Action<float> UpdateHealthPercent;

		private void Awake()
		{
			_source = GetComponent<AudioSource>();
		}

		private void Start()
		{
			_health = startingHealth;
			PublishHealthPercent();
		}

		public void HealTarget(Entity target)
		{
			Debug.Log($"{displayName} is healing {target.displayName}");
			_source.PlayOneShot(_healSounds[Random.Range(0, _healSounds.Count)]);
			target.TakeHeal((byte)(healAmount * _healMultiplier));
		}

		public void DamageTarget(Entity target, float damageMultiplier)
		{
			Debug.Log($"{displayName} is damaging {target.displayName}");
			_source.PlayOneShot(_attackSounds[Random.Range(0, _attackSounds.Count)]);
			target.TakeDamage((byte)(damageAmount * damageMultiplier * _damageMultiplier));
		}

		public void DebuffTargetAttack(Entity target)
		{
			debuffDamage.EffectType = StatusType.Attacking;
			target.AddStatusEffect(debuffDamage);
		}

		public void BuffTargetAttack(Entity target)
		{
			buffDamage.EffectType = StatusType.Attacking;
			target.AddStatusEffect(buffDamage);
		}

		public void BuffTargetHealing(Entity target)
		{
			buffHeal.EffectType = StatusType.Healing;
			target.AddStatusEffect(buffHeal);
		}

		public void ShieldTarget(Entity target)
		{
			buffHeal.EffectType = StatusType.Mitigation;
			target.AddStatusEffect(shield);
		}

		public void StunTarget(Entity target)
		{
			throw new NotImplementedException("Do this");
		}

		public void Upkeep()
		{
			// I think stun will have to be a special case?
			var markedForRemoval = new List<StatusEffect>();
			foreach (var activeEffect in _activeEffects)
			{
				if (--activeEffect.duration <= 0)
				{
					UnapplyStatusEffect(activeEffect);
					markedForRemoval.Add(activeEffect);
				}
			}

			markedForRemoval.ForEach(se => _activeEffects.Remove(se));
		}

		private void PublishHealthPercent()
		{
			UpdateHealthPercent?.Invoke(_health / maxHealth);
		}

		/// <summary>
		/// adds health, don't be dumb and pass a negative amount.
		/// </summary>
		/// <param name="amount"></param>
		private void TakeHeal(float amount)
		{
			_health += amount;
			if (_health >= maxHealth)
			{
				_health = maxHealth;
				OnFullHealth?.Invoke();
			}

			PublishHealthPercent();

			Debug.Log($"{displayName}'s health: {_health}");
		}

		/// <summary>
		/// Subtracts health, don't be dumb and pass a negative amount. 
		/// </summary>
		/// <param name="amount"></param>
		private void TakeDamage(float amount)
		{
			amount *= _damageMitigation;
			_source.PlayOneShot(_damageSounds[Random.Range(0, _damageSounds.Count)]);
			_health -= amount;
			if (_health <= 0)
			{
				_health = 0;
				OnDeath?.Invoke();
			}

			PublishHealthPercent();
			
			Debug.Log($"{displayName}'s health: {_health}");
		}

		private void AddStatusEffect(StatusEffect effect)
		{
			// TODO: obviously need to test the StatusEffect system as a whole, but also make sure the math for
			// the multipliers are correct!
			switch (effect.EffectType)
			{
				case StatusType.Attacking:
					_damageMultiplier += effect.amount;
					break;
				// I think I want to combine attacking and healing, the player will just do "negative" damage
				// and the TakeDamage method will just check for both Full Health and Dead.
				case StatusType.Healing:
					_healMultiplier += effect.amount;
					break;
				case StatusType.Mitigation:
					_damageMitigation += effect.amount;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
			_activeEffects.Add(effect);
		}

		private void UnapplyStatusEffect(StatusEffect effect)
		{
			switch (effect.EffectType)
			{
				case StatusType.Attacking:
					_damageMultiplier -= effect.amount;
					break;
				case StatusType.Healing:
					_healMultiplier -= effect.amount;
					break;
				case StatusType.Mitigation:
					_damageMitigation -= effect.amount;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}