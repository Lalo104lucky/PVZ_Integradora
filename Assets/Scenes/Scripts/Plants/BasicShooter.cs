using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class BasicShooter : MonoBehaviour
{

    [Header("Shooting")]
    public GameObject bullet;
    public Transform shootOrigin;

    public float cooldown = 1.5f;
    public float range = 10f;
    public LayerMask shootMask;

    [Header("Animation")]
    public Animator animator;

    private static readonly int isAttacking = Animator.StringToHash("isAttacking");
    private bool canShoot = true;

    private void Update()
    {
        if (canShoot && ZombieInRange())
        {
            StartCoroutine(AttackSequence());
        }
        else if (!ZombieInRange())
        {
            animator.SetBool(isAttacking, false);
        }
    }

    bool ZombieInRange()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, range, shootMask);
        return hit.collider != null;
    }

    IEnumerator AttackSequence()
    {
        canShoot = false;
        animator.SetBool(isAttacking, true);

        yield return new WaitForSeconds(cooldown);

        canShoot = true;

        if (!ZombieInRange()) 
        {
            animator.SetBool(isAttacking, false);
        }
    }

    public void Shoot() 
    {
        if (bullet == null || shootOrigin == null) return;

        Instantiate(bullet, shootOrigin.position, Quaternion.identity);
    }
}
