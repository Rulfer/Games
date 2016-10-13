using UnityEngine;
using System.Collections;
using FlowPathfinding;
public class DeleteZombiesAtPoint : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Zombie")
        {
            GameManager.manager.CompletelyRemove(other.gameObject);   
        }
    }
}
