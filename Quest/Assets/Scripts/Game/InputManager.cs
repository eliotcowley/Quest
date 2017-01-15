using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static bool usingJoyStick;

    //First establish some variables
    private Vector3 fp; //First finger position
    private Vector3 lp; //Last finger position

    [SerializeField]
    private float dragDistance;  //Distance needed for a swipe to register

    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private PlayerAttack playerAttack;

    [SerializeField]
    private Transform pauseButton;

    [SerializeField]
    private float distanceFromPauseButton = 10f;

    private bool canUseAxis = true;
    private string dpadY = "DpadY";

    private void Awake()
    {
        usingJoyStick = (Input.GetJoystickNames().Length > 0);
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG 
        if (Input.GetMouseButtonDown(0))
        {
            float mouseToPauseButtonDistance = Vector2.Distance(Input.mousePosition, pauseButton.position);
            if ((mouseToPauseButtonDistance > distanceFromPauseButton) && (!Pause.paused))
            {
                Debug.Log("Distance from mouse to pause button: " + mouseToPauseButtonDistance);
                playerAttack.Attack();
            }
        }

        // TOUCH ---------------------------------------------------------------------------------

        //Examine the touch inputs
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fp = touch.position;
                lp = touch.position;
            }
            if (touch.phase == TouchPhase.Moved)
            {
                lp = touch.position;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                //First check if it's actually a drag
                if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
                {   //It's a drag
                    //Now check what direction the drag was
                    //First check which axis
                    if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
                    {   //If the horizontal movement is greater than the vertical movement...
                        if (lp.x > fp.x)  //If the movement was to the right
                        {   //Right move
                            //MOVE RIGHT CODE HERE
                        }
                        else
                        {   //Left move
                            //MOVE LEFT CODE HERE
                        }
                    }
                    else
                    {   //the vertical movement is greater than the horizontal movement
                        if (lp.y > fp.y)  //If the movement was up
                        {   //Up move
                            //MOVE UP CODE HERE
                            Debug.Log("Swipe up");
                            playerMovement.MoveUp();
                        }
                        else
                        {   //Down move
                            //MOVE DOWN CODE HERE
                            Debug.Log("Swipe down");
                            playerMovement.MoveDown();
                        }
                    }
                }
                else
                {   //It's a tap
                    //TAP CODE HERE
                    if (PlayerHealth.canRestart)
                    {
                        // Restart level
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                    else if ((Vector2.Distance(lp, pauseButton.position) > distanceFromPauseButton) && (!Pause.paused))
                    {
                        playerAttack.Attack();
                    }
                }

            }
        }

        // CONTROLLER ---------------------------------------------------------------------------------
        if (!canUseAxis)
        {
            if (Input.GetAxis(dpadY) == 0)
            {
                canUseAxis = true;
            }
        }
    }

    public bool GetDpadUp()
    {
        if (canUseAxis)
        {
            if (Input.GetAxis(dpadY) > 0)
            {
                canUseAxis = false;
                return true;
            }
        }
        return false;
    }

    public bool GetDpadDown()
    {
        if (canUseAxis)
        {
            if (Input.GetAxis(dpadY) < 0)
            {
                canUseAxis = false;
                return true;
            }
        }
        return false;
    }
}
