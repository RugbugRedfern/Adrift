using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

	public Sound[] sounds;

	SettingsManager settingsManager;

	void Awake() {
		settingsManager = FindObjectOfType<SettingsManager>();
		foreach(Sound s in sounds) {
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;

			if(settingsManager) {
				s.source.outputAudioMixerGroup = settingsManager.audioMixer.FindMatchingGroups("Master")[0];
			}

			s.source.volume = s.volume;
		}
	}

	public void Play(string name) {
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if(s == null) {
			Debug.LogWarning("Sound: " + name + " not found");
			return;
		}
		s.source.pitch = UnityEngine.Random.Range(s.minPitch, s.maxPitch);
		s.source.Play();
	}
}

[System.Serializable]
public class Sound {

	public string name;

	public AudioClip clip;

	[Range(0f, 1f)]
	public float volume;

	[Range(0f, 1f)]
	public float minPitch;

	[Range(0f, 1f)]
	public float maxPitch;

	[HideInInspector] public AudioSource source;
}