using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    public float timeToDestroy = 0.8f;
    private void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }
}
