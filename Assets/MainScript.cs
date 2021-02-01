using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainScript : MonoBehaviour
{
    [SerializeField] float remainingTime;
    public GameObject explosion, endPanel, medal;
    public Animator heartAnimator, rampAnimator;
    public Text timeText, heartText, finalText;
    public Camera cam;
    public float gameSpeed = 1f;
    public int limoHealth = 1;
    GameObject[] roadPieces = new GameObject[4];
    Animator animator, explosionAnimator;
    float startPositionX, endPositionX;
    int carPosition = 4, mouseStage = 0;  //     1: get start position   2:get end position      3: slide        4: wait for ending movement
    
    void Start()
    {
        // Road pieces assign
        roadPieces = GameObject.FindGameObjectsWithTag("road");
        // Animators assign
        animator = GetComponent<Animator>();
        explosionAnimator = explosion.GetComponent<Animator>();
        // Change slope of the road
        Invoke("ChangeRamp", Random.Range(10f, 20f));
    }

    // Changing the road's slope
    void ChangeRamp()
    {
        // -1: downhill     0: flat         1: climb  
        int randomNumber = Random.Range(-1, 2);
        // Rotation and position of camera change with animation
        rampAnimator.SetInteger("ramp", randomNumber);
        // Downhill faster, climbing slower
        gameSpeed = 1f + randomNumber * -.1f;
        // Changes of slope repeat
        Invoke("ChangeRamp", Random.Range(10f, 15f));
    }

    void Update()
    {
        // if you didn't lose during the time, you win
        remainingTime -= Time.deltaTime;
        timeText.text = ((int)remainingTime).ToString();
        if(remainingTime < 0) Win();
        //Sliding
        if (Input.GetButtonDown("Fire1") && mouseStage == 0)
        {
            startPositionX = Input.mousePosition.x;
            mouseStage = 1;
        }
        if (Input.GetButtonUp("Fire1") && mouseStage == 1)
        {
            endPositionX = Input.mousePosition.x;
            mouseStage = 2;
        }
        // Decide the slide's direction
        if(mouseStage == 2)
        {
            if(Mathf.Sign(startPositionX - endPositionX) == 1)
            {
                if(carPosition > 1) MoveMotorcade(-1);
                // if the motorcade is most left, cannot move
                else mouseStage = 0;
            }
            else
            {
                if(carPosition < 6) MoveMotorcade(1);
                else mouseStage = 0;
            }
        }
        // Move road
        foreach (GameObject roadPiece in roadPieces)
        {
            roadPiece.transform.Translate(Vector3.back * gameSpeed * 30f * Time.deltaTime);
            if(roadPiece.transform.position.z < -100f) roadPiece.transform.Translate(Vector3.forward * 400f);
        }   
    }

    // Move motorcade to left or right with animation
    void MoveMotorcade(int sign)
    {
        carPosition += sign;
        animator.SetInteger("lane", carPosition);
        mouseStage = 3;
        // Wait for finish animation
        Invoke("WaitEndingMovement", .5f);
    }

    // Make usable the sliding
    void WaitEndingMovement() {     mouseStage = 0;         }

    // if limo exploded
    public void LoseGame()
    {
        EndGame();
        finalText.text = "LOSE";
    }

    // if time finished
    void Win()
    {
        EndGame();
        finalText.text = "SUCCESS";
        // Give success medal
        medal.SetActive(true);
    }

    // Game ends 
    void EndGame()
    { 
        CancelInvoke();
        timeText.gameObject.SetActive(false);
        // Make not usable the sliding
        mouseStage = 3;
        // Stop every object
        gameSpeed = 0;
        // End panel opens
        endPanel.SetActive(true);
    }

    // if collide limo and enemies' car
    public void LimoCrash()
    {
        // not finish game, descrete heart
        if(limoHealth > 0)
        { 
            limoHealth--;
            heartAnimator.Play("heartShake");
            heartText.text = limoHealth.ToString();
        }
        // lose game
        else LoseGame();
    }

    // Explosion animation
    public void Explosion(Vector3 newPosition)
    {
        explosion.transform.position = cam.WorldToScreenPoint(newPosition);
        explosionAnimator.Play("explosion", -1, 0f);
    }

    // I use scene loading, but there are too much object in the scene we can restart without reloading
    public void ButtonAgain() {         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);       }
}
