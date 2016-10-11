using UnityEngine;
using System.Collections;
using FlowPathfinding;

public class FireGun : MonoBehaviour 
{
	public GameObject bulletHole;

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown (0)) 
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane))
            {
                Vector3 hitPosition = hit.point;
                Quaternion hitRotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
                if (hit.transform.tag == "Head")
                {
                    hit.transform.gameObject.GetComponent<RemoveBodyparts>().myParent.transform.GetComponent<EnemyStats>().health = 0;
                    hit.transform.gameObject.GetComponent<RemoveBodyparts>().RemoveBodypart();
                    GameManager.manager.DeleteEnemy(hit.transform.gameObject.GetComponent<RemoveBodyparts>().myParent);
                    Debug.Log(hit.transform.tag);
                    hit.transform.gameObject.GetComponent<RemoveBodyparts>().myParent.transform.GetComponent<AudioSource>().enabled = false;
                }
                if (hit.transform.tag == "Enemy" || hit.transform.tag == "Thigh" || hit.transform.tag == "Calf" || hit.transform.tag == "Foot" || hit.transform.tag == "Spine"
                    || hit.transform.tag == "UpperArm" || hit.transform.tag == "Forearm"  || hit.transform.tag == "Hand")
                {
                    Debug.Log(hit.transform.tag);

                    hit.transform.gameObject.GetComponent<RemoveBodyparts>().myParent.transform.GetComponent<EnemyStats>().health -= 20;
                    hit.transform.gameObject.GetComponent<RemoveBodyparts>().RemoveBodypart();
                    if (hit.transform.gameObject.GetComponent<RemoveBodyparts>().myParent.GetComponent<EnemyStats>().health == 0)
                    {
                        GameManager.manager.DeleteEnemy(hit.transform.gameObject.GetComponent<RemoveBodyparts>().myParent);
                        hit.transform.gameObject.GetComponent<RemoveBodyparts>().myParent.transform.GetComponent<AudioSource>().enabled = false;
                    }
                }
            }
		}
	}
}
