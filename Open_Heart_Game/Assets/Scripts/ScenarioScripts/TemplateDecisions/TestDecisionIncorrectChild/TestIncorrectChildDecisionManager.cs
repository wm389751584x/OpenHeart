using UnityEngine;
using System.Collections;

public class TestIncorrectChildDecisionManager : DecisionManagerAbstractClass {
    protected override void ActivateDecisionManagerScript()
    {
        throw new System.NotImplementedException();
    }

    protected override void DeactivateDecisionManagerScript()
    {
        throw new System.NotImplementedException();
    }


}
