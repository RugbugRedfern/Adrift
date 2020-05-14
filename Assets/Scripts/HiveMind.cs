using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveMind : MonoBehaviour {

	public List<ResourceHandler> worldResources = new List<ResourceHandler>();

	public void AddResource(ResourceHandler handler) {
		worldResources.Add(handler);
	}

	public void RemoveResource(ResourceHandler handler) {
		worldResources.Remove(handler);
	}
}
