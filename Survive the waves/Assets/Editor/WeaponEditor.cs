using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Weapon))]

public class WeaponEditor : Editor
{
    bool isCodeSlide, isCodeTrigger, showTransformAdjust, showSounds;
    public override void OnInspectorGUI()
    {
        Weapon weaponScript = (Weapon)target;
        //base.OnInspectorGUI();

        if (GUILayout.Button("Build Weapon"))
        {
            EditorWindow.GetWindow(typeof(WeaponBuild), false, "WeaponBuilder");

        }

        weaponScript.range = EditorGUILayout.FloatField("Range", weaponScript.range);
        weaponScript.damage = EditorGUILayout.IntField("Damage", weaponScript.damage);
        weaponScript.automaticChamber = EditorGUILayout.Toggle("Automatically chamber next round?", weaponScript.automaticChamber);

        if (weaponScript.automaticChamber)
        {
            weaponScript.fireRate = EditorGUILayout.FloatField("Fire rate", weaponScript.fireRate);
        }

        weaponScript.fireMode = (Weapon.FireMode)EditorGUILayout.EnumPopup("Fire mode", weaponScript.fireMode);

        if (weaponScript.fireMode == Weapon.FireMode.BurstFire)
        {
            weaponScript.burst = EditorGUILayout.IntField("Burst amount", weaponScript.burst);
        }

        else if (weaponScript.fireMode == Weapon.FireMode.Projectile)
        {
            weaponScript.projectile = (GameObject)EditorGUILayout.ObjectField("Projectile", weaponScript.projectile, typeof(GameObject), true);
            weaponScript.projForce = (float)EditorGUILayout.FloatField("Projectile shot force", weaponScript.projForce);
        }
        if (weaponScript.fireMode != Weapon.FireMode.Projectile)
        {
            weaponScript.impact = (GameObject)EditorGUILayout.ObjectField("Impact Object", weaponScript.impact, typeof(GameObject), true);
        }
        weaponScript.force = EditorGUILayout.FloatField("Impact force", weaponScript.force);
        isCodeSlide = EditorGUILayout.Toggle("Animate weapon slide by code?", isCodeSlide);

        if (isCodeSlide)
        {
            weaponScript.slide = (Transform)EditorGUILayout.ObjectField("Slide", weaponScript.slide, typeof(Transform), true);
            weaponScript.slideLength = EditorGUILayout.FloatField("Weapon slide length", weaponScript.slideLength);
            weaponScript.slideTime = EditorGUILayout.IntField("Time it takes for slide to move (in frames)", weaponScript.slideTime);
        }

        isCodeTrigger = EditorGUILayout.Toggle("Animate trigger by code?", isCodeTrigger);

        if (isCodeTrigger)
        {
            weaponScript.trigger = (Transform)EditorGUILayout.ObjectField("Trigger", weaponScript.trigger, typeof(Transform), true);
            weaponScript.triggerMult = EditorGUILayout.IntField("Trigger rotation multiplier", weaponScript.triggerMult);
        }

        showTransformAdjust = EditorGUILayout.Toggle("Show weapon position adjustments", showTransformAdjust);

        if (showTransformAdjust)
        {
            weaponScript.adjustForward = EditorGUILayout.Slider("Forward adjust", weaponScript.adjustForward, -1, 1);
            weaponScript.adjustRotation = EditorGUILayout.Vector3Field("Rotational adjust", weaponScript.adjustRotation);
        }

        weaponScript.feedbackTime = EditorGUILayout.IntField("Haptic time per shot (in frames)", weaponScript.feedbackTime);
        showSounds = EditorGUILayout.Toggle("Show audio options", showSounds);

        if (showSounds)
        {
            weaponScript.shotSound = (AudioClip)EditorGUILayout.ObjectField("Shot sound", weaponScript.shotSound, typeof(AudioClip), true);
            weaponScript.magIn = (AudioClip)EditorGUILayout.ObjectField("Magazine inserted", weaponScript.magIn, typeof(AudioClip), true);
            weaponScript.magOut = (AudioClip)EditorGUILayout.ObjectField("Magazine removed", weaponScript.magOut, typeof(AudioClip), true);

        }

    }

}

