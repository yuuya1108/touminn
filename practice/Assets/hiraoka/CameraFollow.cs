using UnityEngine;

public class CameraFollow2DFight : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    [Header("追従のなめらかさ")]
    public float smoothSpeed = 5f;

    [Header("画面端の余白（プレイヤーの幅に合わせる）")]
    public float screenPadding = 0.5f;

    [Header("カメラを押し出せる画面端からの距離")]
    [Range(0.1f, 2.0f)]
    public float pushZoneWidth = 0.8f;

    private Camera cam;

    // プレイヤーのControllerスクリプトへの参照
    private Controller p1Controller;
    private Controller p2Controller;

    [Header("死亡・リスポーン時のカメラ移動")]
    public float transitionSpeed = 1.5f;

    private bool isTransitioning = false;
    private float transitionTimer = 0f;
    private float transitionStartX;
    private float transitionTargetX;

    private bool wasP1Alive = true;
    private bool wasP2Alive = true;

    void RestrictPlayersInScreen(bool p1Alive, bool p2Alive)
    {
        if (cam == null) return;

        float screenHalfWidth = cam.orthographicSize *
            ((float)Screen.width / Screen.height);

        float minX = transform.position.x - screenHalfWidth + screenPadding;
        float maxX = transform.position.x + screenHalfWidth - screenPadding;

        if (p1Alive)
        {
            Vector3 p1Pos = player1.position;
            p1Pos.x = Mathf.Clamp(p1Pos.x, minX, maxX);
            player1.position = p1Pos;
        }

        if (p2Alive)
        {
            Vector3 p2Pos = player2.position;
            p2Pos.x = Mathf.Clamp(p2Pos.x, minX, maxX);
            player2.position = p2Pos;
        }
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        // 各プレイヤーのControllerスクリプトを取得
        if (player1 != null) p1Controller = player1.GetComponent<Controller>();
        if (player2 != null) p2Controller = player2.GetComponent<Controller>();
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return;

        bool p1Alive = p1Controller != null ? !p1Controller.isDead : true;
        bool p2Alive = p2Controller != null ? !p2Controller.isDead : true;

        // 死亡・リスポーンを検知
        if (p1Alive != wasP1Alive || p2Alive != wasP2Alive)
        {
            float targetX = transform.position.x;

            if (p1Alive && p2Alive)
            {
                // 2人とも生存：2人の中央
                targetX = (player1.position.x + player2.position.x) / 2f;
            }
            else if (p1Alive)
            {
                // プレイヤー1のみ生存
                targetX = player1.position.x;
            }
            else if (p2Alive)
            {
                // プレイヤー2のみ生存
                targetX = player2.position.x;
            }

            // カメラ移動開始
            transitionStartX = transform.position.x;
            transitionTargetX = targetX;
            transitionTimer = 0f;
            isTransitioning = true;
        }

        wasP1Alive = p1Alive;
        wasP2Alive = p2Alive;

        float targetXNormal = transform.position.x;

        if (p1Alive && p2Alive)
        {
            targetXNormal = (player1.position.x + player2.position.x) / 2f;
        }
        else if (p1Alive)
        {
            targetXNormal = player1.position.x;
        }
        else if (p2Alive)
        {
            targetXNormal = player2.position.x;
        }

        // カメラ移動
        if (isTransitioning)
        {
            transitionTimer += Time.deltaTime * transitionSpeed;

            float t = Mathf.SmoothStep(0f, 1f,
                Mathf.Clamp01(transitionTimer));

            // 2人が生存している場合は中央を追従しながら移動
            if (p1Alive && p2Alive)
            {
                transitionTargetX =
                    (player1.position.x + player2.position.x) / 2f;
            }
            else if (p1Alive)
            {
                transitionTargetX = player1.position.x;
            }
            else if (p2Alive)
            {
                transitionTargetX = player2.position.x;
            }

            float newX = Mathf.Lerp(
                transitionStartX,
                transitionTargetX,
                t
            );

            transform.position = new Vector3(
                newX,
                transform.position.y,
                transform.position.z
            );

            if (transitionTimer >= 1f)
            {
                isTransitioning = false;
            }
        }
        else
        {
            Vector3 targetPosition = new Vector3(
                targetXNormal,
                transform.position.y,
                transform.position.z
            );

            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                smoothSpeed * Time.deltaTime
            );
        }

        // 画面端の押し出し処理
        if (cam != null && !isTransitioning)
        {
            float screenHalfWidth = cam.orthographicSize *
                ((float)Screen.width / Screen.height);

            float leftPushBound = transform.position.x -
                screenHalfWidth + pushZoneWidth;

            float rightPushBound = transform.position.x +
                screenHalfWidth - pushZoneWidth;

            if (p1Alive)
            {
                if (player1.position.x > rightPushBound)
                {
                    transform.position += new Vector3(
                        player1.position.x - rightPushBound, 0, 0);
                }
                else if (player1.position.x < leftPushBound)
                {
                    transform.position += new Vector3(
                        player1.position.x - leftPushBound, 0, 0);
                }
            }

            if (p2Alive)
            {
                if (player2.position.x > rightPushBound)
                {
                    transform.position += new Vector3(
                        player2.position.x - rightPushBound, 0, 0);
                }
                else if (player2.position.x < leftPushBound)
                {
                    transform.position += new Vector3(
                        player2.position.x - leftPushBound, 0, 0);
                }
            }
        }

        RestrictPlayersInScreen(p1Alive, p2Alive);
    }
}