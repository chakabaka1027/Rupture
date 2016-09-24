using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Node : MonoBehaviour{

	[Header("Level System")]
	public int level = 1;
	float experience;
	float timeSincePay;
	float payInterval = 3;

	[Header("Effects")]
	public GameObject payEffect;


	float t;

	public int production;
	public int illicitFunds;
	public int minimumThreshold = 300;
	public int corruptionQuotient = 10;
	public enum NodeState{Neutral, Informant, Corrupt, Witness, Whistleblower};
	public NodeState nodeState;

	public List<Node> observableNodes;
	public List<Office> observableOffices;
	public bool isSupervisor;

	public int selfIndex;

	PlayerController player;


	void Start(){
		t = Time.time;
		player = FindObjectOfType<PlayerController>();
		InvokeRepeating("Pay" , 10, 10);

	}

	void Update(){

	//leveling system
		experience = Time.time - t;
		if (experience > 60){
			level = 2;
		}
		if (experience > 60 + 70){
			level = 3;
		}
		if (experience > 60 + 70 + 80){
			level = 4;
		}
		if (experience >  60 + 70 + 80 + 90){
			level = 5;
		}

	//money earning
		if (level == 1){
			production = 50;
		}
		if (level == 2){
			production = 75;
		}
		if (level == 3){
			production = 100;
		}
		if (level == 4){
			production = 125;
		}
		if (level == 5){
			production = 150;
		}

	}

	public void UpdateWitnessableNodes(){

		if(!isSupervisor){
			List<Node> withDupes = gameObject.GetComponentInParent<Office>().officeMembers;
			List<Node> noDupes = withDupes.Distinct().ToList();
			noDupes.Remove(noDupes[selfIndex]);


			//pseudocode: if there is an office connected to you, grab the supervisor from that office
			if (gameObject.GetComponentInParent<Office>().connectingOffices != null){
				foreach (Office office in gameObject.GetComponentInParent<Office>().connectingOffices){
					if (office.officeMembers[0].isSupervisor == true){
						noDupes.Add(office.officeMembers[0]);
					}
				}
			}
			observableNodes = noDupes;
		
		} else if (isSupervisor){
			
			List<Node> withDupes = gameObject.GetComponentInParent<Office>().officeMembers;

			List<Node> noDupes = withDupes.Distinct().ToList();
			noDupes.Remove(noDupes[selfIndex]);

			foreach(Office office in observableOffices){
				noDupes.AddRange(office.officeMembers);
			}

			observableNodes = noDupes;
		}
	}

	public void Pay(){
		if (Time.time > timeSincePay && gameObject.GetComponentInParent<Office>().connectedOffices.Any()){
			if (nodeState == NodeState.Corrupt) {
				//corrupt nodes take a cut of their production before paying the player
				production = production - (production / corruptionQuotient);
				illicitFunds += production / corruptionQuotient;

			} else {
				player.currentFunds += production;
			}

			timeSincePay = Time.time + payInterval;
			PayEffect ();

		}
	}

	public void PayEffect(){
			GameObject effect = Instantiate(payEffect, gameObject.transform.position, Quaternion.identity) as GameObject;
			Destroy(effect, 2);
	}
}
