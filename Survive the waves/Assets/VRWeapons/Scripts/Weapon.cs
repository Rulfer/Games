using UnityEngine;
using System.Collections;
using FlowPathfinding;


[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]

public class Weapon : MonoBehaviour {
    public enum FireMode {
		Bullet = 0,
		Projectile = 1,
		AutoFire = 2,
		BurstFire = 3,
	}

    #region Defining Variables
    public float range, ejectForce, force, slideLength, fireRate;
    public float kickStrength;
    public float rotKickStrength;
    public float recoverSpeed, projForce;
    public Vector3 adjustRotation;
    public float adjustForward;
    private float nextFire;
    public ushort hapticStrength;
    public int slideTime, triggerMult, feedbackTime, burst, damage;
    Coroutine autofire;
    bool chambered, VREnabled;
    public bool automaticChamber, held, isLoaded, muzzleDirectionSet, ejectorDirectionSet, rldPointSet, showMuzDirectionSetup, showEjectDirectionSetup, showRldPointSetup, showPrefabSave, showPairMag, kickSet, showKickSetup;
    public magazine mag;
    bool rackedBack = false;
    AudioSource sound;
    public AudioClip shotSound, magOut, magIn;
    //public checkVr CheckVR;
    public Transform gripPoint;
    public GameObject muzzleFlash;

    public int weaponType;
    public FireMode fireMode;

    public GameObject projectile, impact;
    public Transform ejector, muzzle, trigger, slide, muzzleDirection, ejectorDirection;
    private Vector3 triggerRotation, triggerOrigin;
    public Vector3 rotBack, kickBack, magOPos, magORot, magOSca;

    public GameObject bulletShell;
    #endregion

    void Awake() {              // Doing some initial defining of variables, figuring things out on start
        //if (CheckVR.useVr == true)
            VREnabled = true;
        //else
        
        //   VREnabled = false;
        sound = GetComponent<AudioSource>();
        nextFire = Time.time;
        if (trigger != null)
            triggerOrigin = trigger.localRotation.eulerAngles;  // For trigger pull on Vive controls
        kickStrength *= 0.05f;              // Just to give the slider values more "realistic" feel, but the kickStrength needs to be very light
        if (mag != null)
        {
            reload(mag);                        // Set up magazine for initial firing
        }
        if (recoverSpeed > fireRate)
            recoverSpeed = fireRate;            // Protects from losing origin values and getting a drifting gun

    }
    

    public void fire()
    {
        if (fireMode == FireMode.Bullet && chambered)   // Checks to see what weapon type the weapon is, and fires appropriately
        {
            fireBullet(null);
            chambered = false;
            if (automaticChamber)
            {
                if (mag.ammo > 0)
                    StartCoroutine(RackBack(true));
                else
                    StartCoroutine(RackBack(false));
            }
        }
        else if (fireMode == FireMode.Projectile && chambered)
        {
            fireProj(null);
            chambered = false;
            if (automaticChamber)
            {
                if (mag.ammo > 0)
                    StartCoroutine(RackBack(true));
                else
                    StartCoroutine(RackBack(false));
            }
        }

        else if (fireMode == FireMode.AutoFire && chambered)
            autofire = StartCoroutine(fireAuto(null));
        else if (fireMode == FireMode.BurstFire && chambered)
            autofire = StartCoroutine(fireBurst(null));
    }

    public void fire(SteamVR_Controller.Device device)
    {
        if (fireMode == FireMode.Bullet && chambered)   // Checks to see what weapon type the weapon is, and fires appropriately
        {
            fireBullet(device);
            chambered = false;
            if (automaticChamber)
            {
                if (mag != null)
                {
                    if (mag.ammo > 0)
                        StartCoroutine(RackBack(true));
                    else
                        StartCoroutine(RackBack(false));
                }
            }
        }
        else if (fireMode == FireMode.Projectile && chambered)
        {
            fireProj(device);
            chambered = false;
            if (automaticChamber)
            {
                if (mag.ammo > 0)
                    StartCoroutine(RackBack(true));
                else
                    StartCoroutine(RackBack(false));
            }
        }

        else if (fireMode == FireMode.AutoFire && chambered)
            autofire = StartCoroutine(fireAuto(device));
        else if (fireMode == FireMode.BurstFire && chambered)
            autofire = StartCoroutine(fireBurst(device));
    }

