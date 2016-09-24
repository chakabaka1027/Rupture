using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(UIManager))]

public class PlayerController : MonoBehaviour {

	public List<GameObject> allNodes;
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
	public int hireCost = 100;
	public int networkCost = 300;
	public int officeCost = 1000;

	public int minutesUntilPay = 3;
	public int startingFunds = 2000;
	[HideInInspector]
	public int currentFunds;
	UIManager uiManager;

	void Start () {
		rb = GetComponent<Rigidbody>();
		uiManager = GetComponent<UIManager>();
		currentFunds = startingFunds;
		InvokeRepeating("PayTheBill", 60 * minutesUntilPay, 60 * minutesUntilPay);
	}
	
	void Update () {

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
				currentFunds -= hireCost;
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

			if (office.officeCount < maxPerOffice){

				GameObject currentBureaucrat = Instantiate(bureaucrat, office.transform.position + Vector3.forward * 1, Quaternion.identity) as GameObject;
				currentBureaucrat.transform.parent = hit.collider.gameObject.transform;

				//position desks
				if (office.officeCount == 0){
					currentBureaucrat.transform.position = office.transform.position + Vector3.forward * 1 + Vector3.up * 0.125f;
				}

				if (office.officeCount == 1){
					currentBureaucrat.transform.position = office.transform.position + Vector3.left * 1 + Vector3.up * 0.125f;
				}

				if (office.officeCount == 2){
					currentBureaucrat.transform.position = office.transform.position + Vector3.right * 1 + Vector3.up * 0.125f;
				}

				if (office.officeCount == 3){
					currentBureaucrat.transform.position = office.transform.position + Vector3.back * 1 + Vector3.up * 0.125f;
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
					node.GetComponent<Node>().UpdateWitnessableNodes();
				}
			}
		}
	}

	void MakeNetworkConnection(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

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

				if (officeToConnect.GetComponent<Office>().officeMembers.Any()){

					officeToConnect.GetComponent<MeshRenderer>().material.color = connectionColor;
					currentOffice.GetComponent<MeshRenderer>().material.color = connectionColor;

					currentOfficeSelected = false;

					uiManager.DrawLine(currentOffice.transform.position, officeToConnect.transform.position, connectionColor);

					currentOffice.GetComponent<Office>().MakeSupervisor();

					currentOffice.GetComponent<Office>().officeMembers[0].observableOffices.Add(officeToConnect.GetComponent<Office>());

					//add to list of connected offices for payment checks
					currentOffice.GetComponent<Office>().connectedOffices.Add(officeToConnect.GetComponent<Office>());
					officeToConnect.GetComponent<Office>().connectedOffices.Add(currentOffice.GetComponent<Office>());

					//creating connecting offices list
					officeToConnect.GetComponent<Office>().connectingOffices.Add(currentOffice.GetComponent<Office>());



					currentFunds -= networkCost;

					foreach(GameObject node in allNodes){
						node.GetComponent<Node>().UpdateWitnessableNodes();
					}

				}
			}	
		}
	}

	void PayTheBill(){
		if (currentFunds >= 5000){
			currentFunds -= 5000;
		} 
	}

}
