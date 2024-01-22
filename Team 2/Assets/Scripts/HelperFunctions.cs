using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunctions
{
    /// <summary>
    /// finds a component in a child that has a tag
    /// </summary>
    /// <typeparam name="T">any type of component</typeparam>
    /// <param name="parent">parent gameobject</param>
    /// <param name="tag">tag to search for</param>
    /// <param name="forceActive">always set to false</param>
    /// <returns>a component of a child that has a specific tag</returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag, bool forceActive = false) where T : Component
    {
        //checks to make sure parameters are entered right. and nothing is null or empty
        if (parent == null)
        {
            throw new System.ArgumentNullException("parent is null");
        }
        if (string.IsNullOrEmpty(tag))
        {
            throw new System.ArgumentNullException(tag);
        }

        //makes a list of components in the children of the parent
        List<T> list = new List<T>(parent.GetComponentsInChildren<T>(forceActive));

        //another check to make sure its working properly
        if (list.Count == 0)
        {
            Debug.Log("No children have that component");
            return null;
        }

        //finds a child with the tag from the list of components
        foreach (T component in list)
        {
            if (component.CompareTag(tag))
            {
                return component;
            }
        }

        //returns null if it doesnt find anything
        return null;

    }

    /// <summary>
    /// finds a child with a tag
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="tag"></param>
    /// <returns>a child of the parent</returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static GameObject FindChildWithTag(this GameObject parent, string tag)
    {
        //checks to make sure its working right
        if (parent == null)
        {
            throw new System.ArgumentNullException("parent is null");
        }
        if (string.IsNullOrEmpty(tag))
        {
            throw new System.ArgumentNullException(tag);
        }

        //gets a list of all children with a transform
        List<Transform> list = new List<Transform>(parent.GetComponentsInChildren<Transform>());

        //another check to make sure its working properly
        if (list.Count == 0)
        {
            Debug.Log("No children have the transorm component");
            return null;
        }

        //checks tag of each child the parent has
        foreach (Transform component in list)
        {
            //returns the first child it finds with the tag
            if (component.gameObject.CompareTag(tag))
            {
                return component.gameObject;
            }
        }

        //otherwise it returns null
        return null;
    }
}
