using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RemoveBodyparts : MonoBehaviour 
{
    public GameObject meshObject;
    public GameObject myParent;
    public List<GameObject> children;
    public void RemoveBodypart()
    {
        if (meshObject != null)
        {
            this.transform.gameObject.SetActive(false);
            meshObject.SetActive(false);
            foreach (GameObject go in children)
            {
                go.SetActive(false);
            }   
        }
    }
}
