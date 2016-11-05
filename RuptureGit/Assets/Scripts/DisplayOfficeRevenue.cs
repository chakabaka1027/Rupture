using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayOfficeRevenue : MonoBehaviour {

	public LayerMask officeLayer;
	public GameObject productionDisplay;
	Transform target;

	public Text projectedRevText;
	public Text actualRevText;

	bool hovering = false;
	public float hoverTimer;
	public float projectionTimer;


	// Use this for initialization
	void Start () {
		hoverTimer = 0f;
		projectionTimer = 0f;
	}
	
	// Update is called once per frame
	void Update () {

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (!hovering) {
			hoverTimer = Time.time + 1.5f;
			productionDisplay.SetActive (false);
		}

		if (Physics.Raycast (ray, out hit, Mathf.Infinity, officeLayer)) {
			hovering = true;
		} else {
			hovering = false;
		}
			
		if (hovering && Time.time > hoverTimer){
			Office office = hit.collider.gameObject.GetComponent<Office> ();
			target = hit.collider.gameObject.transform;
			Vector3 screenPos = Camera.main.WorldToScreenPoint (target.position);

			Vector3 temp = screenPos;

			if (screenPos.x < 800) {
				temp.x += 150f;
			} else if (screenPos.x > 800) {
				temp.x -= 150;
			}

			if (screenPos.y < 120) {
				temp.y += 120f;
			} else if (screenPos.y > 300) {
				temp.y -= 120f;
			}

			productionDisplay.transform.position = temp;

			productionDisplay.SetActive (true);

//			productionDisplay.transform.position.x = screenPos.x;
//			productionDisplay.transform.position.y = screenPos.y - 75f;
			if (Time.time > projectionTimer) {
				Debug.Log ("Office is located " + screenPos.x + " pixels from the left and " + screenPos.y + " from the bottom");
				projectedRevText.text = "Projected Revenue: " + office.ProjectedOfficeRevenue ();
				actualRevText.text = "Actual Revenue: " + office.GetOfficeProduction ();
				projectionTimer += 5;
			}
		}
			
	}
}
