using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed, odo;
    private bool isTeleport;
    private GameObject ast;

    void Start()
    {
        ast = GameObject.Find("Asteroids");
        speed = ast.GetComponent<Asteroids>().BulletSpeed;
        isTeleport = ast.GetComponent<Asteroids>().BulletTeleport;
    }

    void Update()
    {
        if (isTeleport) ast.GetComponent<Asteroids>().Teleport(gameObject);
        transform.Translate(transform.up*Time.deltaTime*speed, Space.World);
        float screenWidth = - Camera.main.ScreenToWorldPoint(new Vector3(0f,0f,0f)).x * 2;
        odo += Time.deltaTime*speed;
        if (odo > screenWidth) gameObject.SetActive(false);
    }

    void OnEnable()
    {
        odo = 0;
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ps.Stop();
        if (gameObject.tag == "Player") ps.startColor = Color.green;
            else ps.startColor = Color.red;
        ps.Play();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != gameObject.tag) gameObject.SetActive(false);
    }
}
