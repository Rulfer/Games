using UnityEngine;
using System.Collections;

public class gripPoint : MonoBehaviour {

    public Transform grip;
    public Weapon weap;
    public viveCharControl viveController;
    [Range(0, 1)]
    public float adjust;

    void FixedUpdate()
    {
        if (viveController != null)
        {
            if (viveController.gripped == true && weap.held == true)
            {
                //Vector3 newDir = Vector3.RotateTowards(this.transform.forward, viveController.transform.position, 1, 0.0f);
                //weap.transform.rotation = Quaternion.LookRotation(newDir);

                weap.transform.LookAt(2 * weap.transform.position - viveController.transform.position - (Vector3.down * adjust));
            }
        }
    }

}
