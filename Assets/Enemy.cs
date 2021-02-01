using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public MainScript mainScript;
    Vector3 moveDirection;
    bool move = false;
    float[] xPositions = new float[] {-11.3f, -7f, -2.3f, 2.3f, 7f, 11.3f};

    void Start() {      Replacement();          }
    
    void OnCollisionEnter(Collision collision)
    {
        // Determine which object collide
        if (collision.gameObject.tag == "jeep")
        {
            collision.gameObject.SetActive(false);
            // Start explosion animation
            mainScript.Explosion(collision.contacts[0].point);
            // Replace car and use again
            Replacement();
        }
        if (collision.gameObject.tag == "limo")
        {
            mainScript.LimoCrash();
            mainScript.Explosion(collision.contacts[0].point);
            Replacement();
        }
    }

    void Replacement()
    {
        move = false;
        int tag = int.Parse(gameObject.tag);
        // if car come from across
        if(tag < 4)
        { 
            transform.position = new Vector3(xPositions[Random.Range(0, 6)], 0, Random.Range(300f, 320f));
            transform.Translate(Vector3.forward * 300f, Space.World);
            moveDirection = Vector3.back * Random.Range(.8f, 1.2f);
            Invoke("StartMove", Random.Range(1f, 3f));
        }
        // comes from right
        else if(tag == 4)
        { 
            transform.position = new Vector3(50f, 0, 60f);
            moveDirection = Vector3.Normalize(new Vector3(Random.Range(-100f, -20f), 0, -60f)) * .5f;
            Invoke("StartMove", Random.Range(4f, 9f));
        }
        // comes from left
        else
        { 
            transform.position = new Vector3(-50f, 0, 60f);
            moveDirection = Vector3.Normalize(new Vector3(Random.Range(20f, 100f), 0, -60f)) * .5f;
            Invoke("StartMove", Random.Range(4f, 9f));
        }
    }

    void Update()
    {
        // Move the car
        if(move) transform.Translate(moveDirection * 50f * mainScript.gameSpeed * Time.deltaTime, Space.World);
        // if the car drop behind, replace and move again
        if(transform.position.z < -30f) Replacement();
    }

    // Start to move the car
    void StartMove() {          move = true;          }
}
