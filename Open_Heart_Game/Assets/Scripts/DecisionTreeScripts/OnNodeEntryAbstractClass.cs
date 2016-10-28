using UnityEngine;
using System.Collections;

/// <summary>
/// Class that you must implement for every GameObject that
/// has to perform a behavior upon a entry into a node in the
/// decision tree.
/// </summary>
// The difference between OnNodeEntryAbstractClass and SubscribedAbstractClass 
// is that the classes that implement OnNodeEntry will implement a behavior
// that must execute during that decisionNode regardless of any decisionNode before it.
// You may think that you can accomplish this using SubscribedAbstractClass, and you are
// correct. You would have to program any behavior that you want to happen in the current DecisionNode
// in the DecisionNode before it. But DON'T DO THIS. 
// You will be making behaviors that are required of the current DecisionNode dependent on the DecisionNode before it
// which means that if you need to alter some steps before a particular DecisionNode, you would have to 
// alter in the DecisionTree wher the SubscribedAbstractClass would be called. Which would be more work

// Best practice for choosing between SubscribedAbstractClass and OnNodeEntryClass:
// 1. Create a new class which implements OnNodeEntryAbstractClass for a behavior for a particular DecisionNode: Y
// IF the behavior for Y needs to execute this behavior when it becomes the current DecisionNode
// and this behavior is independently from any DecisionNode which comes before Y in the DecisionTree's hierarchy.
// 2. Create a new class which implements SubscribedAbstractClass for a behavior for a particular DecisionNode: X
// IF the behavior for X needs to execute that behavior when a decision is made and 
// X is no longer considered the current DecisionNode, and if that behavior needs to be executed
// independently from any DecisionNode which comes after X.
public abstract class OnNodeEntryAbstractClass : MonoBehaviour {

    /// <summary>
    /// method that will execute when the node of interest
    /// becomes the current node.
    /// </summary>
    abstract public void nodeEntryMethod();

	
}
