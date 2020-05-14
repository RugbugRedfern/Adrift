using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour {

	public float[] speeds = { 0f, 0.25f, 0.5f, 1f, 2f, 4f, 8f };
	public int speedNum = 0;

	bool active = false;

	Animator anim;

	public TellParent tellParent;
	new AudioSource audio;

	void Awake() {
		anim = GetComponent<Animator>();
		audio = GetComponent<AudioSource>();
		audio.outputAudioMixerGroup = FindObjectOfType<SettingsManager>().audioMixer.FindMatchingGroups("Master")[0];
	}

	void Start() {
		UpdateActive();
		UpdateSpeed();
	}

	void FixedUpdate() {
		if(tellParent.currentColliders.Count > 0 && speeds[speedNum] != 0f) {
			foreach(Collider col in tellParent.currentColliders) {
				if(col) {
					Rigidbody itemRB = col.GetComponent<Rigidbody>();
					if(!itemRB) {
						itemRB = col.GetComponentInParent<Rigidbody>();
					}
					if(itemRB) {
						itemRB.velocity = (itemRB.velocity + transform.forward).normalized * speeds[speedNum];
						//itemRB.AddForce((transform.forward * speeds[speedNum]) - Vector3.Project(itemRB.velocity - Vector3.one, transform.forward.normalized));
						//itemRB.AddRelativeForce(transform.forward * speeds[speedNum] - itemRB.velocity); // TODO: PUT IN FIXEDUPDATE!!!
					}
				}
			}
		}
	}

	public void SetSpeed(int speed) {
		speedNum = speed;
		if(speedNum >= speeds.Length) {
			speedNum = 0;
		}
		if(speeds[speedNum] == 0) {
			active = false;
			UpdateActive();
		} else if(!active) {
			active = true;
			UpdateActive();
		}
		UpdateSpeed();
	}

	public void IncreaseSpeed() {
		speedNum++;
		if(speedNum >= speeds.Length) {
			speedNum = 0;
		}
		if(speeds[speedNum] == 0) {
			active = false;
			UpdateActive();
		} else if(!active) {
			active = true;
			UpdateActive();
		}
		UpdateSpeed();
	}

	public void ToggleActive() {
		active = !active;
		UpdateActive();
	}

	void UpdateActive() {
		anim.SetBool("Active", active);
		audio.mute = !active;
	}

	void UpdateSpeed() {
		anim.SetFloat("Speed", speeds[speedNum]);
		audio.pitch = 0.5f + speeds[speedNum] / 8f;
	}
}
