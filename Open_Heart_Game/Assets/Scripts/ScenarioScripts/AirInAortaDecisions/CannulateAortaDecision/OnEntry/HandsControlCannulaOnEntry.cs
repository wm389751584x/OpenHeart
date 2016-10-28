using UnityEngine;
using System.Collections;

public class HandsControlCannulaOnEntry : OnNodeEntryAbstractClass {

    public GameObject RightHand;
    public GameObject CannulaTarget;
    private HandMovement RightHandScript;
    private const float SCALE = 0.005f;


    public bool activated = false;
	// Use this for initialization
	void Awake () 
    {
        RightHandScript = RightHand.GetComponent<HandMovement>();
	}
	
	// Update is called once per frame
    
	void Update () {
        if (activated)
        {
            CannulaTarget.transform.position = RightHand.transform.position;
        }
   
	}

    public void deactivate()
    {
        activated = false;
        RightHandScript.removeClampHandMovement();
    }

    public override void nodeEntryMethod()
    {
        activated = true;
        RightHandScript.clampHandMovement(1.2f, 1.8f, -0.388f, 0.008f, -0.022f, 0.196f);
    }
}

