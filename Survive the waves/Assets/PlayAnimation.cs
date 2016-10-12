using UnityEngine;
using System.Collections;

public class PlayAnimation : MonoBehaviour {

    Animator anim;

    public void DieAnimation()
    {
        anim = GetComponent<Animator>();
        anim.CrossFade("Die", 0f);
    }

    public void AttackAnimation()
    {
        anim = GetComponent<Animator>();
        anim.CrossFade("Attack", 0f);
    }
}
