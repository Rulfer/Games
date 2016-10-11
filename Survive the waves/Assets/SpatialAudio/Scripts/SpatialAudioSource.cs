using UnityEngine;
using System.Collections;


[RequireComponent(typeof(AudioSource))]
public class SpatialAudioSource : MonoBehaviour
{

    SpatialAudioListener sal;

    bool occluded = false;
    float fadeMix = 0.1f;
    float allpassCoeff = 0.7f;
    float comb1Coeff;
    float comb2Coeff;
    int[] reverbDelay = {341, 613, 1557, 2137};
    float balance = 0.5f;
    float fade = 0.5f;
    bool inFront, isLeft;
    Vector3 delta;
    float leftRightAngle, frontRearAngle, relativeAngle;
    Transform listener;
    int sampleRate;
    float gain = 1f;

    VariableDelayLine left, right;
    DelayLine allpass1;
    DelayLine allpass2;
    DelayLine comb1;
    DelayLine comb2;

	Equalizer eL, eR;

    int phaseLeftAngle = 160;
    int phaseRightAngle = 20;

    Transform Listener {
        get {
            if(listener == null)        
                listener = GameObject.FindObjectOfType<AudioListener> ().transform;
            return listener;
        }
    }

    void Awake ()
    {
        sal = Listener.GetComponent<SpatialAudioListener> ();

        phaseLeftAngle = 90 + (sal.phaseWidth / 2);
        phaseRightAngle = 90 - (sal.phaseWidth / 2);
        listener = GameObject.FindObjectOfType<AudioListener> ().transform;       
        sampleRate = AudioSettings.outputSampleRate;
        var samplesPerMillisecond = 88;
        if (sampleRate != 44100) {
            var scaler = sampleRate / 44100.0f;
            samplesPerMillisecond = Mathf.FloorToInt (scaler * samplesPerMillisecond);
            for (var i = 0; i < reverbDelay.Length; i++) {
                var delay = Mathf.FloorToInt (scaler * reverbDelay [i]);
                if ((delay & 1) == 0) {
                    delay++;
                }
                while (!IsPrime(delay))
                    delay += 2;
                reverbDelay [i] = delay;
            }
        }
        allpass1 = new DelayLine (reverbDelay [0]);
        allpass2 = new DelayLine (reverbDelay [1]);
        comb1 = new DelayLine (reverbDelay [2]);
        comb2 = new DelayLine (reverbDelay [3]);
        var combScale = -3.0f / (sal.decayTime * sampleRate);
        comb1Coeff = Mathf.Pow (10.0f, combScale * comb1.Length);
        comb2Coeff = Mathf.Pow (10.0f, combScale * comb2.Length);
        left = new VariableDelayLine (samplesPerMillisecond*sal.interauralDelay);
        right = new VariableDelayLine (samplesPerMillisecond*sal.interauralDelay);
		eL = Equalizer.Default();
        eR = Equalizer.Default();
       
    }

    bool IsPrime (int N)
    {
        if ((N & 1) == 1) {
            var upto = (int)Mathf.Sqrt (N);
            for (var i = 3; i <= upto; i += 2) {
                if (N % i == 0)
                    return false;
            }
            return true;
        } else {
            return (N == 2);
        }
    }

    void Update ()
    {
        delta = listener.InverseTransformDirection (transform.position);
        inFront = delta.z > 0;
        isLeft = delta.x < 0;
        relativeAngle = Mathf.Abs (Mathf.Atan (delta.z / delta.x) * Mathf.Rad2Deg);
        if (isLeft)
            leftRightAngle = 90 + (90 - relativeAngle);
        else
            leftRightAngle = relativeAngle;
        if (inFront)
            frontRearAngle = 90 - relativeAngle;
        else
            frontRearAngle = 90 + relativeAngle;
        
		balance = Mathf.InverseLerp (phaseLeftAngle, phaseRightAngle, leftRightAngle);
        fade = Mathf.InverseLerp (sal.fadeStartAngle, sal.fadeEndAngle, frontRearAngle);

		left.SetDelay(Mathf.InverseLerp (0f, 0.5f, balance));
		right.SetDelay(Mathf.InverseLerp (1f, 0.5f, balance));

        fadeMix = Mathf.Lerp (0, sal.maxFadeMix, fade);

        RaycastHit hit;
        occluded = false;

		if(sal.enableAudioOcclusion) {
	        if(Physics.Linecast(transform.position, listener.position, out hit)) {
	            occluded = !hit.transform.CompareTag("Player");
	        }
		}

        if(occluded) {
            gain = Mathf.Clamp01(Mathf.Lerp(gain, sal.occlusionDamping, Time.deltaTime*10));
            eR.lg = eL.lg = sal.occlusionEq[0];
            eR.mg = eL.mg = sal.occlusionEq[1];
            eR.hg = eL.hg = sal.occlusionEq[2];
        } else {
            gain = Mathf.Clamp01(Mathf.Lerp(gain, 1, Time.deltaTime*10));
            eR.lg = eL.lg = Mathf.Lerp(1f, sal.rearEq[0], fade);
            eR.mg = eL.mg = Mathf.Lerp(1f, sal.rearEq[1], fade);
            eR.hg = eL.hg = Mathf.Lerp(1f, sal.rearEq[2], fade);
        }

    }

    void OnAudioFilterRead (float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i+=2) {
            var smpL = data [i] * gain;
            var smpR = data [i + 1] * gain;

            smpL = left.Tick (smpL);
            smpR = right.Tick (smpR);

            var input = 0.5f * (smpL + smpR);
            var temp0 = allpassCoeff * allpass1.Last;
            temp0 += input;
            temp0 = allpass1.Tick (temp0) - allpassCoeff * temp0;
            var temp1 = allpassCoeff * allpass2.Last;
            temp1 += temp0;
            temp1 = allpass2.Tick (temp1) - allpassCoeff * temp1;
            var out1 = comb1.Tick (temp1 + comb1Coeff * comb1.Last);
            var out2 = comb2.Tick (temp1 + comb2Coeff * comb2.Last);
            out1 = fadeMix * out1 + (1.0f - fadeMix) * smpL;
            out2 = fadeMix * out2 + (1.0f - fadeMix) * smpR;                
            smpL = out1;
            smpR = out2;

            smpL = eL.Process(smpL);
            smpR = eR.Process(smpR);

            data [i] = smpL<-1?-1:smpL>1?1:smpL;
			data [i + 1] = smpR<-1?-1:smpR>1?1:smpR;
        }
    }


}




