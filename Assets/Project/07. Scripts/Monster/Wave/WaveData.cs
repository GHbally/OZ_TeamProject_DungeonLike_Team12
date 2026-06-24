using System;

[Serializable]
public class WaveData
{
    public int warriorCount;
    public int archerCount;

    //몬스터 체력 배율
    public float hpMultiplier = 1f;
    //몬스터 이동속도 배율
    public float speedMultiplier = 1f;
}