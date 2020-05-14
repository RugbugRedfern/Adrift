using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour {

	[SerializeField] GameObject slider;
	[SerializeField] Vector2 sliderMinMax;
	Animator anim;
	public Sound[] songs;
	[HideInInspector] public int songNum = -1;

	SettingsManager settingsManager;

	void Awake() { // The radio ignores pitch
		settingsManager = FindObjectOfType<SettingsManager>();
		anim = GetComponent<Animator>();
		foreach(Sound s in songs) {
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = true;
			s.source.dopplerLevel = 0;
			s.source.spatialBlend = 1;
			s.source.volume = s.volume;
			s.source.rolloffMode = AudioRolloffMode.Linear;
			s.source.maxDistance = 50f;
			s.source.minDistance = 0f;
			s.source.outputAudioMixerGroup = settingsManager.audioMixer.FindMatchingGroups("Master")[0];
		}
		UpdateGraphics();
	}
	
	public void ChangeSong() {
		songNum++;
		if(songNum >= songs.Length) {
			songNum = -1;
			StopSongs();
			UpdateGraphics();
			return;
		}

		for(int i = 0; i < songs.Length; i++) {
			if(i == songNum) {
				songs[i].source.Play();
			} else {
				if(songs[i].source.isPlaying) {
					songs[i].source.Stop();
				}
			}
		}
		UpdateGraphics();
	}

	public void SetSong(int song) {
		songNum = song;

		if(songNum == -1) {
			UpdateGraphics();
			return;
		}

		for(int i = 0; i < songs.Length; i++) {
			if(i == songNum) {
				songs[i].source.Play();
			} else {
				if(songs[i].source.isPlaying) {
					songs[i].source.Stop();
				}
			}
		}

		UpdateGraphics();
	}

	void UpdateGraphics() {
		Vector3 newPos = slider.transform.localPosition;
		newPos.y = Mathf.Lerp(sliderMinMax.x, sliderMinMax.y, (float)(songNum + 1) / (float)songs.Length);
		slider.transform.localPosition = newPos;
		if(songNum == -1) {
			anim.SetBool("playing", false);
		} else {
			anim.SetBool("playing", true);
		}
	}

	void StopSongs() {
		foreach(Sound s in songs) {
			s.source.Stop();
		}
	}
}