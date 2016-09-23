using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CorruptionBehavior : MonoBehaviour {

	PlayerController player;
	GameObject corruptNodeObject;

	public int whistleblowerPercentChance = 25;

	void Start(){
		player = FindObjectOfType<PlayerController>();
	}

	void Update(){

		//MakeCorrupt
		if (Input.GetKeyDown(KeyCode.Space)){
			if (player.allNodes != null){
				int randomIndex = Random.Range(0, player.allNodes.Count);
				corruptNodeObject = player.allNodes[randomIndex];
				corruptNodeObject.GetComponent<MeshRenderer>().material.color = Color.red;
				corruptNodeObject.GetComponent<Node>().nodeState = Node.NodeState.Corrupt;
				RunAlgorithm();
			}
		}
	}

	void RunAlgorithm(){

		Node corruptNode = corruptNodeObject.GetComponent<Node>();
		corruptNode.acceptedFunds = 500;
		List<Node> witnesses = corruptNode.observableNodes;
		for (int i = 0; i < witnesses.Count; i++){
			if (corruptNode.acceptedFunds > witnesses[i].minimumThreshold){
				//pay off witness
				corruptNode.acceptedFunds -= witnesses[i].minimumThreshold;
				witnesses[i].nodeState = Node.NodeState.Corrupt;
				witnesses[i].gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
				witnesses[i].acceptedFunds += witnesses[i].minimumThreshold;
			} else if (corruptNode.acceptedFunds < witnesses[i].minimumThreshold){
				//witness can become a whistleblower or remain witness
				int chance = Random.Range (1, 100);
				if (chance < whistleblowerPercentChance){
					witnesses[i].nodeState = Node.NodeState.Whistleblower;
					witnesses[i].gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
				} else {
					witnesses[i].nodeState = Node.NodeState.Witness;
					witnesses[i].gameObject.GetComponent<MeshRenderer>().material.color = Color.green;

				}
			}
		}
	}
}
