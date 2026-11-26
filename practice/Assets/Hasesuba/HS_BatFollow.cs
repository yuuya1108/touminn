using UnityEngine;

public class BatFollowPose : MonoBehaviour
{
    [System.Serializable]
    public class Pose
    {
        public Sprite bodySprite;   // この体の絵のとき
        public Vector2 position;    // バットの位置
        public float angle;         // バットの角度
    }

    public SpriteRenderer body;     // Body を入れる
    public Pose[] poses;            // 絵ごとの表

    void LateUpdate()
    {
        if (body == null) return;

        foreach (Pose p in poses)
        {
            if (p.bodySprite == body.sprite)
            {
                transform.localPosition = new Vector3(
                    p.position.x, p.position.y, transform.localPosition.z);
                transform.localRotation = Quaternion.Euler(0f, 0f, p.angle);
                return;
            }
        }
    }
}