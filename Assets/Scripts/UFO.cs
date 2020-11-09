using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour
{
    private float speed, timeToNextShot, minT, maxT;
    private GameObject ast, ship;
    bool isTeleport;
    
    void Start()
    {
        ast = GameObject.Find("Asteroids");
        ship = GameObject.Find("Player");
        minT = ast.GetComponent<Asteroids>().ufoMinShotTimeout;
        maxT = ast.GetComponent<Asteroids>().ufoMaxShotTimeout;
        isTeleport = ast.GetComponent<Asteroids>().UFOTeleport;
    }

    void Update()
    {
        transform.Translate(transform.up*Time.deltaTime*speed, Space.World);
        if (isTeleport) ast.GetComponent<Asteroids>().Teleport(gameObject);
        else if (Mathf.Abs(transform.position.x) > -Camera.main.ScreenToWorldPoint(new Vector3(0f,0f,0f)).x)
            {
                gameObject.SetActive(false);
                ast.SendMessage("StartUFO");
            }

        timeToNextShot -= Time.deltaTime;
        if (timeToNextShot < 0)
        {
            GameObject bullet = ast.GetComponent<Asteroids>().GetAmmo(gameObject);
            bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, ship.transform.position - transform.position);
            timeToNextShot = Random.Range(minT, maxT);
        }
    }

    void OnEnable()
    {
        speed = (-Camera.main.ScreenToWorldPoint(new Vector3(0f,0f,0f)).x / 5);
        timeToNextShot = Random.Range(minT, maxT); 
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player") ast.SendMessage("AddScore", 200);      
        ast.SendMessage("Explosion", gameObject);
        gameObject.SetActive(false);
        ast.SendMessage("StartUFO");
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ast.SendMessage("AddScore", 200);
            ast.SendMessage("Explosion", gameObject);
            gameObject.SetActive(false);
            ast.SendMessage("StartUFO");
        }
    }
}
