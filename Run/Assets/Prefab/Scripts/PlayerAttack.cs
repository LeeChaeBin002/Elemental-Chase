using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject projectilePrefab; // 쏘는 프리팹
    public Transform firePoint;         // 발사 위치 (플레이어 앞 빈 오브젝트)


    public void ShootAt(EnemyMove enemy, GameObject projectilePrefab)
    {
        if (enemy == null || projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
        {
            p.SetTarget(enemy.transform);
        }
    }
}
