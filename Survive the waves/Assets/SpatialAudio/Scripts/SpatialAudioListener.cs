using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(AudioListener))]
public class SpatialAudioListener : MonoBehaviour {
	[Tooltip("Show or Hide Gizmos in the editor")]
	public bool showGizmos = true;

	[Tooltip("Enable or disable audio occlusion testing")]
	public bool enableAudioOcclusion = true;
    
	[Tooltip("The arc in front and behind the player in which phase offset is interpolated. Outside this range, audio will be 100% biased to the left or right. It is represented by the green arc in the scene view.")]
    [Range(1,179)]
    public int phaseWidth = 140;
    
    [Tooltip("Forward of this angle, all audio is considered to be in front of the player. Behind this angle, audio starts to sound from the rear. It is represented by the blue arc in the scene view.")]
    [Range(1,90)]
    public int fadeStartAngle = 83;
    
    [Tooltip("Behind this angle, all audio is considered to be in 100% behind the player. Forward of this angle, audio starts to sound from the front. It is represented by the blue arc in the scene view.")]
    [Range(91,179)]
    public int fadeEndAngle = 136;
    
    [Tooltip("The max reverb signal to mix with the audio when behind the fadeEndAngle")]
    [Range(0,0.5f)]
    public float maxFadeMix = 0.048f;
    
    [Tooltip("The reverb decay time.")]
    public float decayTime = 0.62f;
    
    [Tooltip("The interaural delay (time for sound to travel across two ears) in milliseconds. Most humans heads are 1 ms")]
    public int interauralDelay = 1;
    
    [Tooltip("How much the sound should be damped when an object is between the source and the player.")]
    [Range(0,1)]
    public float occlusionDamping = 0.5f;
    
    [Tooltip("The EQ settings for sounds behind the player.")]
    public float[] rearEq = new float[] { 1.25f, 0.75f, 0.5f };
    
    [Tooltip("The EQ settings for occluded sounds.")]
    public float[] occlusionEq = new float[] { 1f, 0.5f, 0.125f };

    void OnDrawGizmosSelected() {
#if UNITY_EDITOR
		if(showGizmos) {
	        var center = transform.position;
	        Handles.color = new Color(0,1,0,0.15f);
	        
	        Handles.DrawSolidArc(center, transform.up, transform.forward, -(phaseWidth/2), 3);
	        Handles.DrawSolidArc(center, transform.up, transform.forward, +(phaseWidth/2), 3);
	        Handles.DrawSolidArc(center, transform.up, -transform.forward, -(phaseWidth/2), 3);
	        Handles.DrawSolidArc(center, transform.up, -transform.forward, +(phaseWidth/2), 3);
	        
	        Handles.color = new Color(0,0,1,0.15f);
	        Handles.DrawSolidArc(center, transform.up, transform.right, -(90-fadeStartAngle), 3);
	        Handles.DrawSolidArc(center, transform.up, transform.right, (fadeEndAngle-90), 3);
	        Handles.DrawSolidArc(center, transform.up, -transform.right, (90-fadeStartAngle), 3);
	        Handles.DrawSolidArc(center, transform.up, -transform.right, -(fadeEndAngle-90), 3);
		}
#endif
    }

}
