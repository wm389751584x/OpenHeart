using UnityEngine;
using System.Collections;

public class TubePlacement : MonoBehaviour {

    // whether a tube is connected, and the tube that is connected
    public bool hasTube;
    public GameObject connectedTube;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ConnectTube(GameObject tubeIKTarget)
    {
        hasTube = true;
        connectedTube = tubeIKTarget;
    }

    public void DisconnectTube()
    {
        hasTube = false;
        connectedTube = null;
    }
}
