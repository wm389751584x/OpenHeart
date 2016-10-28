using UnityEngine;
using System.Collections;

public class ShowAirInArterialLineDecisionManager : DecisionManagerAbstractClass{

    public GameObject particleSystem;

    ShowAirInArterialLineDecisionManager thisScript;
    float timeBecameActiveDecisionManager = 0.0f;
    float timeTillDeath = 20.0f;

    void Awake()
    {
        thisScript = gameObject.GetComponent<ShowAirInArterialLineDecisionManager>();
    }
    
	// Use this for initialization
	void Start () {
	
	}

    protected override void ActivateDecisionManagerScript()
    {
        thisScript.enabled = true;
        timeBecameActiveDecisionManager = Time.time;
        particleSystem.SetActive(true);
    }

    protected override void DeactivateDecisionManagerScript()
    {
        thisScript.enabled = false;
        particleSystem.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (isActiveDecisionManager)
        {
            if (Time.time - timeBecameActiveDecisionManager > timeTillDeath)
            {
                Decision(Decisions.Incorrect);
            }
            if (Pump.isOn == false)
            {
                Decision(Decisions.Correct);
            }
        }
	}

 

}
