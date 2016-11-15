using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Payroll : MonoBehaviour {

	PlayerController player;
	public float payTimer = 45f;
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
				payroll += node.GetComponent<Node> ().salary;
			}

			player.currentFunds -= payroll;
			Debug.Log ("You paid " + payroll + "in payroll");
			payTimer = Time.time + 45f;
		}
	
	}
}
