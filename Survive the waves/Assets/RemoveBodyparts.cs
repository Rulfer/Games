using UnityEngine;
using System.Collections;

public class RemoveBodyparts : MonoBehaviour 
{
    public GameObject meshObject;
    public GameObject myParent;
   
    public void RemoveBodypart()
    {
        this.transform.gameObject.SetActive(false);
        meshObject.SetActive(false);
    }
}
