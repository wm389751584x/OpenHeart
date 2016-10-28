using UnityEngine;
using System.Collections;

public class TestOnNodeEntryClass : OnNodeEntryAbstractClass {

	// Use this for initialization
	void Start () 
    {
        
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public override void nodeEntryMethod()
    {
        //throw new System.NotImplementedException();
        print("First Decision. Press 'a' for correct decision. 'space' for incorrect");
    }
}
