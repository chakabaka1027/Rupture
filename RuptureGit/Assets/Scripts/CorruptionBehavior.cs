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
		smallCycle = 45;
		bigCycle = 200;
	}

	void Update(){
		

		//MakeCorrupt
		if ((Input.GetKeyDown (KeyCode.Space) || (player.allNodes.Count > 20 && Time.time > 90)) && corruptionStarted == false) {
			int randomIndex = Random.Range (0, player.allNodes.Count);
			initialCorruptSeedNode = player.allNodes [randomIndex];
			initialCorruptSeedNode.GetComponent<Node> ().nodeState = Node.NodeState.Corrupt;
			player.currentFunds -= (player.currentFunds/10);
			initialCorruptSeedNode.GetComponent<Node> ().illicitFunds += (player.currentFunds/10);
			corruptionStarted = true;
		}

		//make this probability of a new seed dependent on the corrupt node's number of witnesses
		if (Time.time > smallCycle && corruptionStarted) {
			
			if (player.allNodes != null) {
				foreach (GameObject node in player.allNodes) {
					if (node.GetComponent<Node> ().nodeState == Node.NodeState.Corrupt) {
						corruptNodes.Add (node);
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

			smallCycle = Time.time + 45f;
			corruptNodes.Clear ();

		}

		if (Time.time > bigCycle && corruptionStarted) {

			nonCorruptNodes.Clear();

			Debug.Log ("searching for noncorrupt nodes");


			foreach (GameObject node in player.allNodes) {
				if (node.GetComponent<Node> ().nodeState != Node.NodeState.Corrupt) {
					nonCorruptNodes.Add (node);
				}
			}

			Debug.Log ("there are " + nonCorruptNodes.Count + " noncorrupt nodes");

			if (nonCorruptNodes.Count > 0) {
				for (int i = 0; i <= 3; i++){
					if (nonCorruptNodes [i] != null && nonCorruptNodes [i].GetComponent<Node> ().observableNodes.Count <= 5) {
						nonCorruptNodes [i].GetComponent<Node> ().nodeState = Node.NodeState.Corrupt;
						player.currentFunds -= (player.currentFunds / 20);
						nonCorruptNodes [i].GetComponent<Node> ().illicitFunds += (player.currentFunds / 20);
					} else if (nonCorruptNodes [i] != null && nonCorruptNodes [i].GetComponent<Node> ().observableNodes.Count <= 7) {
						nonCorruptNodes [i].GetComponent<Node> ().nodeState = Node.NodeState.Corrupt;
						player.currentFunds -= (player.currentFunds / 20);
						nonCorruptNodes [i].GetComponent<Node> ().illicitFunds += (player.currentFunds / 20);
					}

					Debug.Log("New node corrupted");
				}
			}
											
			bigCycle = Time.time + 150;
		}
	}

}


