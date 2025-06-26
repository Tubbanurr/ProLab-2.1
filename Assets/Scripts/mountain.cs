using UnityEngine;

public class Mountain : Obstacle // Engel s�n�f�ndan t�retme
{
    // Da� �zel �zellikleri ve fonksiyonlar� burada tan�mlanabilir

    // Engel s�n�f�ndan InitializeObstacle fonksiyonunu override edebiliriz
    protected override void InitializeObstacle()
    {
        base.InitializeObstacle(); // Temel s�n�f�n ba�lat�c�s�n� �a��r
        // Da�a �zel ba�latma kodlar� burada yer alabilir
        // �rne�in, da� boyutunu ayarlama
        size = new Vector2(15, 15); // Da��n boyutunu 15x15 olarak ayarla
    }

    public float targetWidth = 15f;
    public float targetHeight = 15f;

    void Start()
    {
        // SpriteRenderer'dan sprite'� al
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Sprite'�n boyutunu al
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;

        // Hedef �l�ek fakt�r�n� hesapla
        Vector3 scaleFactor = new Vector3(targetWidth / spriteWidth, targetHeight / spriteHeight, 1f);

        // GameObject'in �l�e�ini ayarla
        transform.localScale = scaleFactor;
    }
}
