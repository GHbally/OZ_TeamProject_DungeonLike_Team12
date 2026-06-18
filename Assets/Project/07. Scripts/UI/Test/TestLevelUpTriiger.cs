using UnityEngine;

public class LevelUpTrigger : MonoBehaviour
{
    public LevelUpManager levelUpManager; // Hierarchy에 있는 LevelUpManager를 드래그해서 연결하세요.

    void Update()
    {
        // 테스트용: 키보드 'L'키를 누르면 레벨업 창이 뜨게 함
        if (Input.GetKeyDown(KeyCode.L))
        {
            levelUpManager.OpenLevelUp();
        }
    }
}
