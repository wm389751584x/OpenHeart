using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Linq;
using System;

public class DFSConstructFromXML  {

    /// <summary>
    /// Constructs a tree from an xml file specified by the filename parameter, and returns the root node.
    /// Filename is constructed using the Unity project's relative path.
    /// </summary>
    /// <returns></returns>
    // Helper method to DFSConstructTreeFromXML(XElement n)
    public static DecisionNode DFSConstructTreeFromXML(String filename)
    {
        XDocument doc = XDocument.Load(Path.Combine(Application.dataPath, 
                               filename));

        return  DFSConstructTreeFromXML(doc.Root);
        
    }

    // Parses the XML document using a depth-first pre-order recursive algorithm, building the tree
    // in the process.
    // TODO refactor this method. write more generic methods to lower the complexity.
    private static DecisionNode DFSConstructTreeFromXML(XElement currentElement)
    {

        // the xml element has no child elements, and therefore is a base case
        if (currentElement.Elements().ToList().Count == 0)
        {
            return null;
        }


        DecisionNode newNode = new DecisionNode();


        // extract the decisionScript element from the currentElement
        IList<XElement> decisionScriptList = currentElement.Elements("decisionScript").ToList();
        if (decisionScriptList.Count != 1) 
        {
            throw new System.Exception("Error in XML document's tree structure."
                +" Error in element:"+ currentElement.ToString()+
                ". Incorrect number of decisionScripts. Counted:" + decisionScriptList.Count +". Expected:1");
        }

        // get the name of the GameObject containing decisionScript
        String decisionGameObjectString = (String)decisionScriptList[0].Attribute("GameObject");
        if (decisionGameObjectString == null)
        {
            throw new System.Exception("Error in XML document's tree structure. No GameObject attribute found in element:"+
                decisionScriptList[0].ToString());
        }
        newNode.decisionGameObject = GameObject.Find(decisionGameObjectString);
        if (newNode.decisionGameObject == null)
        {
            throw new System.Exception("Error in XML document. GameObject " + decisionGameObjectString + " was not found.");
        }


        // get the decisionScript attached to the decisionGameObject
        String decisionScriptString = (String)decisionScriptList[0].Attribute("name");
        if (decisionScriptString == null)
        {
            throw new System.Exception("Error in XML document's tree structure. No name attribute found in element:" +
                decisionScriptList[0].ToString());
        }
        newNode.decisionScript = newNode.decisionGameObject.GetComponent(decisionScriptString) as DecisionManagerAbstractClass;
        if (newNode.decisionScript == null)
        {
            throw new System.Exception("Error in XML document. The script " + decisionScriptString + 
                " was not found on the GameObject " + decisionGameObjectString);
        }


        // disable the decisionScript (DecisionManager) at startup to be more efficient
        // this can be disabled in the individual DecisionManager's awake functions
        // see DecisionMaangerAbstractClass
        if (newNode.decisionScript.DeactivateAtStartup)
        {
            newNode.decisionScript.DeactivateDecisionManager();
        }


        // check if there is an isDead attribute for this node
        // if none is found then default value is false, if multiple
        // are found, the first isDead element will be used
        IList<XElement> isDeadList = currentElement.Elements("isDead").ToList();
        if (isDeadList.Count == 0)
        {
            newNode.isDead = false;
        }
        else
        {
            switch((String)isDeadList[0].Attribute("value"))
            {
                case "true":
                    newNode.isDead = true;
                    break;
                default:
                    newNode.isDead = false;
                    break;
            }
        }

        // process the subscribedClasses
        // if there is an error in processing any of the elements, a warning message will print to
        // the console and that element will be ignored
        IList<XElement> subscribedClassesList = currentElement.Elements("subscribedClass").ToList();
        foreach (XElement element in subscribedClassesList)
        {
            String subscribedGameObjectString = (String)element.Attribute("GameObject");
            if (subscribedGameObjectString == null)
            {
                Debug.Log("Could not find subscribedClass GameObject attribute for element:" + element.ToString());
                continue;
            }

            GameObject subscribedGameObject = GameObject.Find(subscribedGameObjectString);
            if (subscribedGameObject == null)
            {
                Debug.Log("Could not find GameObject:" + subscribedGameObjectString +" for element:"+element.ToString());
                continue;
            }

            String subscribedSciptString = (String)element.Attribute("name");
            if (subscribedSciptString == null)
            {
                Debug.Log("Could not find subscribedClass name attribute for element:" + element.ToString());
                continue;
            }

            SubscribedAbstractClass subscribedClassScript = subscribedGameObject.GetComponent(subscribedSciptString) as SubscribedAbstractClass;
            if (subscribedClassScript == null)
            {
                Debug.Log("Could not find subscribedClassScript:+" +subscribedSciptString+ " for GameObject:" + subscribedGameObjectString + " for element:" + element.ToString());
                continue;
            }

            // insert the new subscribed method into the dictionary's list, if the key
            // already exists, then add the script to the list. If not, create a new list and add to the dictionary
            List<SubscribedAbstractClass> subscribedAbstractClassList;
            if (newNode.subscribedClasses.TryGetValue(subscribedGameObject, out subscribedAbstractClassList))
            {
                subscribedAbstractClassList.Add(subscribedClassScript);
            }
            else
            {
                subscribedAbstractClassList = new List<SubscribedAbstractClass>();
                subscribedAbstractClassList.Add(subscribedClassScript);
                newNode.subscribedClasses.Add(subscribedGameObject, subscribedAbstractClassList);
            }
        }


        // process the onNodeEntryClasses
        // if there is an error in processing any of the elements, a warning message will print to
        // the console and that element will be ignored
        IList<XElement> onNodeEntryClassesList = currentElement.Elements("onNodeEntryClass").ToList();
        foreach (XElement element in onNodeEntryClassesList)
        {
            String onNodeEntryGameObjectString = (String)element.Attribute("GameObject");
            if (onNodeEntryGameObjectString == null)
            {
                Debug.Log("Could not find onNodeEntryClass GameObject attribute for element:" + element.ToString());
                continue;
            }

            GameObject onNodeEntryGameObject = GameObject.Find(onNodeEntryGameObjectString);
            if (onNodeEntryGameObject == null)
            {
                Debug.Log("Could not find GameObject:" + onNodeEntryGameObjectString + " for element:" + element.ToString());
                continue;
            }

            String onNodeEntrySciptString = (String)element.Attribute("name");
            if (onNodeEntrySciptString == null)
            {
                Debug.Log("Could not find onNodeEntryClass name attribute for element:" + element.ToString());
                continue;
            }

            OnNodeEntryAbstractClass onNodeEntryClassScript = onNodeEntryGameObject.GetComponent(onNodeEntrySciptString) as OnNodeEntryAbstractClass;
            if (onNodeEntryClassScript == null)
            {
                Debug.Log("Could not find OnNodeEntryClassScript:+" + onNodeEntrySciptString + " for GameObject:" + onNodeEntryGameObjectString + " for element:" + element.ToString());
                continue;
            }

            // insert the new subscribed method into the dictionary's list, if the key
            // already exists, then add the script to the list. If not, create a new list and add to the dictionary
            List<OnNodeEntryAbstractClass> onNodeEntryAbstractClassList;
            if (newNode.onNodeEntryClasses.TryGetValue(onNodeEntryGameObject, out onNodeEntryAbstractClassList))
            {
                onNodeEntryAbstractClassList.Add(onNodeEntryClassScript);
            }
            else
            {
                onNodeEntryAbstractClassList = new List<OnNodeEntryAbstractClass>();
                onNodeEntryAbstractClassList.Add(onNodeEntryClassScript);
                newNode.onNodeEntryClasses.Add(onNodeEntryGameObject, onNodeEntryAbstractClassList);
            }
        }


        // create children.
        IList<XElement> correctChildList = currentElement.Elements("correctChild").ToList();
        if (correctChildList.Count > 1)
        {
            throw new System.Exception("Error in XML document's structure. Element " + currentElement.ToString() 
                + " contains too many correct children.");
        }
        else if (correctChildList.Count == 1)
        {
            newNode.correctChild = DFSConstructTreeFromXML(correctChildList[0]);
        }
        else
        {
            newNode.correctChild = null;
        }

        IList<XElement> incorrectChildList = currentElement.Elements("incorrectChild").ToList();
        if (incorrectChildList.Count > 1)
        {
            throw new System.Exception("Error in XML document's structure. Element " + currentElement.ToString()
                + " contains too many incorrect children.");
        }
        else if (incorrectChildList.Count == 1)
        {
            newNode.incorrectChild = DFSConstructTreeFromXML(incorrectChildList[0]);
        }
        else
        {
            newNode.incorrectChild = null;
        }


        return newNode;
    }



	
}
