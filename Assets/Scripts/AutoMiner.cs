using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutoMiner : MonoBehaviour {

	NavMeshAgent agent;

	HiveMind hive;

	public GameObject toolHolder;

	[HideInInspector] public ResourceHandler target;

	float interactRange = 2.5f;

	Animator animator;

	bool moving = false;
	bool gathering = false;

	public List<Item> items = new List<Item>();
	public List<int> itemAmounts = new List<int>();

	float gatherTime = 0f;

	public Item currentToolItem = null;
	GameObject currentToolObj = null;

	PlayerController player;

	void Start() {
		hive = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<HiveMind>();
		player = FindObjectOfType<PlayerController>();
		animator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		GetComponent<AudioSource>().outputAudioMixerGroup = FindObjectOfType<SettingsManager>().audioMixer.FindMatchingGroups("Master")[0];
	}
	
	void Update() {
		if(agent.isOnNavMesh) {
			if(currentToolItem) {
				if(target) {
					if(!moving) {
						moving = true;
						animator.SetBool("Moving", moving);
					}
					if(Vector3.Distance(transform.position, target.transform.position) <= interactRange) {
						if(!gathering) {
							gathering = true;
							animator.SetBool("Gathering", gathering);
						}
						if(moving) {
							moving = false;
							animator.SetBool("Moving", moving);
						}
						gatherTime += Time.deltaTime * currentToolItem.speed;
						if(gatherTime >= target.resource.gatherTime) {
							int i = 0;
							foreach(Item item in target.resource.resourceItems) {
								if(Random.Range(0f, 1f) <= target.resource.chances[i]) {
									AddItem(item, currentToolItem.gatherAmount);
								}
								i++;
							}
							target.Gather(currentToolItem.gatherAmount);
							gatherTime = 0f;
						}
					}
				} else {
					if(gathering) {
						gathering = false;
						gatherTime = 0f;
						animator.SetBool("Gathering", gathering);
					}

					ResourceHandler closestHandler = null;
					float closestDistance = Mathf.Infinity;
					foreach(ResourceHandler resourceHandler in hive.worldResources) {
						if(resourceHandler) {
							float dist = Vector3.Distance(transform.position, resourceHandler.transform.position);
							if(dist < closestDistance) {
								closestDistance = dist;
								closestHandler = resourceHandler;
							}
						}
					}

					if(closestHandler != null) {
						target = closestHandler;
						if(agent.isStopped) {
							agent.isStopped = false;
						}
						//target = hive.worldResources[Random.Range(0, hive.worldResources.Count)];
						agent.SetDestination(target.transform.position);
						if(Vector3.Distance(agent.destination, target.transform.position) > interactRange) {
							target = null; 
						}
					} else {
						if(!agent.isStopped) {
							agent.isStopped = true;
							moving = false;
							animator.SetBool("Moving", moving);
						}
					}
				}
			}
		} else {
			//Debug.Log("Not on NavMesh");
		}
	}

	public void SetTool(Item item) {

		if(currentToolObj) {
			Destroy(currentToolObj);
		}

		if(currentToolItem) {
			player.inventory.AddItem(currentToolItem, 1);
		}

		currentToolItem = item;
		GameObject obj = Instantiate(item.prefab, toolHolder.transform) as GameObject;

		Rigidbody objRB = obj.GetComponent<Rigidbody>();
		if(objRB) {
			objRB.isKinematic = true;
		}
		obj.tag = "Untagged";
		foreach(Transform trans in obj.transform) {
			trans.tag = "Untagged";
		}

		currentToolObj = obj;
	}

	public Item GatherTool() {
		Item returnItem = currentToolItem;
		currentToolItem = null;
		Destroy(currentToolObj);
		return returnItem;
	}

	public void ClearItems() {

		items.Clear();
		itemAmounts.Clear();

	}

	void AddItem(Item item, int amount) {

		bool hasItem = false;

		int i = 0;
		foreach(Item _item in items) {
			if(_item.id == item.id) {
				hasItem = true;
				itemAmounts[i] += amount;
			}
			i++;
		}

		if(!hasItem) {
			items.Add(item);
			itemAmounts.Add(amount);
		}
	}
}
