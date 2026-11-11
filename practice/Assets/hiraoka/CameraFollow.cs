
using UnityEngine;

public class CameraFollow2DFight : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    [Header("追従のなめらかさ")]
    public float smoothSpeed = 5f;

    [Header("画面端の余白")]
    public float screenPadding = 0.5f;

    [Header("カメラを押し出せる画面端からの距離")]
    [Range(0.1f, 2.0f)]
    public float pushZoneWidth = 0.8f;

    [Header("死亡・リスポーン時の切り替え時間")]
    public float transitionDuration = 2.0f;

    private Camera cam;

    private Controller p1Controller;
    private Controller p2Controller;

    private bool wasP1Alive = true;
    private bool wasP2Alive = true;

    private bool isTransitioning = false;
    private float transitionTimer = 0f;

    private float transitionStartX;
    private float transitionTargetX;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;

        if (player1 != null)
            p1Controller = player1.GetComponent<Controller>();

        if (player2 != null)
            p2Controller = player2.GetComponent<Controller>();

        wasP1Alive = IsP1Alive();
        wasP2Alive = IsP2Alive();
    }

    bool IsP1Alive()
    {
        return p1Controller != null ? !p1Controller.isDead : true;
    }

    bool IsP2Alive()
    {
        return p2Controller != null ? !p2Controller.isDead : true;
    }

    float GetTargetX(bool p1Alive, bool p2Alive)
    {
        if (p1Alive && p2Alive)
        {
            return (player1.position.x + player2.position.x) / 2f;
        }
        else if (p1Alive)
        {
            return player1.position.x;
        }
        else if (p2Alive)
        {
            return player2.position.x;
        }

        return transform.position.x;
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null)
            return;

        bool p1Alive = IsP1Alive();
        bool p2Alive = IsP2Alive();

        // 死亡・リスポーンを検知
        if (p1Alive != wasP1Alive || p2Alive != wasP2Alive)
        {
            StartCameraTransition(p1Alive, p2Alive);
        }

        wasP1Alive = p1Alive;
        wasP2Alive = p2Alive;

        float targetX = GetTargetX(p1Alive, p2Alive);

        // カメラ移動
        if (isTransitioning)
        {
            transitionTimer += Time.deltaTime;

            float t = transitionDuration > 0f
                ? Mathf.Clamp01(transitionTimer / transitionDuration)
                : 1f;

            t = Mathf.SmoothStep(0f, 1f, t);

            // 移動先を常に更新
            transitionTargetX = targetX;

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

            if (transitionTimer >= transitionDuration)
            {
                isTransitioning = false;
            }
        }
        else
        {
            Vector3 targetPosition = new Vector3(
                targetX,
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
                    transform.position += Vector3.right *
                        (player1.position.x - rightPushBound);
                }
                else if (player1.position.x < leftPushBound)
                {
                    transform.position += Vector3.right *
                        (player1.position.x - leftPushBound);
                }
            }

            if (p2Alive)
            {
                if (player2.position.x > rightPushBound)
                {
                    transform.position += Vector3.right *
                        (player2.position.x - rightPushBound);
                }
                else if (player2.position.x < leftPushBound)
                {
                    transform.position += Vector3.right *
                        (player2.position.x - leftPushBound);
                }
            }
        }

        if (!isTransitioning)
        {
            RestrictPlayersInScreen(p1Alive, p2Alive);
        }
    }

    void StartCameraTransition(bool p1Alive, bool p2Alive)
    {
        isTransitioning = true;
        transitionTimer = 0f;

        transitionStartX = transform.position.x;
        transitionTargetX = GetTargetX(p1Alive, p2Alive);
    }

    void RestrictPlayersInScreen(bool p1Alive, bool p2Alive)
    {
        if (cam == null)
            return;

        float screenHalfWidth = cam.orthographicSize *
            ((float)Screen.width / Screen.height);

        float minX = transform.position.x -
            screenHalfWidth + screenPadding;

        float maxX = transform.position.x +
            screenHalfWidth - screenPadding;

        if (p1Alive)
        {
            Vector3 pos = player1.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            player1.position = pos;
        }

        if (p2Alive)
        {
            Vector3 pos = player2.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            player2.position = pos;
        }
    }
}