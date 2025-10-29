using Unity.VisualScripting;
using UnityEngine;

public class BasicShooter : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bullet;
    public Transform shootOrigin;
    public float cooldown;
    public float range;
    public LayerMask shootMask;

    [Header("Animation")]
    public Animator animator;
    private static readonly int isAttacking = Animator.StringToHash("isAttacking");

    private bool canShoot = true;

    private void Update()
    {
        if(canShoot && ZombieInRange())
        {
            StartCoroutine(AttackSequence());
        } else if (!ZombieInRange())
        {
            animator.SetBool(isAttacking, false);
        }
    }

    bool ZombieInRange()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, range, shootMask);
        return hit.collider != null;
    }

    System.Collections.IEnumerator AttackSequence()
    {
        canShoot = false;
        animator.SetBool(isAttacking, true);

        yield return new WaitForSeconds(0.48f);

        Shoot();

        yield return new WaitForSeconds(cooldown - 0.48f);

        canShoot = true;

        if (!ZombieInRange()) {
            animator.SetBool(isAttacking, false);
        }
    }

    void Shoot() {

        if (bullet == null || shootOrigin == null) return;

        GameObject mybullet = Instantiate(bullet, shootOrigin.position, Quaternion.identity);
    }
}
