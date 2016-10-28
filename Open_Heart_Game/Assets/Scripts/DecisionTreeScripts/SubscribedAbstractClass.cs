using UnityEngine;
using System.Collections;

/// <summary>
/// Class which you must implement for every object that is interested
/// in the outcome of a decision.
/// </summary>
// For example a GameObject that would be interested in the placing of the clamps
// would be the Heart. Once that clamp is placed correctly, the animator controller
// must stop the beating animation. So the HeartClampSubscribedClass is subscribed
// to that deicison, and when the clamps are placed the subscribedMethod is called to stop the heart.
// Subscription occurs in the DecisionTree class. So you must add any classes 
// you want subscribed to the DecisioTreeXML file.
// see OnNodeEntryAbstractClass for an explanation on the difference between 
// SubscribedAbstractClass and OnNodeEntryClass
public abstract class SubscribedAbstractClass : MonoBehaviour {


    void OnDisable()
    {
        DecisionManagerAbstractClass.OnDecisionEvent -= subscribedMethod;
    }

	/// <summary>
	/// method that is interested in the outcome of a decision
	/// </summary>
	/// <param name="decision">decision the user took</param>
    abstract public void subscribedMethod(DecisionManagerAbstractClass.Decisions decision);


}
