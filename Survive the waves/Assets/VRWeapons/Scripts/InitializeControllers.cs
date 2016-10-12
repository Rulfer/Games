using UnityEngine;
using System.Collections;

public class InitializeControllers : MonoBehaviour
{   public SteamVR_TrackedObject[] Controllers;

    void Awake()                        // All this script does is add viveCharControl to each controller. Not necessary if manually added
    {
        foreach (SteamVR_TrackedObject obj in Controllers)
        {
            if (obj.gameObject.GetComponent<Collider>() == null)
            {
                obj.gameObject.AddComponent<SphereCollider>();
                obj.gameObject.GetComponent<SphereCollider>().radius = 0.04f;
                obj.gameObject.GetComponent<SphereCollider>().isTrigger = true;
                obj.gameObject.GetComponent<SphereCollider>().center = new Vector3(0, -0.04f, 0.02f);
            }
            obj.gameObject.AddComponent<viveCharControl>();
        }
    }
}
