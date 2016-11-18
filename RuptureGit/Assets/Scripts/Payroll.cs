using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Payroll : MonoBehaviour {

	PlayerController player;
	public float payTimer = 60f;
	public int payroll;


	// Use this for initialization
	void Start () {
		player = FindObjectOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {

		if (Time.time > payTimer) {
			payroll = 0;

			foreach (GameObject node in player.allNodes) {
				if (node != null) {
					payroll += node.GetComponent<Node> ().salary;
				}
			}

			player.currentFunds -= payroll;
			Debug.Log ("Payroll cost = " + payroll);
			payTimer = Time.time + 60f;
		}
	}

	public int PayrollDue(){
		payroll = 0;
//		if (Time.time < payTimer) {
			foreach (GameObject node in player.allNodes) {
				if (node != null) {
					payroll += node.GetComponent<Node> ().salary;
				}
			}
//		}

		return payroll;
	}
}
