using UnityEngine;

public class ObjectGlow : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color glowColor = Color.white;
    void Start()
    {
        //게임이 시작되면 내 스프라이트의 색상을 인스펙터에서 고른 HDR 색으로 덮어씀
        GetComponent<SpriteRenderer>().material.SetColor("_Color", glowColor);
    }
}