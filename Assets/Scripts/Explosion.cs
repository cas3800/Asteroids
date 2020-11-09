using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    float TTL = 0.5f;
    float aliveT = 0f;

    void Update()
    {
        aliveT += Time.deltaTime;
        if (aliveT > TTL) gameObject.SetActive(false);
    }

    void OnEnable()
    {
        aliveT = 0;    
    }
}
