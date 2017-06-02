using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudRefresh : MonoBehaviour
{

    public Text textHolder;
	// Use this for initialization
	void Start ()
	{

	    textHolder = GameObject.Find("HoloLensCamera/Canvas/Text").GetComponent<Text>();
	   


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}




 
