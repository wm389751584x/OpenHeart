using UnityEngine;
using System.Collections;

public class BubbleEffect : MonoBehaviour {
    public GameObject LookatObject;

	//testing for this script
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(LookatObject.transform.position);
	}
}
