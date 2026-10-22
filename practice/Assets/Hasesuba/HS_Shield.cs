
using UnityEngine;

public class AutoGuardItem : MonoBehaviour
{
    [Header("アイテム設定")]
    public bool hasItem = true;

    [Header("反撃設定")]
    public float stunDuration = 1.5f;

    public GameObject shieldObject;

    void Start()
    {
        shieldObject.SetActive(hasItem);
    }

    // 攻撃を防げたかどうかを返す
    public bool TryBlock(Controller attacker)
    {
        if (!hasItem)
        {
            return false;
        }

        // アイテムを消費
        hasItem = false;

        shieldObject.SetActive(false);

        // 攻撃者を硬直させる
        if (attacker != null)
        {
            attacker.ApplyItemStun(stunDuration);
        }

        Debug.Log("自動ガード発動！ アイテムを消費しました。");

        return true;
    }
}
