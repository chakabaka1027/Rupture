using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Office : MonoBehaviour {

	//list of offices that see this office
	public List<Office> connectingOffices;
	public List<Office> connectedOffices;
	public List<Office> aggregateOfficeList;
	public List<Node> officeMembers;
	public Node supervisor;

	public int officeCount;
	public Office parent;
	public int officeProduction;
	public int projectedRevenue;
	public bool hasAddedMembers = false;

	public List<GameObject> outgoingNetworkLines;
	public List<GameObject> outgoingNetworkFlows;

	PlayerController player;


	void Start(){
		
		player = FindObjectOfType<PlayerController>();
		player.allOffices.Add (this.gameObject);
	}


	void Update(){
// use update function to keep track of empty offices so that their connecting lines can be destroyed?

//		if (officeMembers.Count == 0 && connectingOffices.Count > 0) {
//			foreach (Office connectingOffice in connectedOffices) {
//			}
//		}

	}

	public int ActualOfficeRevenue(){
		int production = 0;
		officeProduction = 0;

		if (Time.time < player.rentTimer) {
			foreach (Node bureaucrat in officeMembers) {
				if (bureaucrat.nodeState == Node.NodeState.Corrupt) {
					production -= bureaucrat.production / bureaucrat.corruptionQuotient;
				}
				production += bureaucrat.production;
			}
		}

		//60 is the length in seconds of the month cycle; 15 is the frequency in seconds of when nodes give revenue to player funds
		production = (60 / 15) * production;
		officeProduction += production;

		return officeProduction;
	}	

	public int ProjectedOfficeRevenue(){
		int production = 0;
		officeProduction = 0;

		if (Time.time < player.rentTimer) {

			foreach (Node bureaucrat in officeMembers) {
				production += bureaucrat.production;
			}
		}

		production = (60 / 15) * production;
		officeProduction += production;

		return officeProduction;
	}	

	public int minRange(){
		int min = ProjectedOfficeRevenue() - (ProjectedOfficeRevenue() / 12);
		return min;
	}

	public int maxRange(){
		int max = ProjectedOfficeRevenue () + (ProjectedOfficeRevenue () / 20);
		return max;
	}

	public void MakeSupervisor(){
		supervisor = officeMembers[0];
		supervisor.isSupervisor = true;
		supervisor.gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
	}
}
