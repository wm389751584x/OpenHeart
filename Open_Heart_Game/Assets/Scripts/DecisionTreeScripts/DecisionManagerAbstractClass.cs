using UnityEngine;
using System.Collections;


/// <summary>
/// the method Decision gets called whenever a new
/// Decision is made. The class that calls this method
/// should be an implementation of DecisionManagerAbstractClass.
/// this class uses an event variable to call
/// other methods when a decision is reached
/// other class methods can subscribe to this event 
/// variable. See SubscribedAbstractClass and
/// unity's tutorial on events for more information
/// </summary>
public abstract class DecisionManagerAbstractClass : MonoBehaviour {

    public delegate void DecisionAction(Decisions decision);
    public static event DecisionAction OnDecisionEvent;

    // if you don't want a DecisionManager that you implement to 
    // be deactivated at startup, then set this bool in the inspector (or Awake())
    // reason for Deactivating at startup is not have any unecessary computations
    // until it is the Scripts turn to be the DecisionManager
    public bool DeactivateAtStartup = true;

    /// <summary>
    /// don't set this variable yourself. The class DecisionTree should take care
    /// of it for you by calling ActivateDecisionManager. Should only read this variable
    /// if needed.
    /// </summary>
    /// could have done {get; private set;} but led to issues with when the 
    /// variable would be set to false. so just don't mess with it, just read it if you need
    /// it in the class that implements DecisionManagerAbstractClass
    protected bool isActiveDecisionManager = false;
    
    
    /// <summary>
    /// Decisions that the DecisionTree can take
    /// </summary>
    public enum Decisions
    {
        Correct,
        Incorrect
    }

    
    /// <summary>
    /// This method should be called by the class that implements
    /// DecisionManagerAbstractClass when it has satisfied the 
    /// requirements of a decision. Correct, or incorrect
    /// </summary>
    /// <param name="decision"></param>
    protected void Decision(Decisions decision)
    {
        if (isActiveDecisionManager)
        {
            DeactivateDecisionManager();
            DeactivateDecisionManagerScript();
            if (OnDecisionEvent != null)
            {
                OnDecisionEvent(decision);
            }
            // at this point all events fired, so set to null
            // then call your decisionMade function to set up the next decision
            OnDecisionEvent = null;
            DecisionTree.DecisionMade(decision);
            
        }
       
    }

 
    /// <summary>
    /// The method that can be accessed by the DecisionTree
    /// </summary>
    public void ActivateDecisionManager()
    {
        ActivateDecisionManagerScript();
        isActiveDecisionManager = true;
    }

    // declared abstract so that you can enable the script in the following ways:
    // 1. the script just has an update function. in that case just set the script's component's 
    // enabled variable to true
    // 2. the script just has physics functions like onTriggerEnter. in that case you just have to enable
    // the collider associated with the collision
    // 3. both 1 & 2.
    // see ClampAortaDecisionManager class for an example on step 3.
    // this method should reverse any method you write for DeactivateDecisionManagerScript
    // you need to find all the scripts and components in the awake portion of the script
    // that are going to be used by this method and deactivate
    protected abstract void ActivateDecisionManagerScript();


    // this may seem like a redundant method to DeactivateDecisionManagerScript()
    // but this method is to prevent fatal errors if you forget to disable a box collider.
    // this method prevents fatal errors in the decisionTree
    // but doesn't cover up any errors you have made from forgetting to disable components.
    // see step 2 below.
    public void DeactivateDecisionManager()
    {
        isActiveDecisionManager = false;
        DeactivateDecisionManagerScript();
    }

    // declared abstract so that you can override however is necessary:
    // 1. if your script is just using the update method, and no physics methods,
    // then disable the script by setting the enabled property to false in the
    // overriden method inside your script. See ClampAortaDecisionManager class for example
    // 2. if you are using physics functions that use colliders
    // and your goal is to disable these, simply disabling the script like in step one will not
    // work. you must disable the collider component. This is because Unity has a feature that will still send collision
    // events to scripts that are disabled, so that they can enable themselves.
    // 3. you don't want to deactivate anything, in that case just leave the method blank in your implementation
    // This method is also called at the startup when the decision tree is loaded. If you don't want this to 
    // happen then set the DeactivateAtStartup bool to false in the inspector, or Awake in the implementation
    // also make sure you set the reference to your own script in the awake() method. so you aren't trying to 
    // reference a variable that hasn't been assigned yet in DeactivateDecisionManagerScript()
    protected abstract void DeactivateDecisionManagerScript();

    
}
