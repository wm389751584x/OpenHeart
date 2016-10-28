using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DecisionTree : MonoBehaviour {

    public string filename;

    private static DecisionNode root;


    void Start()
    {
        // construct the decision tree
        root = DFSConstructFromXML.DFSConstructTreeFromXML(filename);
        //printTree(root);
        

        if (root == null)
        {
            throw new System.Exception("Empty tree");
        }


        MakeNodeActiveDecision(root);
    }




    /// <summary>
    /// Whenever a new decision is made, the decision tree must activate
    /// the next DecisionNode's DecisionManager script. 
    /// the DecisionManager is responsible for deactivating itself, to prevent
    /// asynchronous errors
    /// </summary>
    /// <param name="decision"></param>
    public static void DecisionMade(DecisionManagerAbstractClass.Decisions decision)
    {       
        if (root == null)
        {
            throw new System.Exception("A DecisionManager script was not properly deactivated, and is incorrectly firing an OnDecision event");
        }
        switch (decision)
        {
            case DecisionManagerAbstractClass.Decisions.Correct:   
                root = root.correctChild;
                break;
            case DecisionManagerAbstractClass.Decisions.Incorrect:
                root = root.incorrectChild;
                break;
        }


        if (root != null)
        {
            MakeNodeActiveDecision(root);
        }
        
        
    }



    private static void MakeNodeActiveDecision(DecisionNode node)
    {
        


        node.decisionScript.ActivateDecisionManager();
        

        foreach (List<OnNodeEntryAbstractClass> onEntryList in node.onNodeEntryClasses.Values)
        {
            foreach (OnNodeEntryAbstractClass entryClass in onEntryList)
            {
                entryClass.nodeEntryMethod();
            }
        }

        foreach (List<SubscribedAbstractClass> subscribedList in node.subscribedClasses.Values)
        {
            foreach (SubscribedAbstractClass subscribedClass in subscribedList)
            {            
                DecisionManagerAbstractClass.OnDecisionEvent += subscribedClass.subscribedMethod;
            }
        }
    }



    public static void printTree(DecisionNode node)
    {

        if (node == null)
        {
            return;
        }

        print("Node DecisionScript:" + node.decisionScript);
        
        foreach (List<SubscribedAbstractClass> subscribedList in node.subscribedClasses.Values)
        {
            foreach (SubscribedAbstractClass subscribedClass in subscribedList)
            {
                print(subscribedClass);
            }
        }
        foreach (List<OnNodeEntryAbstractClass> onEntryList in node.onNodeEntryClasses.Values)
        {
            foreach (OnNodeEntryAbstractClass entryClass in onEntryList)
            {
                print(entryClass);
            }
        }

        printTree(node.correctChild);
        printTree(node.incorrectChild);

         
    }



}
