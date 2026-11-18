
using UnityEngine;

public class RedOutline : MonoBehaviour
{
    public SpriteRenderer sourceRenderer;

    private SpriteRenderer outlineRenderer;

    void Awake()
    {
        outlineRenderer = GetComponent<SpriteRenderer>();
        outlineRenderer.enabled = false;
    }

    void LateUpdate()
    {
        if (sourceRenderer == null || outlineRenderer == null)
            return;

        outlineRenderer.sprite = sourceRenderer.sprite;
        outlineRenderer.flipX = sourceRenderer.flipX;
        outlineRenderer.flipY = sourceRenderer.flipY;

        outlineRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
        outlineRenderer.sortingOrder = sourceRenderer.sortingOrder - 1;


    }

    public void SetChargeStage(int stage)
    {
        if (outlineRenderer == null)
            return;

        if (stage == 0)
        {
            outlineRenderer.enabled = false;
            return;
        }

        outlineRenderer.enabled = true;

        if (stage == 1)
        {
            outlineRenderer.color = new Color(1f, 0f, 0f, 0.9f); 
            transform.localScale = new Vector3(1.08f, 1.08f, 1f);
        }
        else if (stage == 2)
        {
            outlineRenderer.color = new Color(1f, 0f, 0f, 0.9f); 
            transform.localScale = new Vector3(1.15f, 1.15f, 1f);
        }
    }
}
