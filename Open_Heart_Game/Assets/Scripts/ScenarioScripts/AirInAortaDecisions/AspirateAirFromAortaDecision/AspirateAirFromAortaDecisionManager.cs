using UnityEngine;
using System.Collections;

public class AspirateAirFromAortaDecisionManager : DecisionManagerAbstractClass {

    AspirateAirFromAortaDecisionManager thisScript;

    public GameObject ArterialLineIK;

    public GameObject SVCTubePlacement;
    TubePlacement SVCTubePlacementScript;

    public GameObject ArterialClamp;
    TubeClampTarget ArterialClampScript;

    

    void Awake()
    {
        thisScript = gameObject.GetComponent<AspirateAirFromAortaDecisionManager>();
        SVCTubePlacementScript = SVCTubePlacement.GetComponent<TubePlacement>();
        ArterialClampScript = ArterialClamp.GetComponent<TubeClampTarget>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (SVCTubePlacementScript.hasTube && SVCTubePlacementScript.connectedTube.Equals(ArterialLineIK) && ArterialClampScript.Clamped == false && Pump.isOn)
        {
            Debug.Log("yes");
            Decision(Decisions.Correct);
        }
	}

    protected override void ActivateDecisionManagerScript()
    {
        thisScript.enabled = true;
    }

    protected override void DeactivateDecisionManagerScript()
    {
        thisScript.enabled = false;
    }
}
