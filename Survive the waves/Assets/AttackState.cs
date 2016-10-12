using UnityEngine;
using System.Collections;
using FlowPathfinding;
public class AttackState : MonoBehaviour 
{
    float stopAtThisDistance = 1.0f;
    private GameObject player;
    bool hunting = true;
	// Use this for initialization
	void Start () 
    {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (hunting)
        {
            float dist = Vector3.Distance(player.transform.position, this.transform.position);
            if (dist < stopAtThisDistance)
                StartAttacking();
        }
        
	}

    private void StartAttacking()
    {
        hunting = false;
        GameManager.manager.RemoveEnemyFromSeeking(this.gameObject);
    }
}
