using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Transform[] tracks;

    [SerializeField]
    private InputManager inputManager;

    private int track;           // 0 = top, 1 = middle, 2 = bottom

    public bool canAttack = true;

    // Use this for initialization
    void Start()
    {
        track = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Up") || inputManager.GetDpadUp())
        {
            MoveUp();
        }
        else if (Input.GetButtonDown("Down") || inputManager.GetDpadDown())
        {
            MoveDown();
        }
    }

    public void MoveUp()
    {
        if (track > 0)
        {
            track--;
            transform.position = tracks[track].position;
        }
    }

    public void MoveDown()
    {
        if (track < (tracks.Length - 1))
        {
            track++;
            transform.position = tracks[track].position;
        }
    }
}
