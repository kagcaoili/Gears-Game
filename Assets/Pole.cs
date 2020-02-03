using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pole : MonoBehaviour
{
	public List<Gear> gearsHit;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }
	
	/*
	public void RemoveFromGears()
	{
		foreach(var gear in gearsHit)
		{
			gear.hitList.Remove(this);
		}
	}
	*/

    // Update is called once per frame
    void Update()
    {
		/*
        if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
			{
				if(hit.collider.gameObject.GetComponent<Pole>())
				{
					Destroy(hit.collider.gameObject);
				}
			}
		}
		*/
    }

	private void OnTriggerEnter(Collider other)
	{
		var gearHit = other.gameObject.GetComponent<Gear>();
		if(gearHit != null)
		{
			gearsHit.Add(gearHit);
			gearHit.SetColor(Gear.Status.Stopped);
		}
	}
}
