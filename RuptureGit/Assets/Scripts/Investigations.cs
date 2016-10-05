﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Investigations : MonoBehaviour {

	//must write code in the UIManager script to change the PlayerState to Cursory or Thorough


	PlayerController player;
	Office parentOffice;
	GameObject node;

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

			node = hit.collider.gameObject;

			if (player.currentFunds >= cursoryCost){
				player.currentFunds -= cursoryCost;

				if (node.GetComponent<Node>().nodeState == Node.NodeState.Corrupt) {
					RemoveThisNodeFromLists ();
					GameObject.Destroy (node);
				}
			}
		}
	}

		

	void ThoroughInvestigation(){

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, bureacratLayer)){

			node = hit.collider.gameObject;

			if (player.currentFunds >= thoroughCost){
				player.currentFunds -= thoroughCost;

				if (node.GetComponent<Node>().nodeState == Node.NodeState.Corrupt) {
					RemoveThisNodeFromLists ();
					GameObject.Destroy (node);
				}
			}
		}

	}


	void RemoveThisNodeFromLists(){
		player.allNodes.Remove (node);

		foreach (Node observableNode in node.GetComponent<Node> ().observableNodes) {
			observableNode.observableNodes.Remove (node.GetComponent<Node> ());
		}

		if (!node.GetComponent<Node> ().isSupervisor) {
			node.GetComponentInParent<Office> ().officeMembers.Remove (node.GetComponent<Node> ());
		} else if (node.GetComponent<Node> ().isSupervisor) {
			foreach (Office observableOffice in node.GetComponent<Node> ().observableOffices) {
				observableOffice.officeMembers.Remove (node.GetComponent<Node> ());
			}
		}
	}

	void RemoveAccompliceFromLists(){
		
	}

}