using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CorruptionBehavior : MonoBehaviour {

	PlayerController player;
	GameObject initialCorruptSeedNode;
	bool corruptionStarted = false;

	void Start(){
		player = FindObjectOfType<PlayerController>();
	}

	void Update(){

		//MakeCorrupt
		if (Input.GetKeyDown(KeyCode.Space) && corruptionStarted == false){
			if (player.allNodes != null){
				int randomIndex = Random.Range(0, player.allNodes.Count);
				initialCorruptSeedNode = player.allNodes[randomIndex];
				initialCorruptSeedNode.GetComponent<MeshRenderer>().material.color = Color.red;
				initialCorruptSeedNode.GetComponent<Node>().nodeState = Node.NodeState.Corrupt;
				player.currentFunds -= 500;
				initialCorruptSeedNode.GetComponent<Node> ().illicitFunds = 500;
//				corruptionStarted = true;
			}
		}
	}

}


