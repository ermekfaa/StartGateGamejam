using System;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("General Attributes")]
    public string enemyName;            // Düþmanýn adý
    public int health = 100;            // Can miktarý
    public bool isTurn = false;
    [Header("Elemental Properties")]
    public Element resistances;         // Dirençli olduðu elementler (tiklenebilir)
    public Element weaknesses;          // Zayýf olduðu elementler (tiklenebilir)

    [Header("Behavior Settings")]
    public float aggroRange = 10f;      // Oyuncuyu algýlayacaðý mesafe
    public float waitingTime = 2f;            // Hareket hýzý
    

    [Header("Attack Settings")]
    public float shootRange = 15f;      // Saldýrý mesafesi
    public float shootFrequency = 1f;  // Saldýrý sýklýðý (atýþlar arasýndaki süre)
    [Range(0f, 1f)] public float shootAccuracy = 0.8f; // Atýþ doðruluðu (0-1 arasýnda)
    public GameObject bulletPrefab;     // Kullanýlacak merminin GameObject prefab referansý
}

[Flags]
public enum Element
{
    None = 0,
    Fire = 1 << 0,       // 1
    Water = 1 << 1,      // 2
    Earth = 1 << 2,      // 4
    Wind = 1 << 3,       // 8
    Electricity = 1 << 4 // 16
}
