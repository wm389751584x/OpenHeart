using UnityEngine;
using System.Collections;

public class TestSubscribedClass : SubscribedAbstractClass {

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void subscribedMethod(DecisionManagerAbstractClass.Decisions decision)
    {
        switch (decision)
        {
            case DecisionManagerAbstractClass.Decisions.Correct:
                print("moving to correct child");
                break;
            case DecisionManagerAbstractClass.Decisions.Incorrect:
                print("moving to incorrect child");
                break;
        }
    }
}
