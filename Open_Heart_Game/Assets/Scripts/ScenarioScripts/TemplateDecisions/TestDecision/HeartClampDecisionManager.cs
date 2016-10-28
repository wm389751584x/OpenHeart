using UnityEngine;
using System.Collections;

public class HeartClampDecisionManager : DecisionManagerAbstractClass {

    void Update()
    {
        
        if (Input.GetKeyDown("a"))
        {
            Decision(DecisionManagerAbstractClass.Decisions.Correct);
        }
        else if (Input.GetKeyDown("space"))
        {
            Decision(DecisionManagerAbstractClass.Decisions.Incorrect);
        }
        
    }


    protected override void ActivateDecisionManagerScript()
    {
        throw new System.NotImplementedException();
    }

    protected override void DeactivateDecisionManagerScript()
    {
        throw new System.NotImplementedException();
    }
}
