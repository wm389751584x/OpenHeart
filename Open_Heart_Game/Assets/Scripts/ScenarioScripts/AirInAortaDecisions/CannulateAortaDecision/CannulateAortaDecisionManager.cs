using UnityEngine;
using System.Collections;

public class CannulateAortaDecisionManager : DecisionManagerAbstractClass {


    private TubePlacement AortaTubePlacementScript;

    private CannulateAortaDecisionManager thisScript;

    public GameObject ArterialClampPlacement;
    private TubeClampTarget ArterialClampTargetScript;

	// Use this for initialization
	void Awake () {
        AortaTubePlacementScript = gameObject.GetComponent<TubePlacement>();
        thisScript = gameObject.GetComponent<CannulateAortaDecisionManager>();
        ArterialClampTargetScript = ArterialClampPlacement.GetComponent<TubeClampTarget>();
	}
	
	// Update is called once per frame
	void Update () {
        
        if ((ArterialClampTargetScript.Clamped == false) && AortaTubePlacementScript.hasTube && AortaTubePlacementScript.connectedTube.name.Equals("ArterialLineTubeIKTarget") && ArterialClampPlacement)
        {
           
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
