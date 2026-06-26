using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    
    public static PoolManager Instance;

    [Header("프리팹")]

    public GameObject arrowPrefab;
    public GameObject expOrbPrefab;
    public GameObject healOrbPrefab;
    public GameObject warriorMonsterPrefab;
    public GameObject archerMonsterPrefab;

    [Header("풀 크기")]

    public int arrowPoolSize = 70; //화살 개수 미리 생성
    public int expPoolSize = 100; //경험치 개수 미리 생성
    public int healOrbPoolSize = 20;
    public int warriorPoolSize = 100; //몬스터 수
    public int archerPoolSize = 50;

    [Header("보스 탄환")]

    public GameObject bossBulletPrefab;     // 직진 탄환
    public int bossBulletPoolSize = 120;

    [Header("반사 탄환")]

    public GameObject bounceBulletPrefab;    // 벽 반사 탄환
    public int bounceBulletPoolSize = 30;


    private Queue<GameObject> arrowPool = new Queue<GameObject>();
    private Queue<GameObject>expPool = new Queue<GameObject>();
    private Queue<GameObject>healOrbPool = new Queue<GameObject>();
    private Queue<GameObject> warriorPool = new Queue<GameObject>();
    private Queue<GameObject> archerPool = new Queue<GameObject>();
    private Queue<GameObject> bossBulletPool = new Queue<GameObject>();
    private Queue<GameObject> bounceBulletPool = new Queue<GameObject>();


    private void Awake()
    {
        //싱글톤 등록
        Instance = this;

        //게임 시작 시 미리 생성
        CreateArrowPool();
        CreateExpPool();
        CreateHealOrbPool();
        CreateWarriorPool();
        CreateArcherPool();
        CreateBossBulletPool();
        CreateBounceBulletPool();
    }

    // 화살 풀 생성
    void CreateArrowPool()
    {
        for (int i = 0; i < arrowPoolSize; i++)
        {
            //화살 생성
            GameObject obj=Instantiate(arrowPrefab);

            //비활성화
            obj.SetActive(false);

            //큐에 저장
            arrowPool.Enqueue(obj);
        }
    }

    

    //화살 꺼내기
    public GameObject GetArrow()
    {
        //풀에 화살없음
        if(arrowPool.Count == 0) return null;

        //하나 꺼내기
        GameObject obj = arrowPool.Dequeue();

        //활성화
        obj.SetActive(true);

        return obj;
    }

    //화살 반환
    public void ReturnArrow(GameObject obj)
    {
        //비활성화
        obj.SetActive(false);

        //다시 큐에 저장
        arrowPool.Enqueue(obj);
    }
    void CreateExpPool()
    {
        for (int i = 0; i < expPoolSize; i++)
        {
            GameObject obj = Instantiate(expOrbPrefab);
            obj.SetActive(false);
            expPool.Enqueue(obj);
        }
    }

    //경험치 구슬 꺼내기
    public GameObject GetExpOrb()
    {
        if (expPool.Count == 0)
        {
            GameObject newObj = Instantiate(expOrbPrefab);
            newObj.SetActive(false);
            expPool.Enqueue(newObj);
        }

        GameObject obj = expPool.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    //경험치 구슬 반환
    public void ReturnExpOrb(GameObject obj)
    {
        obj.SetActive(false) ;
        expPool.Enqueue(obj);
    }


    //전사 몬스터 풀 생성
    void CreateWarriorPool()
    {
        for (int i = 0; i < warriorPoolSize; i++)
        {
            GameObject obj = Instantiate(warriorMonsterPrefab);
            obj.SetActive(false);
            warriorPool.Enqueue(obj);
        }
    }
    
    //궁수 몬스터 풀 생성
    void CreateArcherPool()
    {
        for (int i = 0; i < archerPoolSize; i++)
        {
            GameObject obj = Instantiate(archerMonsterPrefab);
            obj.SetActive(false);
            archerPool.Enqueue(obj);
        }
    }

    //전사 몬스터 꺼내기
    public GameObject GetWarriorMonster()
    {
        if (warriorPool.Count == 0) return null;

        GameObject obj = warriorPool.Dequeue();
        obj.SetActive(true);

        return obj;
    }

    //궁스 몬스터 꺼내기
    public GameObject GetArcherMonster()
    {
        if (archerPool.Count == 0) return null;

        GameObject obj = archerPool.Dequeue();
        obj.SetActive(true);

        return obj;
    }


    //몬스터 반환
    public void ReturnMonster(GameObject monster)
    {
        monster.SetActive(false);

        //전사 몬스터인지 확인
        if (monster.GetComponent<WarriorMonster>() != null)
        {
            warriorPool.Enqueue(monster);
        }

        //궁수 몬스터인지 확인
        else if (monster.GetComponent<ArcherMonster>() != null)
        {
            archerPool.Enqueue(monster);
        }
    }

    //힐 풀 생성
    void CreateHealOrbPool()
    {
        for (int i = 0; i < healOrbPoolSize; i++)
        {
            GameObject obj = Instantiate(healOrbPrefab);
            obj.SetActive(false);
            healOrbPool.Enqueue(obj);
        }
        
    }

    //꺼내기
    public GameObject GetHealOrb()
    {
        if (healOrbPool.Count == 0)
        {
            GameObject newObj = Instantiate(healOrbPrefab);
            newObj.SetActive(false);
            healOrbPool.Enqueue(newObj);
        }

        GameObject obj = healOrbPool.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    //반환
    public void ReturnHealOrb(GameObject obj)
    {
        obj.SetActive(false);
        healOrbPool.Enqueue(obj);
    }

    // 보스 탄환 풀 생성
    void CreateBossBulletPool()
    {
        for (int i = 0; i < bossBulletPoolSize; i++)
        {
            GameObject obj = Instantiate(bossBulletPrefab);

            obj.SetActive(false);

            bossBulletPool.Enqueue(obj);
        }
    }

    // 반사 탄환 풀 생성
    void CreateBounceBulletPool()
    {
        for (int i = 0; i < bounceBulletPoolSize; i++)
        {
            GameObject obj = Instantiate(bounceBulletPrefab);

            obj.SetActive(false);

            bounceBulletPool.Enqueue(obj);
        }
    }

    // 보스 탄환 가져오기
    public GameObject GetBossBullet()
    {
        if (bossBulletPool.Count == 0)
        {
            GameObject newObj = Instantiate(bossBulletPrefab);
            newObj.SetActive(false);
            bossBulletPool.Enqueue(newObj);
        }

        GameObject obj = bossBulletPool.Dequeue();
        obj.SetActive(true);

        return obj;
    }

    // 반사 탄환 가져오기
    public GameObject GetBounceBullet()
    {
        if (bounceBulletPool.Count == 0)
        {
            GameObject newObj = Instantiate(bounceBulletPrefab);
            newObj.SetActive(false);
            bounceBulletPool.Enqueue(newObj);
        }

        GameObject obj = bounceBulletPool.Dequeue();
        obj.SetActive(true);

        return obj;
    }

    // 보스 탄환 반환
    public void ReturnBossBullet(GameObject obj)
    {
        obj.SetActive(false);

        bossBulletPool.Enqueue(obj);
    }

    // 반사 탄환 반환
    public void ReturnBounceBullet(GameObject obj)
    {
        obj.SetActive(false);

        bounceBulletPool.Enqueue(obj);
    }
}
