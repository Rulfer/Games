using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(SteamVR_TrackedObject))]

public class viveCharControl : MonoBehaviour {

    #region Declaring variables
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId trigger = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId dpadAxis0 = Valve.VR.EVRButtonId.k_EButton_Axis0;
    private Valve.VR.EVRButtonId dpadAxis1 = Valve.VR.EVRButtonId.k_EButton_Axis1;
    private Valve.VR.EVRButtonId dpadAxis2 = Valve.VR.EVRButtonId.k_EButton_Axis2;
    private Valve.VR.EVRButtonId dpadAxis3 = Valve.VR.EVRButtonId.k_EButton_Axis3;
    private Valve.VR.EVRButtonId dpadAxis4 = Valve.VR.EVRButtonId.k_EButton_Axis4;
    private Valve.VR.EVRButtonId touchpad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private Valve.VR.EVRButtonId appMenu = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;

    SteamVR_TrackedObject trackedObj;
    FixedJoint fixedJoint;
    SteamVR_Controller.Device device;

    gripPoint grip;
    public Vector3 gripPoint;
    private Weapon weapon;
    public GameObject holdPoint;
    public bool held;           
    public GameObject heldItem;
    viveCharControl otherDevice;
    private float dropTime;
    public float weapRotate, breakDist, teleportDist;
    public bool gripped = false;
    #endregion

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    #region Fire
    void fire()
    {
        if (weapon != null)
            weapon.fire(device);
    }
    
    void StopFiring()
    {
        if (weapon != null)
            weapon.StopFiring();
    }

    #endregion

    #region Check for motion controller input
    void checkForInput(SteamVR_Controller.Device device)
    {
        if (device.GetTouch(trigger))
        {
            if (weapon != null)
            {
                Vector2 triggerAngle = device.GetAxis(trigger);
                weapon.triggerPull(triggerAngle.x);
            }
        }
        if (device.GetPressDown(trigger))
        {
            fire();
        }
        if (!device.GetPress(trigger))
        {
            StopFiring();
        }
        if (device.GetPressUp(trigger))
        {
            StopFiring();
        }
        if (device.GetPressDown(touchpad))
        {
            if (weapon != null)
                if (weapon.mag != null)
                    weapon.DropMag();
        }
    }

    #endregion

    #region Fixed Update
    void FixedUpdate () {


        if (heldItem != null)
        {
            if (heldItem.transform.parent != this.transform)
            {
                drop(null);

            }
        }

        device = SteamVR_Controller.Input((int)trackedObj.index);

        checkForInput(device);
        
    }
    #endregion

    #region Motion controller is inside trigger. Used for picking up objects
    void OnTriggerStay(Collider col)
    {
        if (device.GetPressDown(gripButton))
        {
            if (gripped)
            {
                gripped = false;
                StopCoroutine(CheckDistanceIfGripped());
                otherDevice = grip.gameObject.transform.parent.transform.parent.GetComponent<viveCharControl>();
                otherDevice.alignWithDevice(otherDevice.heldItem.transform);
            } else if (!held && (Time.time - dropTime > 0.2) && col.gameObject.layer != LayerMask.NameToLayer("Environment"))
            {
                heldItem = col.gameObject;
                grab(heldItem);
                dropTime = Time.time;
            } else if (held && (Time.time - dropTime > 0.2))
            {
                drop(heldItem);
                dropTime = Time.time;
            }
        }
    }
    #endregion

    #region Tossing objects
    void tossObject(Rigidbody rigidbody)                                                                        //-----------------------------------
    {                                                                                                           //
        Transform origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;                 //
                                                                                                                //
        if (origin != null)                                                                                     //
        {                                                                                                       //
            rigidbody.velocity = origin.TransformVector(device.velocity);                                       //      TOSSING OBJECTS
            rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity);                         //
        }                                                                                                       //      Conserves velocity and 
        else                                                                                                    //      angular velocity when
        {                                                                                                       //      rigidbody is released. 
            rigidbody.velocity = device.velocity;                                                               //      Prevents objects from 
            rigidbody.angularVelocity = device.angularVelocity;                                                 //      simply dropping to the 
        }                                                                                                       //      ground, enables throws                                                                                            
    }                                                                                                           //-----------------------------------
    #endregion

    #region Align weapons with motion controller
    void alignWithDevice(Transform weap)
    {
        if (weap.GetComponent<Weapon>() != null)
        {
            Vector3 adjustWeapRot = weap.GetComponent<Weapon>().adjustRotation;
            weap.transform.localEulerAngles = adjustWeapRot;
            float adjustWeapForward = weap.GetComponent<Weapon>().adjustForward;
            weap.transform.localPosition = new Vector3(0, -adjustWeapForward, 0);
        }
        else {
            weap.transform.localEulerAngles = new Vector3(-70, 180, 0);
            weap.transform.position = transform.position;
        }                                               
        transform.FindChild("Model").gameObject.SetActive(false);
    }
    #endregion
    
    #region Grabbing and releasing rigid bodies
    void grab(GameObject item)
    {
        if (item.tag == "ReloadPoint")
            return;
        if (item.tag == "Magazine")
        {
            magazine mag = item.GetComponent<magazine>();
            if (mag.transform.parent != null && mag.transform.parent.gameObject.layer == LayerMask.NameToLayer("Weapon"))
                return;
        }
        if (item.tag != "GripPoint" && item.layer != LayerMask.NameToLayer("Environment"))
        {
                if (item.GetComponent<Weapon>() != null)
                {
                    weapon = item.GetComponent<Weapon>();
                    weapon.held = true;
                }
            if (item.GetComponent<Rigidbody>() != null)
                item.GetComponent<Rigidbody>().isKinematic = true;
            item.transform.parent = this.transform;
            if (item.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            {
                alignWithDevice(item.transform);

            }
            held = true;
        } else if (item.tag == "GripPoint")
        {
            grip = item.GetComponent<gripPoint>();
            grip.viveController = this;
            gripped = true;
            StartCoroutine(CheckDistanceIfGripped());
        }
                                                                                                            
    }                                                                                                       
    void drop(GameObject item)                                                                              
    {
        if (item != null) { 
            if (item.tag == "GripPoint")
            {
                gripped = false;
                StopCoroutine(CheckDistanceIfGripped());
            }
            item.transform.SetParent(null);                                                                   
            item.GetComponent<Rigidbody>().isKinematic = false;                                                 
            tossObject(item.GetComponent<Rigidbody>());
            if (weapon != null)
            {
                weapon.held = false;
            }
        }
        held = false;
        weapon = null;                                                                             
        heldItem = null;
        transform.FindChild("Model").gameObject.SetActive(true);                            
    }                                                                                                     
    #endregion
    IEnumerator CheckDistanceIfGripped()
    {
        while (true)
        {
            float dist = Vector3.Distance(this.transform.position, grip.transform.position);
            if (dist > breakDist)
            {
                gripped = false;
                otherDevice = grip.gameObject.transform.parent.transform.parent.GetComponent<viveCharControl>();
                otherDevice.alignWithDevice(otherDevice.heldItem.transform);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
