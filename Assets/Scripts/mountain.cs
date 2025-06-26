using UnityEngine;

public class Mountain : Obstacle // Engel sýnýfýndan türetme
{
    // Dað özel özellikleri ve fonksiyonlarý burada tanýmlanabilir

    // Engel sýnýfýndan InitializeObstacle fonksiyonunu override edebiliriz
    protected override void InitializeObstacle()
    {
        base.InitializeObstacle(); // Temel sýnýfýn baþlatýcýsýný çaðýr
        // Daða özel baþlatma kodlarý burada yer alabilir
        // Örneðin, dað boyutunu ayarlama
        size = new Vector2(15, 15); // Daðýn boyutunu 15x15 olarak ayarla
    }

    public float targetWidth = 15f;
    public float targetHeight = 15f;

    void Start()
    {
        // SpriteRenderer'dan sprite'ý al
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Sprite'ýn boyutunu al
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;

        // Hedef ölçek faktörünü hesapla
        Vector3 scaleFactor = new Vector3(targetWidth / spriteWidth, targetHeight / spriteHeight, 1f);

        // GameObject'in ölçeðini ayarla
        transform.localScale = scaleFactor;
    }
}
