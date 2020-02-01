using System.Collections.Generic;
using UnityEngine;

namespace Code
{
	[RequireComponent(typeof(AudioSource))]
	public class MusicSelector : MonoBehaviour
	{
		private AudioSource _source;
		
		public List<AudioClip> _music;

		private void Awake()
		{
			_source = GetComponent<AudioSource>();
		}

		private void Start()
		{
			var musicIndex = Random.Range(0, _music.Count);
			_source.clip = _music[musicIndex];
			_source.loop = true;
			_source.Play();
		}
	}
}