using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityStandardAssets.ImageEffects;

public class Vignette : MonoBehaviour {

	VignetteAndChromaticAberration vignette;
	PlayerController player;
	List<GameObject> corruptNodes;
	List<GameObject> allNodes;
	float numberOfCorruptNodes;
	float numberOfAllNodes;
	float percentOfNodesCorrupt;
	float timer;

	// Use this for initialization
	void Start () {

		vignette = GetComponent<VignetteAndChromaticAberration> ();
		player = FindObjectOfType<PlayerController> ();
		corruptNodes = new List<GameObject>();
		timer = 0;
	}

	// Update is called once per frame
	void Update () {

		if (Time.time > timer) {

			if (corruptNodes.Count > 0) {
				corruptNodes.Clear ();
			}

			if (player.allNodes != null) {
				foreach (GameObject node in player.allNodes) {
					if (node.GetComponent<Node> ().nodeState == Node.NodeState.Corrupt) {
						corruptNodes.Add (node);
					}
				}
			}

//			numberOfCorruptNodes = (float)player.GetComponentInParent<CorruptionBehavior> ().corruptNodes.Count;
			numberOfCorruptNodes = (float)corruptNodes.Count;
			numberOfAllNodes = (float)player.allNodes.Count;

			percentOfNodesCorrupt = numberOfCorruptNodes / numberOfAllNodes;

			vignette.intensity = percentOfNodesCorrupt;

			timer = Time.time + 2;
		}

	}
}
