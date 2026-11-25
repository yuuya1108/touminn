
using UnityEngine;

public class EnemyTouchDamage : MonoBehaviour
{
    [Header("通常攻撃")]
    public float attackRange = 1.5f;
    public float attackWindup = 0.3f;
    public float attackDuration = 0.7f;
    public float attackCooldown = 1.5f;

    [Header("通常攻撃のノックバック")]
    public float knockbackX = 8f;
    public float knockbackY = 7f;
    public float hitStunDuration = 0.5f;

    [Header("攻撃判定")]
    public float attackRadius = 1.2f;
    public float attackOffsetX = 0.8f;
    public float attackOffsetY = 0.3f;

    private ZombieKnockback zombieKnockback;

    private bool isAttacking;
    private bool attackHitDone;

    private float attackTimer;
    private float cooldownTimer;

    private int attackDirection = 1;

    public bool IsAttacking
    {
        get { return isAttacking; }
    }

    private Rigidbody2D zombieRb;

    private void Awake()
    {
        zombieKnockback = GetComponentInParent<ZombieKnockback>();
        zombieRb = GetComponentInParent<Rigidbody2D>();
    }

    private void Update()
    {
        if (zombieKnockback != null &&
            zombieKnockback.IsKnockedBack())
        {
            isAttacking = false;
            attackTimer = 0f;
            return;
        }

        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        // 攻撃中
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;

            if (!attackHitDone && attackTimer >= attackWindup)
            {
                AttackPlayer();
                attackHitDone = true;
            }

            if (attackTimer >= attackDuration)
            {
                isAttacking = false;
                cooldownTimer = attackCooldown;
            }

            return;
        }

        if (cooldownTimer > 0f)
            return;

        Transform target = FindPlayerInAttackRange();

        if (target == null)
            return;

        attackDirection =
            target.position.x >= transform.position.x ? 1 : -1;

        StartAttack();
    }

    private Transform FindPlayerInAttackRange()
    {
        GameObject[] players =
            GameObject.FindGameObjectsWithTag("Player");

        Transform nearest = null;
        float nearestDistance = attackRange;

        foreach (GameObject player in players)
        {
            // ゾンビの頭上にいるプレイヤーは攻撃対象外
            if (player.transform.position.y >
                transform.position.y + 1.0f)
            {
                continue;
            }

            float distance = Vector2.Distance(
                transform.position,
                player.transform.position
            );

            if (distance <= nearestDistance)
            {
                nearestDistance = distance;
                nearest = player.transform;
            }
        }

        return nearest;
    }

    private void StartAttack()
    {
        isAttacking = true;
        attackHitDone = false;
        attackTimer = 0f;

        Debug.Log("ゾンビ攻撃開始！ IsAttacking = " + isAttacking);

        if (zombieRb != null)
            zombieRb.linearVelocity = Vector2.zero;
    }

    private void AttackPlayer()
    {
        Vector2 attackPosition =
            (Vector2)transform.position +
            new Vector2(
                attackDirection * attackOffsetX,
                attackOffsetY
            );

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPosition,
            attackRadius
        );

        foreach (Collider2D hit in hits)
        {
            Controller player = hit.GetComponent<Controller>();

            if (player == null)
                player = hit.GetComponentInParent<Controller>();

            if (player == null)
                continue;

            float direction =
                hit.transform.position.x >= transform.position.x
                ? 1f : -1f;

            player.ReceiveKnockback(
                direction * knockbackX,
                knockbackY,
                hitStunDuration
            );

            Debug.Log("ゾンビの通常攻撃が命中！");
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 飛んでいるゾンビの命中処理は
        // ZombieKnockback側に任せる
    }
}
