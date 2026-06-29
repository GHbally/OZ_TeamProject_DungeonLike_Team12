using Unity.VisualScripting;
using UnityEngine;

public class StageMapSwitcher : MonoBehaviour
{
    [Header("스테이지 맵 목록")]
    [SerializeField] private GameObject[] stageMaps;

    [Header("각 스테이지의 플레이어 시작 위치")]
    [SerializeField] private Transform[] playerSpawnPoints;

    [Header("플레이어")]
    [SerializeField] private Transform player;

    // 외부에서 스테이지 번호를 넘겨주면
    // 해당 스테이지 맵만 켜고 플레이어를 해당 위치로 이동시킨다.

    public void ChangeMap(int stage)
    {
        // 스테이지 번호는 1부터 시작하지만,
        // 배열 인덱스는 0부터 시작하므로 -1을 해준다.
        int targetIndex = stage - 1;
        
        // 잘못된 스테이지 번호가 들어오면 실행하지 않는다.
        if(targetIndex < 0 || targetIndex >= stageMaps.Length)
        {
            return;
        }

        // 모든 맵을 돌면서 현재 스테이지 맵만 켜고 나머지는 끈다.
        for (int i = 0; i< stageMaps.Length; i++)
        {
            if (stageMaps[i] == null)
            {
                continue;
            }
        }

        // 플레이어를 현재 스테이지 시작 위치로 이동시킨다.
        MovePlayerToSpawn(targetIndex);
    }

    private void MovePlayerToSpawn(int targetIndex)
    {
        if(player == null)
        {
            return;
        }
        
        if(targetIndex < 0 || targetIndex >= playerSpawnPoints.Length)
        {
            return;
        }

        if (playerSpawnPoints[targetIndex] == null)
        {
            return;
        }

        player.position = playerSpawnPoints[targetIndex].position;
    }

    


}
