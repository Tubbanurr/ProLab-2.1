using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
public class Karakter : MonoBehaviour
{
    public int moveSpeed = 1;
    public int ID { get; private set; }
    public string Ad { get; private set; }
    public Lokasyon CurrentLocation { get; private set; }

    // Constructor
    public Karakter(int id, string ad, Lokasyon baslangicLokasyonu)
    {
        ID = id;
        Ad = ad;
        CurrentLocation = baslangicLokasyonu;
    }

    // ID ve Ad i�in Set metotlar�
    public void SetID(int id)
    {
        ID = id;
    }

    public void SetAd(string ad)
    {
        Ad = ad;
    }

    // Karakterin lokasyonunu g�ncelleyen metot
    public void Move(int x, int y)
    {
        // Sadece sa�, sol, yukar� veya a�a�� hareket
        if (x == CurrentLocation.x || y == CurrentLocation.y)
        {
            CurrentLocation.x += x;
            CurrentLocation.y += y;
        }
        else
        {
            Console.WriteLine("�apraz hareket edilemez!");
        }
    }

    public void Initialize(int id, string ad, Lokasyon baslangicLokasyonu)
    {
        ID = id;
        Ad = ad;
        CurrentLocation = baslangicLokasyonu;
        // Karakterin fiziksel konumunu g�ncelle
        transform.position = new Vector3(baslangicLokasyonu.x, baslangicLokasyonu.y, 0);
    }
    // En K�sa Yol hesaplamas� i�in bir metod; bu �rnek, detayl� bir algoritma i�ermez
    // Ger�ek d�nya uygulamas�nda, A* gibi bir algoritma kullanabilirsiniz
    public void EnKisaYol(Lokasyon hedefLokasyon)
    {
        // En k�sa yol hesaplama (burada basit bir placeholder)
        Console.WriteLine($"En k�sa yol hesaplan�yor: ({CurrentLocation.x}, {CurrentLocation.y}) -> ({hedefLokasyon.x}, {hedefLokasyon.y})");
    }

    private List<Vector2Int> path; // Karakterin takip etmesi gereken yol
    private int currentPathIndex = 0; // Karakterin �u anki hedef konumu index'i
    private int stepCount = 0; // Karakterin att��� ad�m say�s�
    private int treasureCount = 0; // Karakterin toplad��� hazine say�s�

    public void Initialize(List<Vector2Int> path)
    {
        this.path = path;
        transform.position = new Vector3(path[0].x, path[0].y, 0);
    }

    void Update()
    {
        // Karakter hedef konuma ula�t� m� kontrol et
        if (transform.position == new Vector3(path[currentPathIndex].x, path[currentPathIndex].y, 0))
        {
            // Hedef konuma ula��ld�ysa
            if (currentPathIndex < path.Count - 1)
            {
                // Bir sonraki hedef konumu belirle
                currentPathIndex++;
                stepCount++;
                // Hazine sand��� var m� kontrol et
                GameObject treasure = GetTreasureAtCurrentPosition();
                if (treasure != null)
                {
                    // Hazine sand��� varsa topla
                    CollectTreasure(treasure);
                }
            }
        }
        else
        {
            // Hedefe ula�mad�ysa, hedefe do�ru hareket et
            MoveToNextPosition();
        }
    }

    void MoveToNextPosition()
    {
        // Hedef konuma do�ru hareket et
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[currentPathIndex].x, path[currentPathIndex].y, 0), Time.deltaTime * moveSpeed);
    }

    GameObject GetTreasureAtCurrentPosition()
    {
        // E�er karakterin oldu�u konumda bir hazine varsa, onu d�nd�r
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
        if (hit.collider != null && hit.collider.CompareTag("Treasure"))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    void CollectTreasure(GameObject treasure)
    {
        // Hazineyi topla
        Destroy(treasure);
        treasureCount++;
        // Toplanan hazineyi ekrana yazd�r
        Debug.Log("Hazine Topland�! Toplam Hazine Say�s�: " + treasureCount);
    }
}

