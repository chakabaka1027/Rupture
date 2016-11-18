using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CorruptionBehavior : MonoBehaviour {

	PlayerController player;
	GameObject initialCorruptSeedNode;
	public List <GameObject> corruptNodes;
	public List <GameObject> nonCorruptNodes;

	bool corruptionStarted = false;
	float smallCycle;
	float bigCycle;


	void Start(){
		player = FindObjectOfType<PlayerController>();
		smallCycle = 60;
		bigCycle = 300;
	}

	void Update(){
		

		//MakeCorrupt
		if ((Input.GetKeyDown (KeyCode.Space) || (player.allNodes.Count > 16 && Time.time > 90)) && corruptionStarted == false) {
			if (player.allNodes != null) {
				int randomIndex = Random.Range (0, player.allNodes.Count);
				initialCorruptSeedNode = player.allNodes [randomIndex];
//				initialCorruptSeedNode.GetComponent<MeshRenderer> ().material.color = Color.red;
				initialCorruptSeedNode.GetComponent<Node> ().nodeState = Node.NodeState.Corrupt;
				player.currentFunds -= (player.currentFunds/10);
				initialCorruptSeedNode.GetComponent<Node> ().illicitFunds += (player.currentFunds/10);
				corruptionStarted = true;

			}
		}

		//make this probability of a new seed dependent on the corrupt node's number of witnesses
		if (Time.time > smallCycle && corruptionStarted) {
			if (player.allNodes != null) {
				foreach (GameObject node in player.allNodes) {
					if (node.GetComponent<Node> ().nodeState == Node.NodeState.Corrupt && node.GetComponent<Node>().counted == false) {
						corruptNodes.Add (node);
						node.GetComponent<Node> ().counted = true;
					}
				}

				if (corruptNodes.Count > 0) {
					int randomIndex = Random.Range (0, corruptNodes.Count);
					GameObject newCorruptionSeed = corruptNodes [randomIndex];
					player.currentFunds -= (player.currentFunds / 10);
					newCorruptionSeed.GetComponent<Node> ().illicitFunds += (player.currentFunds / 10);
					Debug.Log (newCorruptionSeed + " is a new seed");
				}
			}

			smallCycle = Time.time + 60f;
		}

		if (Time.time > bigCycle && corruptionStarted) {
			foreach (GameObject node in player.allNodes) {
				if ((node.GetComponent<Node> ().nodeState == Node.NodeState.Neutral 
					|| node.GetComponent<Node> ().nodeState == Node.NodeState.Witness) 
					&& node.GetComponent<Node> ().counted == false) {
					nonCorruptNodes.Add (node);
					node.GetComponent<Node> ().counted = true;

					if (nonCorruptNodes.Count > 0) {
						foreach (GameObject nonCorruptNode in nonCorruptNodes){
							if (nonCorruptNode.GetComponent<Node>().observableNodes.Count < 5){
								nonCorruptNode.GetComponent<Node> ().nodeState = Node.NodeState.Corrupt;
								player.currentFunds -= (player.currentFunds / 20);
								nonCorruptNode.GetComponent<Node> ().illicitFunds += (player.currentFunds / 20);
							}
						}

//						int randomIndex = Random.Range (0, nonCorruptNodes.Count);
//						GameObject newCorruptionSeed = nonCorruptNodes [randomIndex];
//						player.currentFunds -= (player.currentFunds / 20);
//						newCorruptionSeed.GetComponent<Node> ().illicitFunds += (player.currentFunds / 20);
//						Debug.Log (newCorruptionSeed + " is a newly corrupted seed");
					}
				}
			}

			bigCycle = Time.time + 330;
		}
	}

}


