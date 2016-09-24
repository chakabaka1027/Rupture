using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Office : MonoBehaviour {

	//list of offices that see this office
	public List<Office> connectingOffices;
	public List<Office> connectedOffices;
	public List<Node> officeMembers;
	public Node supervisor;

	public int officeCount;
	public Office parent;
	public float officeProduction;
	public bool hasAddedMembers = false;

	public float GetOfficeProduction(){
		foreach (Node bureaucrat in officeMembers){
			officeProduction += bureaucrat.production;
		}

		return officeProduction;
	}	

	public void MakeSupervisor(){
		supervisor = officeMembers[0];
		supervisor.isSupervisor = true;
		supervisor.gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
	}

}
