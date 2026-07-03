using UnityEngine;

public class ObjectGlow : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color glowColor = Color.white;
    void Start()
    {
        // 먼저 이 오브젝트에서 SpriteRenderer를 찾는다.
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // 이 오브젝트에 SpriteRenderer가 없으면 자식 오브젝트에서 찾는다.
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        // 그래도 SpriteRenderer가 없으면 오류가 나지 않도록 종료한다.
        if (spriteRenderer == null)
        {
            Debug.LogWarning(
                $"ObjectGlow: {gameObject.name} 또는 자식 오브젝트에 SpriteRenderer가 없습니다.",
                gameObject
            );

            return;
        }

        //게임이 시작되면 내 스프라이트의 색상을 인스펙터에서 고른 HDR 색으로 덮어씀
        spriteRenderer.material.SetColor("_Color", glowColor);
    }
}