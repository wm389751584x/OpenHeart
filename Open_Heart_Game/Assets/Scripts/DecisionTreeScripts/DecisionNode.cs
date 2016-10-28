using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DecisionNode {

    /// <summary>
    /// use correctChild for correct and neutral 
    /// decisions (decisions/actions which have no wrong answer/action)
    /// </summary>
    public DecisionNode correctChild;

    /// <summary>
    /// use incorrectChild if the user has the ability to do harm to the patient
    /// </summary>
    public DecisionNode incorrectChild;

    /// <summary>
    /// true if this node of the DecisionTree
    /// kills the patient
    /// </summary>
    public bool isDead; 
    
    /// <summary>
    /// a game object in our scene which will
    /// have the decisionScript attached.
    /// </summary>
    public GameObject decisionGameObject;

    /// <summary>
    /// script determining what decision we make.
    /// should be attached to the decisionGameObject.
    /// DecisionManagerAbstractClass is abstract, so you must
    /// implement a new class for every decision.
    /// </summary>
    public DecisionManagerAbstractClass decisionScript;

    /// <summary>
    /// A dictionary of GameObjects and their corresponding attached scripts,
    /// which will have its subscribedMethod subscribed to the DecisionManagerAbstractClass'
    /// OnDecisionEvent event variable.
    /// value is a list in case you want multiple classes that are attached to the same object
    /// </summary>
    public Dictionary<GameObject, List<SubscribedAbstractClass>> subscribedClasses;

    /// <summary>
    /// A dictionary of GameObjects and their corresponding attached scripts,
    /// which will have its nodeEntryMethod executed when this node becomes
    /// the current node in the DecisionTree.
    /// value is a list in case you want multiple classes that are attached to the same object
    /// </summary>
    public Dictionary<GameObject, List<OnNodeEntryAbstractClass>> onNodeEntryClasses;



    public DecisionNode()
    {
        correctChild = null;
        incorrectChild = null;
        isDead = false;
        decisionGameObject = null;
        decisionScript = null;
        subscribedClasses = new Dictionary<GameObject, List<SubscribedAbstractClass>>();
        onNodeEntryClasses = new Dictionary<GameObject, List<OnNodeEntryAbstractClass>>();
    }

}
