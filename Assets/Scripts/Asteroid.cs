using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private GameObject ast;
    public float speed;
    
    void Start()
    {
        ast = GameObject.Find("Asteroids");
    }

    void Update()
    {
        ast.GetComponent<Asteroids>().Teleport(gameObject);
        transform.Translate(transform.up*Time.deltaTime*speed, Space.World);        
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (gameObject.tag=="1") ast.SendMessage("AddScore", 20);
            if (gameObject.tag=="2") ast.SendMessage("AddScore", 50);
            if (gameObject.tag=="3") ast.SendMessage("AddScore", 100);
        }
        if ((gameObject.tag == "1") || (gameObject.tag == "2")) ast.SendMessage("Get2Asteroids", gameObject);
        ast.SendMessage("Explosion", gameObject);
        gameObject.SetActive(false);
    }
    
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")      
        {
            if (gameObject.tag=="1") ast.SendMessage("AddScore", 20);
            if (gameObject.tag=="2") ast.SendMessage("AddScore", 50);
            if (gameObject.tag=="3") ast.SendMessage("AddScore", 100);
        }
        ast.SendMessage("Explosion", gameObject);
        gameObject.SetActive(false);
    }
}
