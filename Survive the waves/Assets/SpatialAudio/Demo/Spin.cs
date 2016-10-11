using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {
	public Vector3 axis = Vector3.up;
	public float speed = 20;
    public SpatialAudioSource spatialAudio;
	
    void Update () {
		transform.Rotate(axis, speed * Time.deltaTime);
	}

    void OnGUI() {
        spatialAudio.enabled = GUILayout.Toggle(spatialAudio.enabled, "Enable Spatial Audio Filter");
    }
}
