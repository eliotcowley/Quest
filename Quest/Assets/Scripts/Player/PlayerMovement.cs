using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private Transform[] tracks;

    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private string switchButton = "Switch";

    [SerializeField]
    private Transform frontPos;

    [SerializeField]
    private Transform backPos;

    [SerializeField]
    private float smoothSpeed = 5f;

    private int track;           // 0 = top, 1 = middle, 2 = bottom
    private PlayerAttack playerAttack;

    // Use this for initialization
    void Start()
    {
        track = 1;
        playerAttack = GetComponent<PlayerAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Pause.paused) || (!canMove))  return;
        if (Input.GetButtonDown("Up") || inputManager.GetDpadUp())
        {
            MoveUp();
        }
        else if (Input.GetButtonDown("Down") || inputManager.GetDpadDown())
        {
            MoveDown();
        }
        else if (Input.GetButtonDown(switchButton))
        {
            Switch();
        }

        transform.position = Vector3.Lerp(transform.position, tracks[track].position, smoothSpeed * Time.deltaTime);
    }

    public void MoveUp()
    {
        if (track > 0)
        {
            track--;
            //transform.position = tracks[track].position;
        }
    }

    public void MoveDown()
    {
        if (track < (tracks.Length - 1))
        {
            track++;
            //transform.position = tracks[track].position;
        }
    }

    public void Switch()
    {
        Vector2 temp = frontPos.position;
        frontPos.position = backPos.position;
        backPos.position = temp;
        playerAttack.swordsmanInFront = !playerAttack.swordsmanInFront;
    }
}
