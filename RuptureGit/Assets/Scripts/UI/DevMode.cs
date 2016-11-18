using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DevMode : MonoBehaviour {

	PlayerController player;
	public bool devModeActivated = false;
	public Color defaultColor;

	// Use this for initialization
	void Start () {
		player = FindObjectOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Space)) {
			devModeActivated = !devModeActivated;
		}

		if (devModeActivated) {
			foreach (GameObject bureaucrat in player.allNodes) {
				Node node = bureaucrat.GetComponent<Node> ();
				Color bureaucratColor = bureaucrat.GetComponent<MeshRenderer> ().material.color;

				if (node.nodeState == Node.NodeState.Witness) {
					bureaucratColor = Color.green;
					node.gameObject.GetComponent<MeshRenderer> ().material.color = bureaucratColor;
				}

				if (node.nodeState == Node.NodeState.Corrupt) {
					bureaucratColor = Color.red;
					node.gameObject.GetComponent<MeshRenderer> ().material.color = bureaucratColor;
				}
				if (node.nodeState == Node.NodeState.Whistleblower) {
					bureaucratColor = Color.yellow;
					node.gameObject.GetComponent<MeshRenderer> ().material.color = bureaucratColor;
				}
			}
		} else {
			foreach (GameObject bureaucrat in player.allNodes) {
				bureaucrat.GetComponent<MeshRenderer> ().material.color = defaultColor;
				if (bureaucrat.GetComponent<Node> ().isSupervisor) {
					bureaucrat.GetComponent<MeshRenderer> ().material.color = Color.blue;
				}
			}
		}
	
	}
}
