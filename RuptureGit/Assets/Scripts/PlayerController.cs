using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(UIManager))]

public class PlayerController : MonoBehaviour {

	public List<GameObject> allNodes;
	public List<GameObject> allOffices;
	public enum State{Start, Hiring, Network, Office, Cursory, Thorough};
	public State playerState;

	[Header("Bureaucrat Placement")]
	public LayerMask clickable;
	public GameObject bureaucrat;
	public int maxBureaucratPlacement = 10;
	int maxPerOffice = 4;
	int bureaucratCount = 0;
	int bureaucratsRemaining = 10;

	[Header("Connection Placement")]
	public Color connectionColor;
	public Color highlightColor;
	GameObject currentOffice;
	GameObject officeToConnect;
	bool currentOfficeSelected = false;

	[Header("Office Placement")]
	public GameObject office;
	public GameObject officeContainer;


	[Header("Camera Movement")]
	public float moveSpeed = 5;
	Vector3 targetMoveAmount;
	Vector3 smoothDampMoveRef;
	Vector3 moveAmount;
	Rigidbody rb;

	[Header("Money")]
	public int hireCost = 2000;
	public int networkCost = 5000;
	public int officeCost = 15000;
	public int rent;

//	public int minutesUntilPay = 3;
	public float rentTimer;
	public int startingFunds = 100000;

	[HideInInspector]
	public int currentFunds;
	UIManager uiManager;


	void Start () {
		rb = GetComponent<Rigidbody>();
		uiManager = GetComponent<UIManager>();
		currentFunds = startingFunds;
		rentTimer = 60;
//		InvokeRepeating("PayTheRent", 90, 90);
	}
	
	void Update () {

		if (Time.time > rentTimer) {
			PayTheRent ();
			rentTimer = Time.time + 60;
		}

		//Create Office
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane groundPlane = new Plane(Vector3.up, Vector3.zero );
		float rayDistance;

		if (groundPlane.Raycast(ray, out rayDistance)){
			Vector3 point = ray.GetPoint(rayDistance);
			Debug.DrawLine(ray.origin, point, Color.red);

			if (Input.GetMouseButtonDown(0) && playerState == State.Office && currentFunds > officeCost && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1) == false){
				GameObject officeCreation = Instantiate(office, point, Quaternion.identity) as GameObject;
				officeCreation.transform.parent = officeContainer.transform;
				currentFunds -= officeCost;

			}
		}