    public void StopFiring()
    {
		if (autofire == null) {
			return;
		}
        StopCoroutine(autofire);
		autofire = null;
    }

	public void SetFireMode (FireMode mode) {
		fireMode = mode;
	}

    void fireBullet(SteamVR_Controller.Device device)
    {
        if (Time.time - nextFire > fireRate)
        {

            RaycastHit hit;
            Debug.DrawRay(muzzle.position, (muzzleDirection.position - muzzle.position).normalized, Color.red, Mathf.Infinity);
            if (Physics.Raycast(muzzle.transform.position, (muzzleDirection.position - muzzle.position).normalized, out hit, range))
            {
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
                if (impact != null)
                {
                    GameObject cloneImpact = Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
                    cloneImpact.transform.parent = hit.transform;
                }
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForceAtPosition(force * (muzzleDirection.position - muzzle.position).normalized, hit.point);
                    CheckHit(hit.rigidbody);
                }
                
            }

            if (device != null)
                StartCoroutine(forceFeedback(device));
            chambered = false;
            StartCoroutine(kick());
            StartCoroutine(MuzzleFlash());
            nextFire = Time.time;
            PlaySound(0);
        }

    }

    IEnumerator MuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, muzzle);
            yield return new WaitForSeconds(0.05f);
            Destroy(muzzleFlash);
        }
        yield break;
    }

    void fireProj(SteamVR_Controller.Device device)     // Fires a projectile. Can be used for - well, anything
    {                                                   // Can be used for rockets, shotguns, energy weapons, etc
        if (Time.time - nextFire > fireRate)
        {
            GameObject proj = (GameObject)Instantiate(projectile, muzzle.position, Quaternion.Euler((muzzleDirection.position - muzzle.position).normalized));
            if (proj.GetComponent<Rigidbody>() != null)
            {
                proj.GetComponent<Rigidbody>().AddRelativeForce((muzzleDirection.position - muzzle.position) * projForce, ForceMode.Impulse);
            }
            if (device != null)
                StartCoroutine(forceFeedback(device));                    
            nextFire = Time.time;
            StartCoroutine(kick());
            PlaySound(0);
        }
    }

    IEnumerator fireAuto(SteamVR_Controller.Device device)
    {
        while (true)
        {
            if (chambered)
            {
                fireBullet(device);
                chambered = false;
                if (mag != null)
                {
                    if (mag.ammo > 0)
                        StartCoroutine(RackBack(true));
                    else
                        StartCoroutine(RackBack(false));
                }
                yield return new WaitForSeconds(fireRate);
            }
            if (!chambered)
                yield break;
        }
    }

    IEnumerator fireBurst(SteamVR_Controller.Device device)
    {
        for (int i = 0; i < burst; i++)
        {
            if (chambered)
            {
                fireBullet(device);
                chambered = false;
                if (mag.ammo > 0)
                    StartCoroutine(RackBack(true));
                else
                    StartCoroutine(RackBack(false));
                yield return new WaitForSeconds(fireRate);
            }
            if (!chambered)
                yield break;
        }
    }

    public void triggerPull(float angle)    // Trigger pull for VR weapons, accurately reflects trigger position
    {
        if (VREnabled == true)
        {
            if (trigger != null)
            {
                triggerRotation = new Vector3(triggerOrigin.x - (angle * triggerMult), triggerOrigin.y, triggerOrigin.z);
                trigger.transform.localEulerAngles = triggerRotation;
            }
        }
    }


    public void reload(magazine newMag)
    {
        if (newMag.weaponType.Equals(this.weaponType))
        {
            mag = newMag;           // Uses a new magazine script with a new ammo value, effectively reloading
            mag.transform.parent = this.transform;
            mag.transform.localPosition = magOPos;
            mag.transform.localEulerAngles = magORot;
            mag.transform.localScale = magOSca;
            if (mag.transform.GetComponent<Rigidbody>() != null)
                mag.transform.GetComponent<Rigidbody>().isKinematic = true;
            if (mag.transform.GetComponent<Collider>() != null)
                mag.transform.GetComponent<Collider>().enabled = false;
            if (rackedBack)         // Automatically chambers new round on reload
                StartCoroutine(RackForward());
            else if (!chambered)
                Chamber();
            PlaySound(1);
            isLoaded = true;
            mag.ableToUse = false;
        }
    }

    public void DropMag()
    {
        mag.dropMag();
        mag = null;
        PlaySound(2);
    }

    void PlaySound(int clip)
    {
        if (clip == 0)
        {
            sound.clip = shotSound;
            sound.Play();
        } else if (clip == 1)
        {
            sound.clip = magIn;
            sound.Play();
        } else if (clip == 2)
        {
            sound.clip = magOut;
            sound.Play();
        }

    }

    void CheckHit(Rigidbody hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            enemy target = hit.GetComponent<enemy>();
            target.OnHit(damage);
        }
        else if (hit.gameObject.layer == LayerMask.NameToLayer("Hittable"))
        {
            hitObject target = hit.GetComponent<hitObject>();
            target.OnHit(damage);
        }
        else
            return;
    }


    

    //------------------------------------
    //
    // Below here is reactionary functions - what happens to the weapon after firing
    //
    //------------------------------------


    IEnumerator RackBack(bool ret)          // This and RackForward are used for procedurally animated guns, currently VR only
    {
        if (VREnabled == true)
        {
            if (slide != null)
            {
                for (float i = 0; i < slideTime; i++)
                {
                    Vector3 slideComp = Vector3.forward * (slideLength / slideTime);
                    slide.transform.Translate(slideComp * 0.01f);
                    yield return new WaitForFixedUpdate();
                }
                
            }
            if (ret)
                StartCoroutine(RackForward());
            if (automaticChamber)
                eject();
            rackedBack = true;
        }
    }

    IEnumerator RackForward()
    {
        if (VREnabled == true)
        {
            if (slide != null)
            {
                for (float i = 0; i < slideTime; i++)
                {
                    Vector3 slideComp = Vector3.forward * (slideLength / slideTime);
                    slide.transform.Translate(-slideComp * 0.01f);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
    
        Chamber();
        rackedBack = false;

}

IEnumerator kick()
    {
        if (VREnabled == true)
        {
            float elapsed = 0;
            Vector3 weaponOrigin = transform.localPosition;
            Vector3 weaponRotOrigin = transform.localEulerAngles;
            transform.Rotate(rotBack, rotKickStrength);
            transform.Translate(kickBack * kickStrength);
            Vector3 pos = transform.localPosition;
            Vector3 rot = transform.localEulerAngles;
            while (elapsed <= recoverSpeed)
            {
                elapsed += Time.deltaTime;
                transform.localEulerAngles = Vector3.Lerp(rot, weaponRotOrigin, (elapsed / recoverSpeed));
                transform.localPosition = Vector3.Lerp(pos, weaponOrigin, (elapsed / recoverSpeed));
                yield return null;
            }
            transform.localPosition = weaponOrigin;
        }
    }

    IEnumerator forceFeedback(SteamVR_Controller.Device device)
    {
        if (device != null)
        {
            for (int i = 0; i < feedbackTime; i++)
            {
                device.TriggerHapticPulse(hapticStrength);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public void Chamber()
    {
        if (mag.ammo > 0)
        {
            chambered = true;
            mag.ammo--;
        }
    }

    void eject()
    {
        GameObject cloneShell = (GameObject)Instantiate(bulletShell, ejector.position, ejector.rotation, this.transform);
        cloneShell.GetComponent<Rigidbody>().AddForce((ejectorDirection.position - ejector.position).normalized * ejectForce, ForceMode.Impulse);
        cloneShell.transform.parent = null;
    }
}
