using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    public int pierceCount = 0; // ���� ���� Ƚ��
    public float lifetime = 3f; // ����ü ���� �ð� (��)

    private Rigidbody2D rb;
    private Vector2 direction;
    private List<Collider2D> piercedEnemies = new List<Collider2D>(); // �̹� ������ �� ���

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
        Destroy(gameObject, lifetime); // ���� �ð� �� �ڵ� �ı�
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
        // ȭ�� ������ ������ �ı��ϴ� ������ �߰��ϸ� ����
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // �̹� ������ ���̰ų�, �÷��̾� �Ǵ� �ٸ� ����ü�� �浹�� ���� ���� (�±� ��� ����)
        if (piercedEnemies.Contains(other) || other.CompareTag("Player") || other.CompareTag("Projectile"))
        {
            return;
        }

        if (other.CompareTag("Enemy")) // �� �±װ� "Enemy"��� ����
        {
            EnemyController enemy = other.GetComponent<EnemyController>(); // EnemyController�� ���� ����
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log(other.name + "���� " + damage + " ����!");
            }

            if (pierceCount <= 0)
            {
                Destroy(gameObject); // ���� Ƚ�� ���� �� �ı�
            }
            else
            {
                pierceCount--;
                piercedEnemies.Add(other); // ������ �� ��Ͽ� �߰�
            }
        }
        // else if (other.CompareTag("Obstacle")) // ��ֹ��� �浹 �� �ı�
        // {
        //     Destroy(gameObject);
        // }
    }
}
