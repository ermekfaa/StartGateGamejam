using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Scriptable Objects/BulletData")]
public class BulletData : ScriptableObject
{
    [Header("Visual & General Settings")]
    public Sprite bulletSprite;         // Merminin g�rseli
    public float bulletSpeed = 10f;     // Merminin h�z�
    public int bulletDamage = 1;        // Merminin verdi�i hasar

    [Header("Special Properties")]
    public int bulletPierce = 0;        // Merminin ka� hedefe kadar delip ge�ebilece�i
    public float bulletLifetime = 5f;  // Merminin �mr� (sahnede ne kadar kalacak)
}