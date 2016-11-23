using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayExpensesDue : MonoBehaviour {

	public Text payrollText;
	public Text rentText;
	public Text totalText;
	public Text expensesTimer;

	PlayerController player;

	float timer = 60;

	// Use this for initialization
	void Start () {
		player = FindObjectOfType<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (timer <= 0) {
			timer = 60;
		}

		timer -= Time.deltaTime;

		string secondsTimer = ((int)timer).ToString();

		expensesTimer.text = secondsTimer;

		payrollText.text = "Payroll: " + player.GetComponentInParent<Payroll> ().PayrollDue ();
		
		rentText.text = "Rent: " + player.RentDue();

		totalText.text = "Total: " + (player.RentDue () + player.GetComponentInParent<Payroll> ().PayrollDue ());

	}
}
