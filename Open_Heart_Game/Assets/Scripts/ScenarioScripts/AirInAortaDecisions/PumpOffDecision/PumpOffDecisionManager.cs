using UnityEngine;
using System.Collections;

public class PumpOffDecisionManager : DecisionManagerAbstractClass
{

    PumpOffDecisionManager thisScript;

    void Awake()
    {
        thisScript = gameObject.GetComponent<PumpOffDecisionManager>();
    }
    protected override void ActivateDecisionManagerScript()
    {
        thisScript.enabled = true;

    }

    protected override void DeactivateDecisionManagerScript()
    {
        thisScript.enabled = false;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!Pump.isOn)
        {
            Decision(Decisions.Correct);
        }

    }
}
