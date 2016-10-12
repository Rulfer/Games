using UnityEngine;
using System.Collections;

public class magazine : MonoBehaviour {
    public int ammo;
    public float magEjectForce;
    public Weapon weapon;
    public int weaponType;
    public bool ableToUse = true;

    public void dropMag()
    {
        weapon = this.transform.parent.GetComponent<Weapon>();
        transform.parent = null;
        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = true;
        if (GetComponent<Rigidbody>() != null)
        {
            //GetComponent<Rigidbody>().AddForce(Vector3.forward * magEjectForce, ForceMode.Impulse);       // Need to fix eject direction before using this
            GetComponent<Rigidbody>().isKinematic = false;
        }
        StartCoroutine(dropProtect());

    }
    IEnumerator dropProtect()
    {
        yield return new WaitForSeconds(1);
        ableToUse = true;
        yield break;
    }
}
