using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class OffersAndDiffusion : MonoBehaviour {

	int whistleblowerPercentChance = 10;
	private float nextCycle;
	Node thisNode;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		thisNode = GetComponent<Node>();

		List<Node> witnesses = thisNode.observableNodes;

		if (thisNode.nodeState == Node.NodeState.Corrupt && Time.time > nextCycle) {

			for (int i = 0; i < witnesses.Count; i++){

				//the chance that a node will accept a bribe is inversely proportional to its number of potential witnesses
				int bribeChance = Random.Range (witnesses [i].minimumThreshold, witnesses [i].minimumThreshold + 1 + witnesses[i].observableNodes.Count);

				if ((thisNode.illicitFunds / 2) > witnesses [i].minimumThreshold && witnesses[i].minimumThreshold == bribeChance) {
					//pay off witness
					if (witnesses [i] != null) {
						thisNode.illicitFunds -= witnesses [i].minimumThreshold;
						witnesses [i].illicitFunds += witnesses [i].minimumThreshold;
						witnesses [i].nodeState = Node.NodeState.Corrupt;	
//						witnesses [i].gameObject.GetComponent<MeshRenderer> ().material.color = Color.red;
					}
				} else if (witnesses[i].nodeState != Node.NodeState.Corrupt){
					witnesses[i].nodeState = Node.NodeState.Witness;
//					witnesses[i].gameObject.GetComponent<MeshRenderer> ().material.color = Color.green;

					int chance = Random.Range(1, 2000);
					if (chance < whistleblowerPercentChance){
						witnesses[i].nodeState = Node.NodeState.Whistleblower;
//						witnesses[i].gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
						Debug.Log (thisNode.name + "Is corrupt!");
					}
				}
			}

			//time before the algorith runs again for each node
			nextCycle = Time.time + 8; 
		}
	}
}
