using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEndAttack : MonoBehaviour
{
    public void EndAttack()
    {
        PlayerAttack.isAttacking = false;
    }
}
