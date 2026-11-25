
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    [Header("移動")]
    public float moveSpeed = 3f;
    public float changeDirectionTime = 4f;

    [Header("追跡")]
    public float searchRange = 15f;

    [Header("地面から出る設定")]
    public float riseSpeed = 2f;

    private float targetY;
    private bool isRising = true;

    private Rigidbody2D rb;
    private ZombieKnockback knockback;
    private EnemyTouchDamage enemyTouchDamage;

    private int direction = 1;
    private float timer;

    private Transform target;
    private bool wasKnockedBack;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        knockback = GetComponent<ZombieKnockback>();
        enemyTouchDamage = GetComponentInChildren<EnemyTouchDamage>();

        timer = changeDirectionTime;

        targetY = transform.position.y;

        transform.position = new Vector3(
            transform.position.x,
            targetY - 1.5f,
            transform.position.z
        );

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

        direction = Random.Range(0, 2) == 0 ? 1 : -1;

        UpdateSpriteDirection();
    }

    void Update()
    {
        if (isRising)
        {
            RiseFromGround();
            return;
        }

        bool knockedBack = knockback != null &&
                           knockback.IsKnockedBack();

        // ノックバック中
        if (knockedBack)
        {
            wasKnockedBack = true;
            target = null;
            return;
        }

        // ノックバック終了直後
        if (wasKnockedBack)
        {
            wasKnockedBack = false;

            target = FindNearestPlayer();

            if (target != null)
            {
                direction = target.position.x >= transform.position.x
                    ? 1 : -1;

                UpdateSpriteDirection();
            }

            timer = changeDirectionTime;
        }

        // 攻撃中は追跡しない
        if (enemyTouchDamage != null &&
            enemyTouchDamage.IsAttacking)
        {
            return;
        }

        // 常に一番近いプレイヤーを探す
        target = FindNearestPlayer();

        if (target != null)
        {


            direction = target.position.x >= transform.position.x
                ? 1 : -1;

            UpdateSpriteDirection();

            return;
        }

        // プレイヤーがいない場合は方向転換
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            ChangeDirection();
        }
    }

    void FixedUpdate()
    {
        if (isRising)
            return;

        if (knockback != null &&
            knockback.IsKnockedBack())
            return;

        // 攻撃中は完全停止
        if (enemyTouchDamage != null &&
            enemyTouchDamage.IsAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = new Vector2(
            direction * moveSpeed,
            rb.linearVelocity.y
        );
    }

    Transform FindNearestPlayer()
    {
        GameObject[] players =
            GameObject.FindGameObjectsWithTag("Player");

        Transform nearest = null;
        float nearestDistance = searchRange;

        foreach (GameObject player in players)
        {
            // ゾンビより上にいるプレイヤーは追跡対象から除外
            if (player.transform.position.y >
                transform.position.y + 0.5f)
            {
                continue;
            }

            float distance = Mathf.Abs(
                player.transform.position.x - transform.position.x
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = player.transform;
            }
        }

        return nearest;
    }

    void RiseFromGround()
    {
        float newY = Mathf.MoveTowards(
            transform.position.y,
            targetY,
            riseSpeed * Time.deltaTime
        );

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );

        if (Mathf.Approximately(transform.position.y, targetY))
        {
            isRising = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (knockback != null &&
            knockback.IsKnockedBack())
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            ChangeDirection();
        }
    }

    void ChangeDirection()
    {
        direction *= -1;
        UpdateSpriteDirection();
        timer = changeDirectionTime;
    }

    void UpdateSpriteDirection()
    {
        Vector3 scale = transform.localScale;

        if ((direction > 0 && scale.x < 0) ||
            (direction < 0 && scale.x > 0))
        {
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}
