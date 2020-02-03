using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	public GameObject polePrefab;
	public GameObject gearPrefab;

	[Header("Bar Params")]
	public GameObject barObject;
	public Vector3 barOriginPos;
	public float barSpeed = 50f;
	public bool barActive = true;
	
	[Header("Ball Params")]
	public GameObject ballObject;
	public Vector3 ballOriginPos;
	public Vector3 ballOriginScale;
	public float jumpAmount = 2f;

	[Header("Goal Params")]
	public GameObject goalObject;
	public Text scoreText;
	public Image backgroundPanel;
	public Color normalColor = Color.white;
	public Color winColor = Color.green;
	public Color loseColor = Color.red;

	[Header("Map Params")]
	public float xRadius = 10f;
	public float yRadius = 10f;
	public int gearNum = 3;
	public GameObject gearsHolder;

	[Header("Lists")]
	public List<Gear> gears;
	public List<Pole> poles;
	
    // Start is called before the first frame update
    void Start()
    {
		GenerateMap();
		CreateGoal();
    }

    // Update is called once per frame
    void Update()
    {
		CheckForMouseInteractions();

		if(barActive)
			ControlBar();
		else
			ControlBall();

		if(Input.GetKeyDown(KeyCode.R))
		{
			Reset(false);
		}
    }
	
	void CreatePole()
	{
		var pos = Input.mousePosition;
		pos.z = 10;
		var pole = Instantiate(polePrefab);
		pole.transform.position = Camera.main.ScreenToWorldPoint(pos);
		poles.Add(pole.GetComponent<Pole>());
	}
	
	void ControlBar()
	{
		if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			barObject.transform.position += Vector3.left * barSpeed * Time.deltaTime;
		} else if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			barObject.transform.position += Vector3.right * barSpeed * Time.deltaTime;
		}
		else if(Input.GetKeyDown(KeyCode.Space))
		{
			ballObject.transform.parent = null;
			barObject.SetActive(false);
			barActive = false;
		}
	}
	
	void ControlBall()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			ballObject.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpAmount);
		}
	}
	
	void CheckForMouseInteractions()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
			{
				if(hit.collider.gameObject.GetComponent<Pole>())
				{
					var gearsHit = hit.collider.gameObject.GetComponent<Pole>().gearsHit;
					foreach(var gear in gearsHit)
					{
						gear.RemoveHitPole(hit.collider.gameObject.GetComponent<Pole>());
					}
					Destroy(hit.collider.gameObject);
					poles.Remove(hit.collider.gameObject.GetComponent<Pole>());
				} else if(hit.collider.gameObject.GetComponent<Gear>())
				{
					CreatePole();
					Debug.LogWarning("Hit gear: " + hit.collider.gameObject.name);
				}
			} else
			{
				CreatePole();
			}
		}
	}

	private void Reset(bool changeGoal)
	{
		//ballObject.transform.parent = null;
		ballObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
		ballObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		ballObject.transform.parent = barObject.transform;
		ballObject.transform.localScale = ballOriginScale;
		ballObject.transform.localPosition = ballOriginPos;
		ballObject.transform.rotation = Quaternion.identity;

		barObject.SetActive(true);
		barObject.transform.position = barOriginPos;
		barActive = true;

		if(changeGoal)
		{
			foreach(var gear in gears)
			{
				Destroy(gear.gameObject);
			}
			gears.Clear();
			
			GenerateMap();
			
			Destroy(goalObject);
			CreateGoal();
		}
		else
		{
			foreach(var gear in gears)
			{
				gear.Reset();
				gear.hitList.Clear();
			}
		}

		foreach(var pole in poles)
		{
			Destroy(pole.gameObject);
		}
		poles.Clear();
	}
	
	void CreateGoal()
	{
		Gear goalGear = gears[Random.Range(0, gears.Count)];
		var components = goalGear.GetComponentsInChildren<GearComponent>();
		GearComponent goalComp = components[Random.Range(0, components.Length)];
		goalObject = goalComp.goals[Random.Range(0, goalComp.goals.Count)];
		goalObject.SetActive(true);
	}
	
	public void Win()
	{
		Debug.Log("Win");

		StartCoroutine(ChangeColor(winColor));

		int score = int.Parse(scoreText.text);
		score++;
		scoreText.text = score.ToString();
		
		Reset(true);
	}
	
	IEnumerator ChangeColor(Color color)
	{
		backgroundPanel.color = color;

		yield return new WaitForSeconds(0.15f);
		
		backgroundPanel.color = normalColor;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			StartCoroutine(ChangeColor(loseColor));
			Reset(false);
		}
	}
	
	void GenerateMap()
	{
		var amt = Random.Range(3, gearNum);
		for(int i = 0; i < amt; i++)
		{
			Vector3 pos = Vector3.zero;
			var x = Random.Range(-xRadius, xRadius);
			var y = Random.Range(-yRadius, yRadius);
			pos.x = x;
			pos.y = y;

			var gear = Instantiate(gearPrefab, pos, Quaternion.identity, gearsHolder.transform);
			gears.Add(gear.GetComponent<Gear>());

			var goals = gear.GetComponentsInChildren<Goal>(true);
			foreach(var goal in goals)
			{
				goal.playerRef = this;
			}

			var forward = Random.Range(0, 2);
			gear.GetComponent<Gear>().isForward = forward == 0;
		}
	}
}
