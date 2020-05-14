using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSorter : MonoBehaviour {

	public TellParent tellParent;

	public Transform exit;

	public Item sortingItem;

	public Renderer iconRenderer;

	void Update () {
		if(tellParent.currentColliders.Count > 0) {
			foreach(Collider col in tellParent.currentColliders) {
				if(col && col.CompareTag("Item")) {
					ItemHandler itemHandler = col.GetComponentInParent<ItemHandler>();
					if(itemHandler) {
						if(itemHandler.item == sortingItem) {
							itemHandler.gameObject.transform.position = exit.position;
							Rigidbody objRB = itemHandler.GetComponent<Rigidbody>();
							if(objRB) {
								objRB.velocity = transform.forward * 2f;
							}
						}
					}
				}
			}
		}
	}

	public void SetItem(Item item) {
		sortingItem = item;
		iconRenderer.gameObject.SetActive(true);
		iconRenderer.material.mainTexture = item.icon.texture;
	}

	public void RemoveItem() {
		sortingItem = null;
		iconRenderer.material.mainTexture = null;
		iconRenderer.gameObject.SetActive(false);
	}
}
