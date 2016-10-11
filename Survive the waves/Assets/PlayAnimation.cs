using UnityEngine;
using System.Collections;

public class PlayAnimation : MonoBehaviour {

    Animator anim;

    public void DieAnimation()
    {
        anim = GetComponent<Animator>();
        anim.CrossFade("Die", 0f);
    }
}
