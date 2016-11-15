using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CorruptionBehavior : MonoBehaviour {

	PlayerController player;
	GameObject initialCorruptSeedNode;
	public List <GameObject> corruptNodes;
	bool corruptionStarted = false;
	float cycle;


	void Start(){
		player = FindObjectOfType<PlayerController>();
		cycle = 0;
	}

	void Update(){
		

		//MakeCorrupt
		if (Input.GetKeyDown (KeyCode.Space) && corruptionStarted == false) {
			if (player.allNodes != null) {
				int randomIndex = Random.Range (0, player.allNodes.Count);
				initialCorruptSeedNode = player.allNodes [randomIndex];
				initialCorruptSeedNode.GetComponent<MeshRenderer> ().material.color = Color.red;
				initialCorruptSeedNode.GetComponent<Node> ().nodeState = Node.NodeState.Corrupt;
				player.currentFunds -= (player.currentFunds/10);
				initialCorruptSeedNode.GetComponent<Node> ().illicitFunds += (player.currentFunds/10);
//				corruptionStarted = true;

			}
		}

		//make this probability of a new seed dependent on the corrupt node's number of witnesses
		//WHY AM I GETTING AN INDEX OUT OF RANGE ERROR WHEN I TRY TO ASSIGN THE NEWCORRUPTIONSEED OBJECT?!!!!
		if (Time.time > cycle) {
			if (player.allNodes != null) {
				foreach (GameObject node in player.allNodes) {
					if (node.GetComponent<Node> ().nodeState == Node.NodeState.Corrupt && node.GetComponent<Node>().counted == false) {
						corruptNodes.Add (node);
						node.GetComponent<Node> ().counted = true;
					}
				}

//				int randomIndex = Random.Range (0, corruptNodes.Count);
//				GameObject newCorruptionSeed = corruptNodes [randomIndex];
//				player.currentFunds -= (player.currentFunds/10);
//				newCorruptionSeed.GetComponent<Node> ().illicitFunds += (player.currentFunds/10);
			}

			cycle = Time.time + 60f;
		}
	}

}


