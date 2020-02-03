using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
	public bool isRunning = true;
	public bool isForward = true;
	public float speed = 5f;

	private bool canControl;
	private bool playerIsOn;

	[Header("Colors")]
	public Material regularGearMat;
	public Material highlightedGearMat;
	public Material stoppedGearMat;
	
	public List<Pole> hitList;
	public List<GameObject> pieces;
	
	public enum Status
	{
		Running,
		Highlighted,
		Stopped
	}
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if(canControl)
		{
			if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
			{
				isForward = true;
			} else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
			{
				isForward = false;
			}
		}
		
		if(hitList.Count == 0 && isRunning)
		{
			Rotate();
		}
    }
	
	public void RemoveHitPole(Pole pole)
	{
		hitList.Remove(pole);


		if(hitList.Count != 0)
			return;

		if(playerIsOn)
		{
			Debug.LogWarning("Player is on");
			SetColor(Status.Highlighted);
			canControl = true;
		} else
		{
			Debug.LogWarning("Player not on");
			SetColor(Status.Running);
		}
	}

	void Rotate()
	{
		var dir = isForward ? Vector3.forward : Vector3.back;
		transform.RotateAround(dir, Time.deltaTime * speed);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.GetComponent<Pole>())
		{
			hitList.Add(other.gameObject.GetComponent<Pole>());
		}
		else if(other.gameObject.CompareTag("Player"))
		{
			if(hitList.Count == 0)
			{
				SetColor(Status.Highlighted);
				canControl = true;
			}
			playerIsOn = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			canControl = false;
			playerIsOn = false;
			
			if(hitList.Count == 0)
				SetColor(Status.Running);
		}
	}

	public void SetColor(Status status)
	{
		Material mat = null;
		switch(status)
		{
		case Status.Stopped:
			mat = stoppedGearMat;
			break;
		case Status.Highlighted:
			mat = highlightedGearMat;
			break;
		case Status.Running:
		default:
			mat = regularGearMat;
			break;
		}
		
		foreach(var piece in pieces)
		{
			piece.GetComponent<MeshRenderer>().material = mat;
		}
	}

	public void Reset()
	{
		SetColor(Status.Running);
		isRunning = true;
		canControl = false;
		isForward = true;
		playerIsOn = false;
	}
}
