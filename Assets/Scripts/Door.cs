using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	Animator anim;
	[HideInInspector] public bool open = false;

	void Start() {
		anim = GetComponent<Animator>();
	}

	public void ToggleOpen() {
		open = !open;
		anim.SetBool("Open", open);
	}
}
