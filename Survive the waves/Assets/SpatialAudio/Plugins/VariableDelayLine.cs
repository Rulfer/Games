using UnityEngine;
using System.Collections;

public class VariableDelayLine
{

    int N;
    float[] A;
    int rptr = 0;
    int wptr = 0;
    int reDelay = -1;

    public VariableDelayLine (int N)
    {
        this.N = N;
        A = new float[N];
    }

    public void SetDelay (float P)
    {
        reDelay = Mathf.Clamp (Mathf.FloorToInt (P * N), 0, N - 1);
    }
    
    public float Tick (float x)
    {
        if (reDelay > 0) {
            rptr = wptr - reDelay;
            while (rptr < 0) {
                rptr += N;
            }
            reDelay = -1;
        }

        float y = 0;
        A [wptr++] = x; 
        rptr++;
        while (rptr >= N) {
            rptr -= N;
        }

        y = A [rptr];

        while ((wptr) >= N) {
            wptr -= N;
        }
        return y;
    }
}
