using System;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("General Attributes")]
    public string enemyName;            // D��man�n ad�
    public int health = 100;            // Can miktar�
    public bool isTurn = false;
    [Header("Elemental Properties")]
    public Element resistances;         // Diren�li oldu�u elementler (tiklenebilir)
    public Element weaknesses;          // Zay�f oldu�u elementler (tiklenebilir)

    [Header("Behavior Settings")]
    public float aggroRange = 10f;      // Oyuncuyu alg�layaca�� mesafe
    public float waitingTime = 2f;            // Hareket h�z�
    

    [Header("Attack Settings")]
    public float shootRange = 15f;      // Sald�r� mesafesi
    public float shootFrequency = 1f;  // Sald�r� s�kl��� (at��lar aras�ndaki s�re)
    [Range(0f, 1f)] public float shootAccuracy = 0.8f; // At�� do�rulu�u (0-1 aras�nda)
    public GameObject bulletPrefab;     // Kullan�lacak merminin GameObject prefab referans�
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
