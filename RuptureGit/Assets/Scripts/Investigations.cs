using UnityEngine;
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
					RemoveNodeFromLists (Accomplice(selectedNode));
					RemoveNodeFromLists (selectedNode);
					GameObject.Destroy (selectedNode);
				}
			}
		}

	}


	void RemoveNodeFromLists(GameObject node){

		List <Office> supervisorOfficeList = null;
		parentOffice = node.GetComponentInParent<Office> ();

		player.allNodes.Remove (node);

		foreach (Node observableNode in node.GetComponent<Node> ().observableNodes) {
			observableNode.observableNodes.Remove (node.GetComponent<Node> ());
		}

		parentOffice.officeMembers.Remove (node.GetComponent<Node> ());
		parentOffice.officeCount --;

		if (node.GetComponent<Node> ().isSupervisor) {
			//copies the supervisor's list of observable offices to be transferred to the new supervisor
			supervisorOfficeList = node.GetComponent<Node> ().observableOffices;

			foreach (Office observableOffice in node.GetComponent<Node> ().observableOffices) {
				observableOffice.officeMembers.Remove (node.GetComponent<Node> ());
			}

			//makes the next node down in the list, which has been shifted to the zeroeth position, the new supervisor
			parentOffice.MakeSupervisor ();
			parentOffice.supervisor.observableOffices = supervisorOfficeList;
		}


//		foreach (Node officemember in parentOffice.officeMembers) {
//			officemember.selfIndex --;
//		}

	}

	GameObject Accomplice(GameObject node){

		List <Node> accomplices = null;

//		//this loop SHOULD determine who is an accomplice to the corrupt node being investigated. Presently not functional
//		//for reasons I can't understand :( Error message says that accomplices "not set to an instance of an object", even tho I add witness?
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

//first experimental method placed in the Hire() method of the PlayerController script 
//to try and fully account for when a supervisor is fired and replaced
//

//				if (office.officeCount > 0 && !office.officeMembers [0].isSupervisor) {
////					office.officeMembers.Insert (0, currentBureaucrat.GetComponent<Node>());
//					currentBureaucrat.transform.position = office.transform.position + Vector3.forward * 1 + Vector3.up * 0.125f;
////					office.officeMembers [0].isSupervisor = true;
//				}