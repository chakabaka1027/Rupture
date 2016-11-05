using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Investigations : MonoBehaviour {

	PlayerController player;
	Office parentOffice;
	GameObject selectedNode;

	public LayerMask bureacratLayer;
	int cursoryCost = 200;
	int thoroughCost = 350;


	// Use this for initialization
	void Start () {
		player = FindObjectOfType<PlayerController> ();	
		Debug.Log ("bada bing");
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0) && player.playerState == PlayerController.State.Cursory && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (-1) == false) {
		
			CursoryInvestigation ();
		}

		if (Input.GetMouseButtonDown (0) && player.playerState == PlayerController.State.Thorough && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (-1) == false) {

			ThoroughInvestigation ();
		}
	
	}
		
	void CursoryInvestigation(){

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, bureacratLayer)){

			selectedNode = hit.collider.gameObject;

			if (player.currentFunds >= cursoryCost){
				player.currentFunds -= cursoryCost;

				if (selectedNode.GetComponent<Node>().nodeState == Node.NodeState.Corrupt) {
					RemoveNodeFromLists (selectedNode);
					Destroy (selectedNode);

					for (int i = 0; i < player.allNodes.Count; i++) {
						if (player.allNodes [i] == null) {
							Destroy(player.allNodes[i]);
						}
					}

					foreach (GameObject node in player.allNodes) {
						node.GetComponent<Node>().UpdateWitnessableNodes ();
					}
						
				}
			}
		}
	}

		
	void ThoroughInvestigation(){

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, bureacratLayer)){

			selectedNode = hit.collider.gameObject;

			if (player.currentFunds >= thoroughCost){
				player.currentFunds -= thoroughCost;

				if (selectedNode.GetComponent<Node>().nodeState == Node.NodeState.Corrupt) {
					
					player.currentFunds += selectedNode.GetComponent<Node> ().illicitFunds;
					player.currentFunds += Accomplice (selectedNode).GetComponent<Node> ().illicitFunds;

					RemoveNodeFromLists (selectedNode);
					RemoveNodeFromLists (Accomplice(selectedNode));

					GameObject.Destroy (Accomplice (selectedNode));
					GameObject.Destroy (selectedNode);

					for (int i = 0; i < player.allNodes.Count; i++) {
						if (player.allNodes [i] == null) {
							Destroy(player.allNodes[i]);
						}
					}

					foreach (GameObject node in player.allNodes) {
						node.GetComponent<Node>().UpdateWitnessableNodes ();
					}
						

				}
			}
		}
	}


	void RemoveNodeFromLists(GameObject node){

		parentOffice = node.GetComponentInParent<Office> ();

//		//Alex's suggested way of serving the same purpose intended to be carreied out by the foreach loop below
		foreach (GameObject observableNode in player.allNodes) {
			observableNode.GetComponent<Node> ().observableNodes.Remove (node.GetComponent<Node>());
		}
		parentOffice.officeMembers.Remove (node.GetComponent<Node> ());
		parentOffice.officeCount --;

//		foreach (Node observableNode in node.GetComponent<Node> ().observableNodes) {
//			if (node != null) {
//				
//				observableNode.observableNodes.Remove (node.GetComponent<Node> ());
//			}
//		}

		if (node.GetComponent<Node> ().isSupervisor) {
			//connected offices are offices you see into
			//connecting offices are ones that you are seen by

			foreach (Office connectedOffice in parentOffice.connectedOffices) {
				connectedOffice.connectingOffices.Remove (parentOffice);
			}
			//connected offices are offices you see into
			parentOffice.connectedOffices.Clear();

			//destroys the network ties/flow emerging from each office
			foreach (GameObject outgoingFlow in parentOffice.outgoingNetworkFlows) {
				Destroy (outgoingFlow);
			}

			foreach (GameObject outgoingLine in parentOffice.outgoingNetworkLines) {
				Destroy (outgoingLine);
			}

			foreach(GameObject office in player.allOffices){
				office.GetComponent<Office> ().aggregateOfficeList.Remove (parentOffice);
			}

			parentOffice.outgoingNetworkFlows.Clear ();
			parentOffice.outgoingNetworkLines.Clear ();
			parentOffice.aggregateOfficeList.Clear ();

		}
			
		player.allNodes.Remove (node);

	}

	GameObject Accomplice(GameObject node){

		List <Node> accomplices = new List<Node>();

		foreach (Node witness in node.GetComponent<Node> ().observableNodes) {
			if (witness.nodeState == Node.NodeState.Corrupt) {
				accomplices.Add(witness);
			}			
		}

		print ("Accomplice count is " + accomplices.Count);


		int randomAccomplice = Random.Range (0, accomplices.Count);
		GameObject outedNode = accomplices [randomAccomplice].gameObject;
		print("Outed node should be " + outedNode.name);

		return outedNode;
	}
}
