using UnityEngine;
using System.Collections;

public class ReloadPoint : MonoBehaviour {

    public magazine mag;
    public Weapon weap;

    void OnTriggerStay(Collider col)
    {
        if (col.GetComponent<magazine>() != null && weap.weaponType == col.GetComponent<magazine>().weaponType)
        {
            if (col.gameObject.tag == "Magazine" && col.GetComponent<magazine>().ableToUse == true)
            {
                if (weap.mag == null)
                {
                    weap.reload(col.GetComponent<magazine>());
                }
            }
        }
    }
}
