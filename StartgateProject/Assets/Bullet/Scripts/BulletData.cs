using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Scriptable Objects/BulletData")]
public class BulletData : ScriptableObject
{
    [Header("Visual & General Settings")]
    public Sprite bulletSprite;         // Merminin görseli
    public float bulletSpeed = 10f;     // Merminin hýzý
    public int bulletDamage = 1;        // Merminin verdiði hasar

    [Header("Special Properties")]
    public int bulletPierce = 0;        // Merminin kaç hedefe kadar delip geçebileceði
    public float bulletLifetime = 5f;  // Merminin ömrü (sahnede ne kadar kalacak)
}