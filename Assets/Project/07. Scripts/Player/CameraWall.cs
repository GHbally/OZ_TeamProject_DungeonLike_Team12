using UnityEngine;

public class CameraWall : MonoBehaviour
{
    // 벽 두께
    public float wallThickness = 0.5f;

    // 카메라보다 살짝 안쪽에 벽을 둘 여유값
    public float padding = 0.4f;

    private Camera cam;

    private BoxCollider2D topWall;
    private BoxCollider2D bottomWall;
    private BoxCollider2D leftWall;
    private BoxCollider2D rightWall;

    private void Awake()
    {
        // 이 스크립트가 붙은 오브젝트의 Camera 컴포넌트 가져오기
        cam = GetComponent<Camera>();

        // 위쪽 벽 생성
        topWall = CreateWall("TopWall");

        // 아래쪽 벽 생성
        bottomWall = CreateWall("BottomWall");

        // 왼쪽 벽 생성
        leftWall = CreateWall("LeftWall");

        // 오른쪽 벽 생성
        rightWall = CreateWall("RightWall");
    }

    private BoxCollider2D CreateWall(string wallName)
    {
        // 새 오브젝트 생성
        GameObject wall = new GameObject(wallName);

        // 카메라의 자식으로 넣기
        wall.transform.SetParent(transform);

        // 카메라 기준 로컬 위치 초기화
        wall.transform.localPosition = Vector3.zero;

        // 벽 태그 설정
        wall.tag = "Wall";

        // BoxCollider2D 추가
        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();

        // 실제 벽 충돌로 사용하기 위해 Trigger 끄기
        col.isTrigger = false;

        // 생성한 Collider 반환
        return col;
    }

    private void LateUpdate()
    {
        // 카메라가 없으면 종료
        if (cam == null) return;

        // 카메라 세로 절반 크기
        float height = cam.orthographicSize;

        // 카메라 가로 절반 크기
        float width = height * cam.aspect;

        // 위쪽 벽 위치 설정
        topWall.transform.localPosition = new Vector3(0f, height - padding, 0f);

        // 위쪽 벽 크기 설정
        topWall.size = new Vector2(width * 2f, wallThickness);

        // 아래쪽 벽 위치 설정
        bottomWall.transform.localPosition =  new Vector3(0f, -height + padding, 0f);

        // 아래쪽 벽 크기 설정
        bottomWall.size = new Vector2(width * 2f, wallThickness);

        // 왼쪽 벽 위치 설정
        leftWall.transform.localPosition = new Vector3(-width + padding, 0f, 0f);

        // 왼쪽 벽 크기 설정
        leftWall.size = new Vector2(wallThickness, height * 2f);

        // 오른쪽 벽 위치 설정
        rightWall.transform.localPosition = new Vector3(width - padding, 0f, 0f);

        // 오른쪽 벽 크기 설정
        rightWall.size = new Vector2(wallThickness, height * 2f);
    }
}