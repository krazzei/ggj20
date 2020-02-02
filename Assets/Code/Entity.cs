using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code
{
	[RequireComponent(typeof(AudioSource))]
	public class Entity : MonoBehaviour
	{
		private float _health;
		private AudioSource _source;

		[SerializeField]
		private byte maxHealth = 100;

		[SerializeField]
		private byte healAmount = 10;

		[SerializeField]
		private byte damageAmount = 10;

		[SerializeField]
		private byte startingHealth = 0;

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
			target.TakeHeal(healAmount);
		}

		public void DamageTarget(Entity target)
		{
			Debug.Log($"{displayName} is damaging {target.displayName}");
			_source.PlayOneShot(_attackSounds[Random.Range(0, _attackSounds.Count)]);
			target.TakeDamage(damageAmount);
		}

		public void PublishHealthPercent()
		{
			UpdateHealthPercent?.Invoke(_health / maxHealth);
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
				OnFullHealth?.Invoke();
			}

			PublishHealthPercent();

			Debug.Log($"{displayName}'s health: {_health}");
		}

		/// <summary>
		/// Subtracts health, don't be dumb and pass a negative amount. 
		/// </summary>
		/// <param name="amount"></param>
		private void TakeDamage(byte amount)
		{
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
	}
}