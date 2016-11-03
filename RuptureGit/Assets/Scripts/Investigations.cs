﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Investigations : MonoBehaviour {

	//must write code in the UIManager script to change the PlayerState to Cursory or Thorough


	PlayerController player;
	Office parentOffice;
	GameObject selectedNode;

	public LayerMask bureacratLayer;
	int cursoryCost = 200;
	int thoroughCost = 350;


	// Use this for initialization
	void Start () {
		player = FindObjectOfType<PlayerController> ();
	
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
				}
			}
		}
	}


	void RemoveNodeFromLists(GameObject node){

//		List <Office> supervisorOfficeList = new List<Office>();
		parentOffice = node.GetComponentInParent<Office> ();


		foreach (GameObject observableNode in player.allNodes) {
			observableNode.GetComponent<Node> ().observableNodes.Remove (node.GetComponent<Node>());
		}


//		foreach (Node observableNode in node.GetComponent<Node> ().observableNodes) {
//			if (node != null) {
//				
//				observableNode.observableNodes.Remove (node.GetComponent<Node> ());
//			}
//		}

		if (node.GetComponent<Node> ().isSupervisor) {
			//copies the supervisor's list of observable offices to be transferred to the new supervisor
//			supervisorOfficeList = node.GetComponent<Node> ().observableOffices;


			foreach (Office connectedOffice in parentOffice.connectedOffices) {
				connectedOffice.connectingOffices.Remove (parentOffice);
			}

			parentOffice.connectedOffices.Clear();

			//destroys the network ties/flow emerging from each office
			foreach (GameObject outgoingFlow in parentOffice.outgoingNetworkFlows) {
				Destroy (outgoingFlow);
			}

			foreach (GameObject outgoingLine in parentOffice.outgoingNetworkLines) {
				Destroy (outgoingLine);
			}

			parentOffice.outgoingNetworkFlows.Clear ();
			parentOffice.outgoingNetworkLines.Clear ();

		}

		parentOffice.officeMembers.Remove (node.GetComponent<Node> ());
		parentOffice.officeCount --;


		player.allNodes.Remove (node);

//		foreach (Node officemember in parentOffice.officeMembers) {
//			officemember.selfIndex --;
//		}

	}

	GameObject Accomplice(GameObject node){

		Debug.Log ("I'm running");

		List <Node> accomplices = new List<Node>();

		foreach (Node witness in node.GetComponent<Node> ().observableNodes) {
			if (witness.nodeState == Node.NodeState.Corrupt) {
				accomplices.Add(witness);
			}			
		}

		int randomAccomplice = Random.Range (0, accomplices.Count);
		GameObject outedNode = accomplices [randomAccomplice].gameObject;

		return outedNode;
	}

}
