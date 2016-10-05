using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public Text funds;
	public Text bureaucratText;
	public Color titleBlackFade;
	public Shader lineShader;
	public ParticleSystem flow;

	[Header("Build UI")]
	public GameObject buildButton;
	public GameObject buildEndPos;
	public RectTransform buildUI;
	bool buildUIActive = false;

	[Header("Investigate UI")]
	public GameObject investigateButton;
	public GameObject investigateEndPos;
	public RectTransform investigateUI;
	bool investigateUIActive = false;

	[Header("Game Mode Text")]
	public GameObject gameModeObject;
	public Text gameModeText;

	[Header("Pay Time Text")]
	public Text payTimeText;


	PlayerController player;

	void Start(){
		player = FindObjectOfType<PlayerController>(); 
	}

	void Update(){
		funds.text = "Funds: $" + player.currentFunds;
	}

	public void SubtractBureaucrats(int count){
		if (count > 0){
			bureaucratText.text = count + " BUREAUCRATS";
			bureaucratText.color = titleBlackFade;
		} else {
			bureaucratText.text = "";
		}
	}

	public void DrawLine(Vector3 start, Vector3 end, Color color){
		GameObject myLine = new GameObject();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = new Material(lineShader);
		lr.SetColors(color, color);
		lr.SetWidth(0.1f, 0.1f);
		lr.SetPosition(0, start + Vector3.down * 0.1f);
		lr.SetPosition(1, end + Vector3.down * 0.1f);

		GameObject flowAnimation = Instantiate(flow.gameObject, start + Vector3.down * 0.2f, Quaternion.identity) as GameObject;
		flowAnimation.transform.LookAt(end + Vector3.down * 0.2f);
		float distance = GetDistance(start, end);
		flowAnimation.GetComponent<ParticleSystem>().startLifetime = distance/4.5f;
	}

	float GetDistance (Vector3 start, Vector3 end){
		float distanceX = Mathf.Abs(end.x - start.x);
		float distanceY = Mathf.Abs(end.z - start.z);
		return Mathf.Sqrt(Mathf.Pow(distanceX, 2) + Mathf.Pow(distanceY, 2));
	}

	IEnumerator AnimateBuildUI(){

		float percent = 0;
		float animationTime = 0.2f;
		float animationSpeed = 1 / animationTime;

		while(percent >= 0){
			percent += Time.deltaTime * animationSpeed;
			buildUI.transform.position = Vector3.up * Mathf.Lerp(-30, buildEndPos.transform.position.y, percent) + new Vector3(buildButton.transform.position.x, 0, 0);
			yield return null;
		}
	}

	public void GetBuildUI(){
		if(buildUIActive == false){
			CloseInvestigateUI();
			buildUI.gameObject.SetActive(true);

			StopCoroutine("AnimateBuildUI");
			StartCoroutine("AnimateBuildUI");
			buildUIActive = true;
		} else if (buildUIActive == true){
			CloseBuildUI();
		}

	}

	public void CloseBuildUI(){
		buildUI.gameObject.SetActive(false);
		buildUIActive = false;
	}

	public void GetInvestigateUI(){
		if(investigateUIActive == false){
			CloseBuildUI();
			investigateUI.gameObject.SetActive(true);

			StopCoroutine("AnimateInvestigateUI");
			StartCoroutine("AnimateInvestigateUI");
			investigateUIActive = true;
		} else if (investigateUIActive == true){
			CloseInvestigateUI();
		}
	}

	public void CloseInvestigateUI(){
		investigateUI.gameObject.SetActive(false);
		investigateUIActive = false;
	}

	IEnumerator AnimateInvestigateUI(){
		float percent = 0;
		float animationTime = 0.2f;
		float animationSpeed = 1 / animationTime;

		while(percent >= 0){
			percent += Time.deltaTime * animationSpeed;
			investigateUI.transform.position = Vector3.up * Mathf.Lerp(-30, investigateEndPos.transform.position.y, percent) + new Vector3(investigateButton.transform.position.x, 0, 0);
			yield return null;
		}
	}

	public void Hire(){
		CloseBuildUI();
		player.playerState = PlayerController.State.Hiring;
		SetGameModeText("Staffing");

	}

	public void Network(){
		CloseBuildUI();
		player.playerState = PlayerController.State.Network;
		SetGameModeText("Establishing Networks");

	}

	public void Office(){
		CloseBuildUI();
		player.playerState = PlayerController.State.Office;
		SetGameModeText("Building Offices");

	}

	//Austin added this method on 10/4/2016
	public void Cursory(){
		CloseInvestigateUI ();
		player.playerState = PlayerController.State.Cursory;
		SetGameModeText ("Cursory Investigation");
	}

	//Austin added this method on 10/4/2016
	public void Thorough(){
		CloseInvestigateUI ();
		player.playerState = PlayerController.State.Thorough;
		SetGameModeText ("Thorough Investigation");
	}

	public void SetGameModeText(string mode){
		gameModeObject.SetActive(true);
		gameModeText.text = mode;
	}

	public void CloseModeTab(){
		gameModeObject.SetActive(false);
		player.playerState = PlayerController.State.Start;
		gameModeObject.SetActive(false);

	}

}
