using UnityEngine;

public class DropManager : MonoBehaviour
{
    // 싱글톤
    public static DropManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // 경험치 생성
    public void DropExp(Vector2 position)
    {
        GameObject expOrb = PoolManager.Instance.GetExpOrb();

        if (expOrb == null) return;

        // 몬스터가 죽은 위치에 생성
        expOrb.transform.position = position;

        //Debug.Log("경험치 드랍");
    }
    public void DropHealOrb(Vector2 position)
    {
        GameObject healOrb = PoolManager.Instance.GetHealOrb();

        if(healOrb == null) return;

        healOrb.transform.position = position;

        //Debug.Log("체력 구슬 드랍");
    }
}
