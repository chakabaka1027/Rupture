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
	public int officeProduction;
	public int projectedRevenue;
	public bool hasAddedMembers = false;


	public int GetOfficeProduction(){
		foreach (Node bureaucrat in officeMembers){
			officeProduction += bureaucrat.production;
		}

		return officeProduction;
	}	

	public int ProjectedOfficeRevenue(){
		int nodeProjectedRevenue = 0;
		foreach (Node bureaucrat in officeMembers) {
			if (bureaucrat.level == 1) {
				nodeProjectedRevenue = 150;
			} else if (bureaucrat.level == 2) {
				nodeProjectedRevenue = 175;
			} else if (bureaucrat.level == 3) {
				nodeProjectedRevenue = 200;
			} else if (bureaucrat.level == 4) {
				nodeProjectedRevenue = 225;
			} else if (bureaucrat.level == 5) {
				nodeProjectedRevenue = 250;
			}

			projectedRevenue += nodeProjectedRevenue;
		}

		return projectedRevenue;
	}
//
//	int ProjectedMin(int ProjectedOfficeRevenue()){
//		int projectedMin = 0;
//
//		projectedMin = (projectedRevenue - ProjectedOfficeRevenue() / 20);
//
//		return projectedMin;
//		
//	}
//
//	int ProjectedMax(int projectedRevenue){
//		int projectedMax = 0;
//
//		projectedMax = (projectedRevenue + ProjectedOfficeRevenue() / 50); 
//
//		return projectedMax;
//	}
//

	public void MakeSupervisor(){
		supervisor = officeMembers[0];
		supervisor.isSupervisor = true;
		supervisor.gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
	}
		
}
