
using System.Collections.Generic;
using UnityEngine;

public class ZombieKnockback : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isKnockedBack = false;

    // 一度ぶつかったプレイヤーを記録
    private HashSet<int> hitPlayers = new HashSet<int>();

    [Header("着地判定")]
    public LayerMask groundLayer;

    [Header("プレイヤー接触時のノックバック")]
    public float collisionKnockbackX = 20f;
    public float collisionKnockbackY = 10f;
    public float collisionHitStunDuration = 0f;

    private float receivedPowerX;
    private float receivedPowerY;

    [Header("吹っ飛び中のプレイヤー判定")]
    public Vector2 playerHitboxSize = new Vector2(1.5f, 2f);
    public Vector2 playerHitboxOffset = Vector2.zero;


    private int attackerPlayerID = -1;

    // ゾンビを飛ばしたプレイヤーの向き
    private int attackerFacingDirection = 1;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // ゾンビが吹っ飛ばされたとき
    public void GetKnockedBack(
      float powerX,
      float powerY,
      Controller attacker)
    {
        if (rb == null) return;

        isKnockedBack = true;
        hitPlayers.Clear();

        attackerPlayerID = attacker != null
            ? attacker.gameObject.GetInstanceID()
            : -1;

        if (attacker != null)
        {
            // P1とP2でflipXの意味が逆なので分ける
            if (!attacker.player2)
            {
                attackerFacingDirection =
                    attacker.spriteRenderer.flipX ? -1 : 1;
            }
            else
            {
                attackerFacingDirection =
                    attacker.spriteRenderer.flipX ? 1 : -1;
            }
        }
        // バットで受けた攻撃力を保存
        receivedPowerX = Mathf.Abs(powerX);
        receivedPowerY = powerY;

        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = new Vector2(powerX, powerY);
    }


    public void TryHitPlayer(GameObject otherObject)
    {
        if (!isKnockedBack) return;
        if (rb == null) return;

        Controller player = otherObject.GetComponent<Controller>();

        if (player == null)
            player = otherObject.GetComponentInParent<Controller>();

        if (player == null)
            return;

        int playerID = player.gameObject.GetInstanceID();

        // 攻撃した本人には当たらない
        if (playerID == attackerPlayerID)
            return;

        // 一度当たったプレイヤーには再度当たらない
        if (hitPlayers.Contains(playerID))
            return;

        // ゾンビの飛んでいる方向を取得
        float velocityX = rb.linearVelocity.x;

        if (Mathf.Abs(velocityX) < 0.1f)
            return;
        // 攻撃したプレイヤーが向いている方向
        float directionX = attackerFacingDirection;

        hitPlayers.Add(playerID);

        Debug.Log("飛んでいるゾンビが命中！反対方向へ吹っ飛ばす！");

        player.ReceiveKnockback(
            directionX * receivedPowerX,
            receivedPowerY,
            collisionHitStunDuration
        );
    }



    // 地面との衝突

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGround(collision);
    }

    private void CheckGround(Collision2D collision)
    {
        if (!isKnockedBack) return;

        if (((1 << collision.gameObject.layer) & groundLayer) == 0)
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // 地面から上向きの接触なら着地
            if (contact.normal.y > 0.5f)
            {
                isKnockedBack = false;

                Debug.Log("ゾンビのノックバック解除！");
                return;
            }
        }
    }

    public bool IsKnockedBack()
    {
        return isKnockedBack;
    }
    private void FixedUpdate()
    {
        if (!isKnockedBack) return;

        Vector2 center = (Vector2)transform.position + playerHitboxOffset;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            center,
            playerHitboxSize,
            0f
        );

        foreach (Collider2D hit in hits)
        {
            TryHitPlayer(hit.gameObject);
        }
    }
}
