using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code
{
	[RequireComponent(typeof(AudioSource), typeof(Animation))]
	public class Entity : MonoBehaviour
	{
		// TODO: move these to different files.
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

			public StatusEffect Clone()
			{
				return new StatusEffect {EffectType = EffectType, amount = amount, duration = duration};
			}
		}
		#endregion Supporting Types
		
		private float _health;
		private AudioSource _source;

		private float _damageMultiplier = 1;
		private float _healMultiplier = 1;
		private float _damageMitigation = 1;

		private Animation _animation;

		private readonly List<StatusEffect> _activeEffects = new List<StatusEffect>();
		private readonly Queue<StatusEffect> _queuedEffects = new Queue<StatusEffect>();

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

		[SerializeField]
		private AnimationClip _attack;
		
		[SerializeField]
		private AnimationClip _idle;
		
		[SerializeField]
		private AnimationClip _hit;
		
		[SerializeField]
		private AnimationClip _buff;

		public string displayName;

		public event Action OnDeath;
		public event Action OnFullHealth;
		public event Action<float> UpdateHealthPercent;
		public event Action<StatusEffect> OnAddStatusEffect;
		public event Action<StatusEffect> OnRemoveStatusEffect;

		private void Awake()
		{
			_source = GetComponent<AudioSource>();
			_animation = GetComponent<Animation>();
		}

		private void Start()
		{
			_health = startingHealth;
			PublishHealthPercent();
		}

		public float HealTarget(Entity target)
		{
			_source.PlayOneShot(_healSounds[Random.Range(0, _healSounds.Count)]);
			StartCoroutine(PlayAnimationThenReturnToIdle(_attack));
			target.TakeHeal(healAmount * _healMultiplier);
			return _attack.length;
		}

		public float DamageTarget(Entity target, float damageMultiplier)
		{
			_source.PlayOneShot(_attackSounds[Random.Range(0, _attackSounds.Count)]);
			StartCoroutine(PlayAnimationThenReturnToIdle(_attack));
			target.TakeDamage((byte)(damageAmount * damageMultiplier * _damageMultiplier));
			return _attack.length;
		}

		public float DebuffTargetAttack(Entity target)
		{
			debuffDamage.EffectType = StatusType.Attacking;
			StartCoroutine(PlayAnimationThenReturnToIdle(_attack));
			target.QueueStatusEffect(debuffDamage);
			return _attack.length;
		}

		public float BuffTargetAttack(Entity target)
		{
			buffDamage.EffectType = StatusType.Attacking;
			StartCoroutine(PlayAnimationThenReturnToIdle(_buff));
			target.QueueStatusEffect(buffDamage);
			return _buff.length;
		}

		public float BuffTargetHealing(Entity target)
		{
			buffHeal.EffectType = StatusType.Healing;
			StartCoroutine(PlayAnimationThenReturnToIdle(_buff));
			target.QueueStatusEffect(buffHeal);
			return _buff.length;
		}

		public float ShieldTarget(Entity target)
		{
			shield.EffectType = StatusType.Mitigation;
			StartCoroutine(PlayAnimationThenReturnToIdle(_buff));
			target.QueueStatusEffect(shield);
			return _buff.length;
		}

		public void StunTarget(Entity target)
		{
			throw new NotImplementedException("Do this");
		}

		/// <summary>
		/// Happens at start of turn.
		/// </summary>
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
					OnRemoveStatusEffect?.Invoke(activeEffect);
				}
			}

			markedForRemoval.ForEach(se => _activeEffects.Remove(se));

			while (_queuedEffects.Count > 0)
			{
				var effect = _queuedEffects.Dequeue();
				AddStatusEffect(effect);
				OnAddStatusEffect?.Invoke(effect);
			}
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
			StartCoroutine(PlayAnimationThenReturnToIdle(_hit));
			_health += amount;
			if (_health >= maxHealth)
			{
				_health = maxHealth;
				OnFullHealth?.Invoke();
			}

			PublishHealthPercent();
		}

		/// <summary>
		/// Subtracts health, don't be dumb and pass a negative amount. 
		/// </summary>
		/// <param name="amount"></param>
		private void TakeDamage(float amount)
		{
			StartCoroutine(PlayAnimationThenReturnToIdle(_hit));
			amount *= _damageMitigation;
			_source.PlayOneShot(_damageSounds[Random.Range(0, _damageSounds.Count)]);
			_health -= amount;
			if (_health <= 0)
			{
				_health = 0;
				OnDeath?.Invoke();
			}

			PublishHealthPercent();
		}

		private void QueueStatusEffect(StatusEffect effect)
		{
			_queuedEffects.Enqueue(effect.Clone());	
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
					_damageMitigation -= effect.amount;
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
					_damageMitigation += effect.amount;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private IEnumerator PlayAnimationThenReturnToIdle(AnimationClip animationClip)
		{
			_animation.CrossFade(animationClip.name, 0.1f);
			yield return new WaitForSeconds(animationClip.length + 0.1f);
			_animation.CrossFade(_idle.name, 0.1f);
		}
	}
}