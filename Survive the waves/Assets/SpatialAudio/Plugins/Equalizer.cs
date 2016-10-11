using UnityEngine;
using System.Collections;

public class Equalizer
{

    float  lf;       // Frequency
    float  f1p0;     // Poles ...
    float  f1p1;
    float  f1p2;
    float  f1p3;
 
    // Filter #2 (High band)
 
    float  hf;       // Frequency
    float  f2p0;     // Poles ...
    float  f2p1;
    float  f2p2;
    float  f2p3;
 
    // Sample history buffer
 
    float  sdm1;     // Sample data minus 1
    float  sdm2;     //                   2
    float  sdm3;     //                   3
 
    // Gain Controls
 
    public float  lg;       // low  gain
    public float  mg;       // mid  gain
    public float  hg;       // high gain
 
    static float vsa = (1.0f / 4294967295.0f);   // Very small amount (Denormal Fix)

    public static Equalizer Default ()
    {
        return new Equalizer (880, 5000, 44100);
    }

    public Equalizer (int lowfreq, int highfreq, int mixfreq)
    {
        lg = 1.0f;
        mg = 1.0f;
        hg = 1.0f;
        // Calculate filter cutoff frequencies
        lf = 2 * Mathf.Sin (Mathf.PI * ((float)lowfreq / (float)mixfreq)); 
        hf = 2 * Mathf.Sin (Mathf.PI * ((float)highfreq / (float)mixfreq));
    }

    public float Process (float sample)
    {

  
        float l, m, h;      // Low / Mid / High - Sample Values
  
        // Filter #1 (lowpass)
  
        f1p0 += (lf * (sample - f1p0)) + vsa;
        f1p1 += (lf * (f1p0 - f1p1));
        f1p2 += (lf * (f1p1 - f1p2));
        f1p3 += (lf * (f1p2 - f1p3));
  
        l = f1p3;
  
        // Filter #2 (highpass)
  
        f2p0 += (hf * (sample - f2p0)) + vsa;
        f2p1 += (hf * (f2p0 - f2p1));
        f2p2 += (hf * (f2p1 - f2p2));
        f2p3 += (hf * (f2p2 - f2p3));
  
        h = sdm3 - f2p3;
  
        // Calculate midrange (signal - (low + high))
  
        m = sdm3 - (h + l);
  
        // Scale, Combine and store
  
        l *= lg;
        m *= mg;
        h *= hg;
  
        // Shuffle history buffer 
  
        sdm3 = sdm2;
        sdm2 = sdm1;
        sdm1 = sample;                
  
        // Return result
  
        return(l + m + h);
    }

}

 
 