		//Camera Movement
		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0 , Input.GetAxisRaw("Vertical")).normalized;
		targetMoveAmount = moveDir * moveSpeed;
		moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothDampMoveRef, 0.05f);

		//Clicking
		if (Input.GetMouseButtonDown(0) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1) == false){
			if (playerState == State.Hiring && currentFunds > hireCost){
				Hire();
			}  else if (playerState == State.Network && currentFunds > networkCost){
				MakeNetworkConnection();
			}
		}
	}

	void FixedUpdate(){
		rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime * Time.timeScale);
	}

	void Hire(){

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable)){
			Office office = hit.collider.gameObject.GetComponent<Office>();

			int nodeLayer = 1 << 9; //the Bureacrat layer
			Collider[] hitColliders1;
			Collider[] hitColliders2;
			Collider[] hitColliders3;
			Collider[] hitColliders4;
//
//			Vector3 position1 = new Vector3 (Vector3.forward * 1 + Vector3.up * 0.125f);
//			Vector3 position2 = new Vector3 (Vector3.left * 1 + Vector3.up * 0.125f);
//			Vector3 position3 = new Vector3 (Vector3.right * 1 + Vector3.up * 0.125f);
//			Vector3 position4 = new Vector3 (Vector3.back * 1 + Vector3.up * 0.125f);

			//creates four small spheres that check to see if the places Bureaucrats might be placed in are occupied
			hitColliders1 = Physics.OverlapSphere ((office.transform.position + Vector3.forward * 1 + Vector3.up * 0.125f), 0.1f, nodeLayer);
			hitColliders2 = Physics.OverlapSphere ((office.transform.position + Vector3.left * 1 + Vector3.up * 0.125f), 0.1f, nodeLayer);
			hitColliders3 = Physics.OverlapSphere ((office.transform.position + Vector3.right * 1 + Vector3.up * 0.125f), 0.1f, nodeLayer);
			hitColliders4 = Physics.OverlapSphere ((office.transform.position + Vector3.back * 1 + Vector3.up * 0.125f), 0.1f, nodeLayer);


			if (office.officeCount < maxPerOffice){

				GameObject currentBureaucrat = Instantiate(bureaucrat, office.transform.position + Vector3.forward * 1, Quaternion.identity) as GameObject;
				currentBureaucrat.transform.parent = hit.collider.gameObject.transform;

				//this if/else chain will check each position in order before deciding where to place a Bureaucrat
				//this solves the bug of having overlapping Bureaucrats if trying to place them following a corrupt one's removal


//				if (!Physics.OverlapSphere ((office.transform.position + position1), 0.1f, nodeLayer).Any) {
//					currentBureaucrat.transform.position = office.transform.position + position1;
//				} else if (!Physics.OverlapSphere ((office.transform.position + position2), 0.1f, nodeLayer).Any) {
//					currentBureaucrat.transform.position = office.transform.position + position2;
//				} else if (!Physics.OverlapSphere ((office.transform.position + position3), 0.1f, nodeLayer).Any) {
//					currentBureaucrat.transform.position = office.transform.position + position3;
//				} else if (!Physics.OverlapSphere ((office.transform.position + position4), 0.1f, nodeLayer).Any) {
//					currentBureaucrat.transform.position = office.transform.position + position4;
//				}


				if (hitColliders1.Length == 0) {
					currentBureaucrat.transform.position = office.transform.position + Vector3.forward * 1 + Vector3.up * 0.125f;
//					currentBureaucrat.GetComponent<Node>().selfIndex = 0;

				} else if (hitColliders2.Length == 0) {
					currentBureaucrat.transform.position = office.transform.position + Vector3.left * 1 + Vector3.up * 0.125f;
//					currentBureaucrat.GetComponent<Node>().selfIndex = 1;

				} else if (hitColliders3.Length == 0) {
					currentBureaucrat.transform.position = office.transform.position + Vector3.right * 1 + Vector3.up * 0.125f;
//					currentBureaucrat.GetComponent<Node>().selfIndex = 2;

				} else if (hitColliders4.Length == 0) {
					currentBureaucrat.transform.position = office.transform.position + Vector3.back * 1 + Vector3.up * 0.125f;
//					currentBureaucrat.GetComponent<Node>().selfIndex = 3;

				} 

				//generate list of all bureaucrats in office
				office.officeMembers.Add(currentBureaucrat.GetComponent<Node>());

				currentBureaucrat.GetComponent<Node>().selfIndex = office.officeMembers.Count - 1;

				office.officeCount ++;
				bureaucratsRemaining --;
				bureaucratCount ++;
				uiManager.SubtractBureaucrats(bureaucratsRemaining);
				allNodes.Add(currentBureaucrat);

				foreach(GameObject node in allNodes){
					if (node != null) {
						node.GetComponent<Node> ().UpdateWitnessableNodes ();
					}
				}

				currentFunds -= hireCost;
			}
		}
	}

	void MakeNetworkConnection(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		bool cycle = false;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable)){

			if(currentOfficeSelected == false){
				currentOffice = hit.collider.gameObject;
				if (currentOffice.GetComponent<Office>().officeMembers.Any()){
					currentOffice.GetComponent<MeshRenderer>().material.color = highlightColor;
					currentOfficeSelected = true;
				}
			}

			if(currentOfficeSelected == true && hit.collider.gameObject != currentOffice){
				officeToConnect = hit.collider.gameObject;


//				for (int i = 0; i < officeToConnect.GetComponent<Office>().aggregateOfficeList.Count; i++) {
//					
//					if (currentOffice.GetComponent<Office>().aggregateOfficeList[i].gameObject == officeToConnect){
//						cycle = true;
//						Debug.Log ("Cycle is true");
//					}
//				}

				foreach (Office previousOffice in currentOffice.GetComponent<Office>().aggregateOfficeList) {
					if (previousOffice == officeToConnect.GetComponent<Office> ()) {
						cycle = true;
						Debug.Log ("Cycle is true");

						if (cycle) {
							currentOfficeSelected = false;
							currentOffice.GetComponent<MeshRenderer>().material.color = connectionColor;
						}
					}
				}

				if (officeToConnect.GetComponent<Office>().officeMembers.Any() && cycle == false){

					officeToConnect.GetComponent<MeshRenderer>().material.color = connectionColor;
					currentOffice.GetComponent<MeshRenderer>().material.color = connectionColor;
					connectionColor = Color.white;

					currentOfficeSelected = false;

					uiManager.DrawLine(currentOffice.transform.position, officeToConnect.transform.position, connectionColor);

					currentOffice.GetComponent<Office>().MakeSupervisor();

					currentOffice.GetComponent<Office>().officeMembers[0].observableOffices.Add(officeToConnect.GetComponent<Office>());

					//add to list of connected offices for payment checks
					currentOffice.GetComponent<Office>().connectedOffices.Add(officeToConnect.GetComponent<Office>());

					//creating connecting offices list
					officeToConnect.GetComponent<Office>().connectingOffices.Add(currentOffice.GetComponent<Office>());

					//add previous offices to list of aggregate offices in chain
					officeToConnect.GetComponent<Office> ().aggregateOfficeList.Add (currentOffice.GetComponent<Office>());
					officeToConnect.GetComponent<Office> ().aggregateOfficeList.AddRange(currentOffice.GetComponent<Office>().aggregateOfficeList);

					currentFunds -= networkCost;

					foreach(GameObject node in allNodes){
						node.GetComponent<Node>().UpdateWitnessableNodes();
					}

				}
			}	
		}
	}

	void PayTheRent(){
		rent = 2500;
		rent *= allOffices.Count;
			
		currentFunds -= (rent);
		Debug.Log ("Rent cost = " + rent);
	}

	public int RentDue(){
		
		if (Time.time < rentTimer) {
			rent = 2500;
			rent *= allOffices.Count;
		}
		return rent;
	}
}
