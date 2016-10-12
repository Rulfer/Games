using UnityEngine;
using System.Collections;

public class hitObject : MonoBehaviour {

    public int health = 1;

    public void OnHit(int damage)
    {
        health -= damage;
        if (health <= 0)
            Destroy(gameObject);
    }
}