public class WeaponBuild : EditorWindow
{
    ReloadPoint rld;
    GameObject dir, orig, ejDir, ejOrig, kiDir, kiOrig, magAssign, rotDir;
    int index = 0;
    string prefabName;
    void OnGUI()
    {
        Weapon[] tmp = FindObjectsOfType<Weapon>();
        int count = 0;
        foreach (Weapon wpn in tmp)
            count++;
        string[] names = new string[count];
        int i = 0;
        foreach (Weapon wpn in tmp)
        {
            names[i] = wpn.name;
            i++;
        }

        index = EditorGUILayout.Popup("Weapon: ", index, names);
        Weapon weaponScript = tmp[index];

        EditorGUILayout.LabelField("Tags \"ReloadPoint\" and \"Magazine\", and layer \"weapon\" must be");
        EditorGUILayout.LabelField("added manually through the editor.");

        if (weaponScript.muzzleDirectionSet == false)
        {
            if (GUILayout.Button("Muzzle direction"))
            {
                if (orig == null)
                {
                    orig = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    orig.name = "Muzzle origin";
                    orig.transform.parent = weaponScript.transform;
                    orig.transform.localPosition = new Vector3(0, 0, 0);
                    orig.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                    var tempMaterial = new Material(orig.GetComponent<Renderer>().sharedMaterial);
                    tempMaterial.color = new Vector4(1, 0, 0, 0.5f);
                    orig.gameObject.GetComponent<Renderer>().sharedMaterial = tempMaterial;
                    dir = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    dir.name = "Muzzle direction";
                    dir.transform.parent = orig.transform;
                    dir.transform.localPosition = new Vector3(0, 0, 0);
                    dir.transform.localScale = new Vector3(1, 1, 1);
                    dir.gameObject.GetComponent<Renderer>().sharedMaterial = tempMaterial;
                }
                weaponScript.showMuzDirectionSetup = true;
                Selection.activeGameObject = orig.gameObject;

            }
        }
        if (weaponScript.showMuzDirectionSetup)
        {
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Move the RED CUBE to muzzle origin location.");
            EditorGUILayout.LabelField("Move the RED SPHERE to muzzle shot direction.");
            weaponScript.muzzleFlash = (GameObject)EditorGUILayout.ObjectField("Muzzle flash", weaponScript.muzzleFlash, typeof(GameObject), true);
            if (GUILayout.Button("Save muzzle direction"))
            {
                weaponScript.muzzleDirection = dir.transform;
                weaponScript.muzzle = orig.transform;
                orig.SetActive(false);
                dir.SetActive(false);
                weaponScript.showMuzDirectionSetup = false;
                weaponScript.muzzleDirectionSet = true;
                orig = null;
                dir = null;

                //////////// Set up layers and tags (Assuming everyone will use the muzzle direction, so safe to assume this will cover all weapons) /////////////
         
                weaponScript.gameObject.layer = LayerMask.NameToLayer("Weapon");
                foreach (Transform t in weaponScript.GetComponentsInChildren<Transform>())
                {
                    t.gameObject.layer = LayerMask.NameToLayer("Weapon");
                }

            }
            if (GUILayout.Button("Cancel"))
            {
                DestroyImmediate(orig.gameObject);
                DestroyImmediate(dir.gameObject);
                weaponScript.showMuzDirectionSetup = false;
                orig = null;
                dir = null;
            }
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        }

        if (weaponScript.muzzleDirectionSet == true)
        {
            if (GUILayout.Button("Reset muzzle direction"))
            {
                if (weaponScript.gameObject.transform.FindChild("Muzzle origin") != null)
                {
                    DestroyImmediate(weaponScript.gameObject.transform.FindChild("Muzzle origin").gameObject);
                    DestroyImmediate(weaponScript.gameObject.transform.FindChild("Muzzle direction").gameObject);
                }
                weaponScript.muzzleDirectionSet = false;
                weaponScript.muzzleDirection = null;
            }
        }
        if (weaponScript.ejectorDirectionSet == false)
        {
            if (GUILayout.Button("Add spent shell ejector"))
            {
                if (ejOrig == null)
                {
                    ejOrig = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    ejOrig.name = "Ejector origin";
                    ejOrig.transform.parent = weaponScript.transform;
                    ejOrig.transform.localPosition = new Vector3(0, 0, 0);
                    ejOrig.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                    var tempMaterial = new Material(ejOrig.GetComponent<Renderer>().sharedMaterial);
                    tempMaterial.color = new Vector4(1, 0.92f, 0.016f, 0.3f);
                    ejOrig.gameObject.GetComponent<Renderer>().sharedMaterial = tempMaterial;
                    ejDir = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    ejDir.name = "Ejector direction";
                    ejDir.transform.parent = ejOrig.transform;
                    ejDir.transform.localPosition = new Vector3(0, 0, 0);
                    ejDir.transform.localScale = new Vector3(1, 1, 1);
                    ejDir.gameObject.GetComponent<Renderer>().sharedMaterial = tempMaterial;
                }
                weaponScript.showEjectDirectionSetup = true;
                Selection.activeGameObject = ejOrig.gameObject;
            }
        }
        if (weaponScript.showEjectDirectionSetup)
        {
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Move the YELLOW CUBE to spent shell origin location.");
            EditorGUILayout.LabelField("Move the YELLOW SPHERE to spent shell eject direction.");
            weaponScript.ejectForce = EditorGUILayout.FloatField("Shell eject force", weaponScript.ejectForce);
            weaponScript.bulletShell = (GameObject)EditorGUILayout.ObjectField("Bullet Shell: ", weaponScript.bulletShell, typeof(GameObject), true);
            if (GUILayout.Button("Save shell eject direction"))
            {
                weaponScript.ejector = ejOrig.transform;
                weaponScript.ejectorDirection = ejDir.transform;
                ejOrig.SetActive(false);
                ejDir.SetActive(false);
                weaponScript.showEjectDirectionSetup = false;
                weaponScript.ejectorDirectionSet = true;
                ejOrig = null;
                ejDir = null;
            }
            if (GUILayout.Button("Cancel"))
            {
                DestroyImmediate(ejOrig.gameObject);
                DestroyImmediate(ejDir.gameObject);
                weaponScript.showEjectDirectionSetup = false;
                ejOrig = null;
                ejDir = null;
            }
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        }

        if (weaponScript.ejectorDirectionSet == true)
        {
            if (GUILayout.Button("Reset shell eject direction"))
            {
                weaponScript.ejectorDirectionSet = false;
                weaponScript.ejectorDirection = null;
                weaponScript.ejector = null;
                DestroyImmediate(weaponScript.transform.FindChild("Ejector origin").gameObject);
                DestroyImmediate(weaponScript.transform.FindChild("Ejector direction").gameObject);
            }
        }

        if (!weaponScript.kickSet)
        {
            if (GUILayout.Button("Set up kick direction"))
            {
                if (kiOrig == null)
                {
                    kiOrig = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    kiOrig.name = "Kick origin";
                    kiOrig.transform.parent = weaponScript.transform;
                    kiOrig.transform.localPosition = new Vector3(0, 0, 0);
                    kiOrig.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                    var tempMaterial = new Material(kiOrig.GetComponent<Renderer>().sharedMaterial);
                    tempMaterial.color = new Vector4(0, 1, 0, 0.3f);
                    kiOrig.gameObject.GetComponent<Renderer>().sharedMaterial = tempMaterial;
                    kiDir = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    kiDir.name = "Kick direction";
                    kiDir.transform.parent = kiOrig.transform;
                    kiDir.transform.localPosition = new Vector3(0, 0, 0);
                    kiDir.transform.localScale = new Vector3(1, 1, 1);
                    kiDir.gameObject.GetComponent<Renderer>().sharedMaterial = tempMaterial;
                    rotDir = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    rotDir.name = "Rotation axis";
                    rotDir.transform.parent = kiOrig.transform;
                    rotDir.transform.localPosition = new Vector3(0, 0, 0);
                    rotDir.transform.localScale = new Vector3(1, 1, 1);
                    var tempMaterial2 = new Material(rotDir.GetComponent<Renderer>().sharedMaterial);
                    tempMaterial2.color = new Vector4(1, 0, 1, 0.3f);
                    rotDir.gameObject.GetComponent<Renderer>().sharedMaterial = tempMaterial2;
                }
                weaponScript.showKickSetup = true;
                Selection.activeGameObject = kiDir.gameObject;
            }
        }
        if (weaponScript.showKickSetup)
        {
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Leave the GREEN CUBE in place");
            EditorGUILayout.LabelField("Move the GREEN SPHERE to the kick direction");
            EditorGUILayout.LabelField("Move the MAGENTA SPHERE to the axis of rotation");
            weaponScript.kickStrength = EditorGUILayout.Slider("Kick strength", weaponScript.kickStrength, 0, 10);
            weaponScript.rotKickStrength = EditorGUILayout.Slider("Rotational kick strength", weaponScript.rotKickStrength, 0, 60);
            weaponScript.recoverSpeed = EditorGUILayout.Slider("Kick recovery speed", weaponScript.recoverSpeed, 0.1f, 2);
            if (GUILayout.Button("Save kick direction"))
            {
                weaponScript.kickBack = (kiDir.transform.position - kiOrig.transform.position).normalized;
                weaponScript.rotBack = -(rotDir.transform.position - kiOrig.transform.position).normalized;

                DestroyImmediate(kiOrig.gameObject);
                DestroyImmediate(kiDir.gameObject);
                DestroyImmediate(rotDir.gameObject);
                weaponScript.showKickSetup = false;
                weaponScript.kickSet = true;
                kiOrig = null;
                kiDir = null;
                rotDir = null;
            }
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        }
        if (weaponScript.kickSet)
        {
            if (GUILayout.Button("Reset kick settings"))
            {
                weaponScript.kickSet = false;
                weaponScript.kickStrength = 0;
                weaponScript.rotKickStrength = 0;
                weaponScript.recoverSpeed = 0;
                weaponScript.kickBack = new Vector3(0, 0, 0);
                weaponScript.rotBack = new Vector3(0, 0, 0);
            }
        }
        
        if (weaponScript.rldPointSet == false)
        {
            if (weaponScript.showRldPointSetup == false)
            {
                if (GUILayout.Button("Add reload point"))
                {
                    if (weaponScript.gameObject.GetComponentInChildren<ReloadPoint>() == null)
                    {
                        rld = GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<ReloadPoint>();
                        rld.transform.parent = weaponScript.gameObject.transform;
                        rld.name = "Reload Point";
                        rld.tag = "ReloadPoint";
                        rld.gameObject.layer = LayerMask.NameToLayer("Weapon");
                        rld.gameObject.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                        var tempMaterial = new Material(rld.GetComponent<Renderer>().sharedMaterial);
                        tempMaterial.color = new Vector4(0, 1, 1, 0.3f);

                        rld.gameObject.GetComponent<Renderer>().sharedMaterial = tempMaterial;
                        rld.transform.localPosition = new Vector3(0, 0, 0);
                        rld.transform.localEulerAngles = new Vector3(0, 0, 0);
                        rld.gameObject.GetComponent<SphereCollider>().isTrigger = true;
                        rld.weap = weaponScript;
                    }
                    else
                    {
                        rld = weaponScript.gameObject.GetComponentInChildren<ReloadPoint>();
                    }
                    Selection.activeGameObject = rld.gameObject;
                    weaponScript.showRldPointSetup = true;
                }
            }
            if (weaponScript.showRldPointSetup == true)
            {
                EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Move and resize the BLUE SPHERE to the magazine reload point");
                if (GUILayout.Button("Save reload point"))
                {
                    weaponScript.showRldPointSetup = false;
                    weaponScript.rldPointSet = true;
                    DestroyImmediate(rld.GetComponent<Renderer>());
                }
                EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
            }
        }
        if (weaponScript.rldPointSet == true)
        {
            if (GUILayout.Button("Reset reload point"))
            {
                if (weaponScript.GetComponentInChildren<ReloadPoint>() != null)
                {
                    rld = weaponScript.GetComponentInChildren<ReloadPoint>();
                    DestroyImmediate(rld.gameObject);
                }
                
                weaponScript.rldPointSet = false;
                rld = null;
            }
        }
        if (GUILayout.Button("Pair magazine type to weapon"))
        {
            weaponScript.showPairMag = true;
        }
        if (weaponScript.showPairMag)
        {
            if (weaponScript.mag != null)
            {
                EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Align magazine to desired transform values");
                weaponScript.mag = (magazine)EditorGUILayout.ObjectField("Magazine: ", weaponScript.mag, typeof(magazine), true);

                weaponScript.mag.ammo = EditorGUILayout.IntField("Ammo capacity: ", weaponScript.mag.ammo);
            }
            else
            {
                magAssign = (GameObject)EditorGUILayout.ObjectField("Magazine: ", magAssign, typeof(GameObject), true);
            }
            if (magAssign != null)
            {
                if (weaponScript.mag != magAssign)
                {
                    if (magAssign.GetComponent<magazine>() == null)
                        weaponScript.mag = magAssign.AddComponent<magazine>();
                    else
                        weaponScript.mag = magAssign.GetComponent<magazine>();
                }
            }
            if (GUILayout.Button("Pair"))
            {
                if (weaponScript.mag != null)
                {
                    if (weaponScript.weaponType != weaponScript.mag.weaponType)
                    {
                        i = 0;
                        foreach (Weapon wpn in tmp)
                        {
                            while (i == wpn.weaponType)
                            {
                                i++;
                            }
                        }
                        weaponScript.weaponType = i;
                        weaponScript.mag.weaponType = i;
                    }
                    Vector3 magOPos = weaponScript.mag.transform.localPosition;
                    Vector3 magORot = weaponScript.mag.transform.localEulerAngles;
                    Vector3 magOSca = weaponScript.mag.transform.localScale;

                    weaponScript.magOPos = magOPos;
                    weaponScript.magORot = magORot;
                    weaponScript.magOSca = magOSca;
                    weaponScript.showPairMag = false;
                    weaponScript.mag.tag = "Magazine";

                    ///////////////// Ensuring that magazines are able to be picked up /////////////////////

                    if (weaponScript.mag.GetComponent<Collider>() == null)
                    {
                        weaponScript.mag.gameObject.AddComponent<BoxCollider>();
                    }
                    if (weaponScript.mag.GetComponent<Rigidbody>() == null)
                    {
                        weaponScript.mag.gameObject.AddComponent<Rigidbody>();
                    }
                    if (weaponScript.GetComponent<Collider>() == null)
                    {
                        foreach (Collider col in weaponScript.GetComponentsInChildren<Collider>())
                        {
                            if (col.gameObject.tag != "ReloadPoint" && col.gameObject.tag != "Magazine")
                                if (col.GetComponent<Collider>() != null)
                                    DestroyImmediate(col.GetComponent<Collider>());
                        }
                        weaponScript.gameObject.AddComponent<BoxCollider>();
                        if (weaponScript.gameObject.GetComponent<Mesh>() == null)       // If it has a mesh, the box collider should accurately adjust. Only make smaller if no mesh
                            weaponScript.gameObject.GetComponent<BoxCollider>().transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Add a GameObject as the weapon's magazine");

                }

             



            }
            if (GUILayout.Button("Cancel"))
            {
                weaponScript.showPairMag = false;
            }
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        }
        if (GUILayout.Button("Save weapon as prefab"))
        {
            weaponScript.showPrefabSave = true;
        }
        if (weaponScript.showPrefabSave == true)
        {
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
            prefabName = EditorGUILayout.TextField("Prefab name:", prefabName);

            if (GUILayout.Button("Save") && prefabName != "")
            {
                if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                    AssetDatabase.CreateFolder("Assets", "Prefabs");
                PrefabUtility.CreatePrefab("Assets/Prefabs/" + prefabName + ".prefab", weaponScript.gameObject);
                PrefabUtility.CreatePrefab("Assets/Prefabs/" + prefabName + "_magazine.prefab", weaponScript.mag.gameObject);
                weaponScript.showPrefabSave = false;
                prefabName = "";
            }
            if (GUILayout.Button("Cancel"))
            {
                weaponScript.showPrefabSave = false;
                prefabName = "";
            }
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        }
    }
}
