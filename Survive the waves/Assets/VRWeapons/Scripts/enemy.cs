using UnityEngine;
using System.Collections;

public class enemy : MonoBehaviour {
    public int health = 100;
    // Use this for initialization
    void Awake () {
        gameObject.GetComponentInChildren<Renderer>().material.color = Color.red; 
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //Debug.Log("Health: " + health + " Damage: " + damage);

        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
	}
    
    public void OnHit(int damage)
    {
        health -= damage;
    }



}
