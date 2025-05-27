using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    public int pierceCount = 0; // 남은 관통 횟수
    public float lifetime = 3f; // 투사체 생존 시간 (초)

    private Rigidbody2D rb;
    private Vector2 direction;
    private List<Collider2D> piercedEnemies = new List<Collider2D>(); // 이미 관통한 적 목록

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 dir, float projSpeed, int projDamage, int projPierce)
    {
        direction = dir.normalized;
        speed = projSpeed;
        damage = projDamage;
        pierceCount = projPierce;
        Destroy(gameObject, lifetime); // 일정 시간 후 자동 파괴
    }

    void FixedUpdate()
    {
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            transform.Translate(direction * speed * Time.fixedDeltaTime, Space.World);
        }
        else // Dynamic
        {
            rb.linearVelocity = direction * speed;
        }
        // 화면 밖으로 나가면 파괴하는 로직도 추가하면 좋음
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 이미 관통한 적이거나, 플레이어 또는 다른 투사체와 충돌한 경우는 무시 (태그 사용 권장)
        if (piercedEnemies.Contains(other) || other.CompareTag("Player") || other.CompareTag("Projectile"))
        {
            return;
        }

        if (other.CompareTag("Enemy")) // 적 태그가 "Enemy"라고 가정
        {
            EnemyController enemy = other.GetComponent<EnemyController>(); // EnemyController는 추후 생성
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log(other.name + "에게 " + damage + " 피해!");
            }

            if (pierceCount <= 0)
            {
                Destroy(gameObject); // 관통 횟수 소진 시 파괴
            }
            else
            {
                pierceCount--;
                piercedEnemies.Add(other); // 관통한 적 목록에 추가
            }
        }
        // else if (other.CompareTag("Obstacle")) // 장애물과 충돌 시 파괴
        // {
        //     Destroy(gameObject);
        // }
    }
}
