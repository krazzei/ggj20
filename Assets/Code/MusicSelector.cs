using UnityEngine;

namespace Code
{
	[RequireComponent(typeof(AudioSource))]
	public class MusicSelector : MonoBehaviour
	{
		private AudioSource _source;
		
		public AudioClip intro;
		public AudioClip loop;

		private void Awake()
		{
			_source = GetComponent<AudioSource>();
		}

		private void Start()
		{
			// var musicIndex = Random.Range(0, _music.Count);
			// _source.clip = _music[musicIndex];
			// _source.loop = true;
			// _source.Play();
			_source.PlayOneShot(intro);
			_source.clip = loop;
			_source.loop = true;
			Debug.Log(intro.length);
			_source.PlayScheduled(AudioSettings.dspTime + intro.length);
		}
	}
}