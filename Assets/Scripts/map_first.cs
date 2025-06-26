using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MapGenerator : MonoBehaviour
{
    public GameObject winter; // K�� temas� i�in prefab
    public GameObject summer; // Yaz temas� i�in prefab
    public int width = 20; // Haritan�n geni�li�i
    public int height = 20; // Haritan�n y�ksekli�i
    public int mountainCount = 1; // Olu�turulacak da� say�s�
    public int mountainAreaSize = 10; // Da�lar�n spawn edilece�i alan�n boyutu (15x15)
    public GameObject mountainPrefabWinter; // K�� da� prefab'�
    public GameObject mountainPrefabSummer; // Yaz da� prefab'�
    public GameObject rockPrefabWinter; // K�� kaya prefab'�
    public GameObject rockPrefabSummer; // Yaz kaya prefab'�
    public int rockCount = 2;
    public int minTreeSize = 2; // Minimum a�a� boyutu
    public int maxTreeSize = 5; // Maksimum a�a� boyutu
    public int treeCount = 2; // Olu�turulacak a�a� say�s�
    public GameObject treePrefabSummer; // Yaz a�ac� prefab'�
    public GameObject treePrefabWinter; // K�� a�ac� prefab'�
    public int wallCount = 5;
    public GameObject wallPrefabWinter; // K�� duvar prefab'�
    public GameObject wallPrefabSummer; // Yaz duvar prefab'�
    public int birdCount = 10;
    public GameObject winterBirdPrefab; // K�� ku�u prefab�
    public GameObject summerBirdPrefab; // Yaz ku�u prefab�
    public GameObject redPathPrefab; // K�rm�z� yol prefab'�
    public GameObject yellowPathPrefab; // sar� yol prefab'�
    public GameObject winterBeePrefab; // K�� ar�s� prefab'�n�z� buraya s�r�kleyin.
    public GameObject summerBeePrefab; // Yaz ar�s� prefab'�n�z� buraya s�r�kleyin.
    public int beeCount = 10;
    public GameObject characterPrefab;
    private int[,] mapMatrix; // Harita matrisi
    public int treasureChestCount = 2;
    public GameObject treasureChestPrefab;
    public GameObject goldTreasureChestPrefabSummer; // Alt�n sand�k yaz prefab�
    public GameObject goldTreasureChestPrefabWinter; // Alt�n sand�k k�� prefab�
    public GameObject silverTreasureChestPrefabSummer; // G�m�� sand�k yaz prefab�
    public GameObject silverTreasureChestPrefabWinter; // G�m�� sand�k k�� prefab�
    public GameObject bronzeTreasureChestPrefabSummer; // Bronz sand�k yaz prefab�
    public GameObject bronzeTreasureChestPrefabWinter; // Bronz sand�k k�� prefab�
    public GameObject emeraldTreasureChestPrefabSummer; // Bronz sand�k yaz prefab�
    public GameObject emeraldTreasureChestPrefabWinter; // Bronz sand�k k�� prefab�
    public int goldChestCount = 1; // Alt�n sand�k say�s�
    public int silverChestCount = 1; // G�m�� sand�k say�s�
    public int bronzeChestCount = 1; // Bronz sand�k say�s�
    public int emeraldChestCount = 1;
    public int konumx, konumy;
    public Vector2Int characterPosition;
    public GameObject characterGO;
    public int viewDistance = 3;
    private GameObject characterInstance;
    private int totalChestCount; // Toplam sand�k say�s� (hedef)
    private int discoveredChestCount = 0;
    private Vector2Int chestPosition;
    List<Vector2Int> possibleMoves = new List<Vector2Int> {
        new Vector2Int(1, 0),   // Sa�a
        new Vector2Int(-1, 0),  // Sola
        new Vector2Int(0, 1),   // Yukar�
        new Vector2Int(0, -1)   // A�a��
    };


    void Start()
    {
        mapMatrix = new int[width, height]; // Harita matrisini ba�lat
        InitializeMapMatrix(); // Matrisi ba�lang�� de�erleriyle doldur
        totalChestCount = goldChestCount + silverChestCount + bronzeChestCount + emeraldChestCount;
        GenerateMap();
        GenerateMountains();
        GenerateTrees();
        GenerateBirds();
        GenerateBees();
        GenerateRocks();
        GenerateWalls();
        GenerateCharacter();
        CreatePathToTreasure(); // Yolu olu�tur
        PlaceTreasureAlongThePath();
        GenerateFog();
        //ClearFogAroundCharacter();
        Camera.main.orthographicSize = height / 2;
        Camera.main.transform.position = new Vector3(width / 2, height / 2, -10);
    }

    void GenerateCharacter()
    {
        // Karakterin rastgele ba�lang�� konumunu belirle
        int startX = Random.Range(0, width);
        int startY = Random.Range(0, height);
        characterPosition = new Vector2Int(startX, startY); // Karakterin ba�lang�� konumunu g�ncelle

        Vector2Int treasurePosition = new Vector2Int(width - 1, height - 1);

        // Yolu bul ve liste olarak kaydet
        List<Vector2Int> path = new List<Vector2Int>();
        Queue<Vector2Int> toVisit = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        toVisit.Enqueue(characterPosition);
        visited.Add(characterPosition);

        while (toVisit.Count > 0)
        {
            Vector2Int currentPos = toVisit.Dequeue();
            if (currentPos == treasurePosition)
            {
                break; // Hedefe ula��ld�
            }

            List<Vector2Int> neighbours = GetNeighbours(currentPos, width, height);
            foreach (var neighbour in neighbours)
            {
                if (!visited.Contains(neighbour) && mapMatrix[neighbour.x, neighbour.y] == -1)
                {
                    toVisit.Enqueue(neighbour);
                    visited.Add(neighbour);
                    path.Add(neighbour);
                }
            }
        }

        // Karakterin ba�lang�� konumunu matriste 10 olarak i�aretle
        mapMatrix[characterPosition.x, characterPosition.y] = 10;

        // Karakteri sahnede g�rsel olarak olu�tur
        characterGO = Instantiate(characterPrefab, new Vector3(characterPosition.x, characterPosition.y, 0), Quaternion.identity);

        Karakter karakterComponent = characterGO.AddComponent<Karakter>();
        karakterComponent.Initialize(path);
    }


    void GenerateBirds()
    {
        for (int i = 0; i < birdCount; i++)
        {
            float birdX = Random.Range(0, width);
            float birdY = Random.Range(0, height);
            Vector3 birdPosition = new Vector3(birdX, birdY, -1);

            GameObject selectedPrefab = birdX < width / 2 ? winterBirdPrefab : summerBirdPrefab;
            GameObject birdGO = Instantiate(selectedPrefab, birdPosition, Quaternion.identity);

            // Bird s�n�f�ndan t�retiyoruz.
            Bird birdComponent = birdGO.AddComponent<Bird>();
            

            // Hareket alan�n� ve k�rm�z� yolu olu�tur
            for (int y = (int)birdY - 5; y <= (int)birdY + 5; y++)
            {
                if (y >= 0 && y < height)
                {
                    Instantiate(redPathPrefab, new Vector3(birdX, y, 0), Quaternion.identity);
                    mapMatrix[(int)birdX, y] = 6; // Matriste ku�un hareket edebilece�i alan� i�aretle
                }
            }
        }
    }

    void GenerateBees()
    {
        for (int i = 0; i < beeCount; i++)
        {
            float beeX = Random.Range(0, width);
            float beeY = Random.Range(0, height);
            Vector3 beePosition = new Vector3(beeX, beeY, -1);

            GameObject selectedPrefab = beeX < width / 2 ? winterBeePrefab : summerBeePrefab;
            GameObject beeGO = Instantiate(selectedPrefab, beePosition, Quaternion.identity);
            beeGO.transform.localScale = new Vector3(2, 2, 1); 

            Bee beeComponent = beeGO.AddComponent<Bee>(); 

            // Ar�n�n hareket alan�n� ve sar� yolu olu�tur (Sa�-sol hareket i�in)
            for (int x = (int)beeX - 3; x <= (int)beeX + 3; x++)
            {
                if (x >= 0 && x < width)
                {
                    Instantiate(yellowPathPrefab, new Vector3(x, beeY, 0), Quaternion.identity);
                    mapMatrix[x, (int)beeY] = 7; 
                }
            }
        }
    }

    void GenerateWalls()
    {
        int attemptsToPlaceWall = 0;

        for (int i = 0; i < wallCount && attemptsToPlaceWall < 100; i++)
        {
            bool validPositionFound = false;

            while (!validPositionFound && attemptsToPlaceWall < 100)
            {
                validPositionFound = true;
                float wallX = Random.Range(0, width - 10); 
                float wallY = Random.Range(0, height);

                // Se�ilen konumun ve �evresinin kontrol�
                for (int xOffset = -1; xOffset <= 10; xOffset++)
                {
                    for (int yOffset = -1; yOffset <= 1; yOffset++)
                    {
                        int checkX = Mathf.RoundToInt(wallX) + xOffset;
                        int checkY = Mathf.RoundToInt(wallY) + yOffset;

                        // Harita s�n�rlar�n� kontrol et
                        if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                        {
                            if (mapMatrix[checkX, checkY] != -1) 
                            {
                                validPositionFound = false;
                                break;
                            }
                        }
                    }
                    if (!validPositionFound) break;
                }

                if (!validPositionFound)
                {
                    attemptsToPlaceWall++;
                    continue;
                }

                // Duvar prefab�n� se� ve yerle�tir
                GameObject selectedPrefab = wallX < width / 2 ? wallPrefabWinter : wallPrefabSummer;

                for (int x = 0; x < 10; x++)
                {
                    Vector3 wallPartPosition = new Vector3(wallX + x, wallY, 0);
                    GameObject wallGO = Instantiate(selectedPrefab, wallPartPosition, Quaternion.identity, transform);

                    
                    Wall wallComponent = wallGO.AddComponent<Wall>();
                    wallComponent.length = 10f; 

                    int matrixX = Mathf.RoundToInt(wallX) + x;
                    int matrixY = Mathf.RoundToInt(wallY);
                    mapMatrix[matrixX, matrixY] = 4; 
                }
            }
        }

        if (attemptsToPlaceWall >= 100)
        {
            Debug.LogWarning("Failed to place all walls without adjacency after 100 attempts.");
        }
    }

    void GenerateTrees()
    {
        for (int i = 0; i < treeCount; i++)
        {
            // A�ac�n boyutunu rastgele belirle
            float treeScaleFactor = Random.Range(minTreeSize, maxTreeSize + 1);

            // A�ac�n konumunu rastgele belirle
            float treeX = Random.Range(0, width - treeScaleFactor);
            float treeY = Random.Range(0, height - treeScaleFactor);
            Vector3 treePosition = new Vector3(treeX, treeY, 0);

            GameObject selectedPrefab = treeX < width / 2 ? treePrefabWinter : treePrefabSummer;

            // Se�ilen a�a� prefab�n� olu�tur ve konumland�r
            GameObject treeGO = Instantiate(selectedPrefab, treePosition, Quaternion.identity, transform);
            treeGO.transform.localScale = new Vector3(treeScaleFactor, treeScaleFactor, 1); // A�ac�n boyutunu ayarla

            Tree treeComponent = treeGO.AddComponent<Tree>();

            int scaledSizeX = Mathf.CeilToInt(treeScaleFactor);
            int scaledSizeY = Mathf.CeilToInt(treeScaleFactor);
            for (int x = 0; x < scaledSizeX; x++)
            {
                for (int y = 0; y < scaledSizeY; y++)
                {
                    int matrixX = Mathf.RoundToInt(treePosition.x) + x;
                    int matrixY = Mathf.RoundToInt(treePosition.y) + y;
                    if (matrixX >= 0 && matrixX < width && matrixY >= 0 && matrixY < height)
                    {
                        mapMatrix[matrixX, matrixY] = 1; // A�a�lar�n bulundu�u yerdeki de�eri 1 olarak g�ncelle
                    }
                }
            }
        }
    }

    void InitializeMapMatrix()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                mapMatrix[x, y] = -1; // Engelsiz alanlar� -1 ile ba�lat
            }
        }
    }
    void GenerateMountains()
    {
        for (int i = 0; i < 1; i++) 
        {
            float mountainScaleFactor = 15;

            float mountainX = Random.Range(0, width - mountainScaleFactor);
            float mountainY = Random.Range(0, height - mountainScaleFactor);
            Vector3 mountainPosition = new Vector3(mountainX, mountainY, 0);

            GameObject selectedPrefab = mountainX < width / 2 ? mountainPrefabWinter : mountainPrefabSummer;
            GameObject mountainGO = Instantiate(selectedPrefab, mountainPosition, Quaternion.identity, transform);
            mountainGO.transform.localScale = new Vector3(mountainScaleFactor, mountainScaleFactor, 1);

            Mountain mountainComponent = mountainGO.AddComponent<Mountain>();
            // Mountain bile�eninin �zelliklerini ayarla
            mountainComponent.targetWidth = mountainScaleFactor;
            mountainComponent.targetHeight = mountainScaleFactor;

            // Matriste da��n kaplad��� alan� g�ncelle
            int scaledSizeX = Mathf.CeilToInt(mountainScaleFactor);
            int scaledSizeY = Mathf.CeilToInt(mountainScaleFactor);
            for (int x = 0; x < scaledSizeX; x++)
            {
                for (int y = 0; y < scaledSizeY; y++)
                {
                    int matrixX = Mathf.RoundToInt(mountainPosition.x) + x;
                    int matrixY = Mathf.RoundToInt(mountainPosition.y) + y;
                    if (matrixX >= 0 && matrixX < width && matrixY >= 0 && matrixY < height)
                    {
                        mapMatrix[matrixX, matrixY] = 2; // Da�lar�n bulundu�u yerdeki de�eri 2 olarak g�ncelle
                    }
                }
            }
        }
    }

    void GenerateRocks()
    {
        for (int i = 0; i < rockCount; i++)
        {
            // Kaya boyutunu rastgele belirle
            float rockSize = Random.Range(2, 4); // 2 veya 3

            
            float rockX = Random.Range(0, width - rockSize);
            float rockY = Random.Range(0, height - rockSize);
            Vector3 rockPosition = new Vector3(rockX, rockY, 0);

            
            GameObject selectedPrefab = rockX < width / 2 ? rockPrefabWinter : rockPrefabSummer;

            
            GameObject rockGO = Instantiate(selectedPrefab, rockPosition, Quaternion.identity, transform);

          
            Rock rockComponent = rockGO.AddComponent<Rock>();
            rockComponent.size = rockSize; // Kaya boyutunu rastgele se�ilen de�ere ayarla

            // Matriste kayan�n kaplad��� alan� g�ncelle
            for (int x = 0; x < rockSize; x++)
            {
                for (int y = 0; y < rockSize; y++)
                {
                    int matrixX = Mathf.RoundToInt(rockPosition.x) + x;
                    int matrixY = Mathf.RoundToInt(rockPosition.y) + y;
                    if (matrixX >= 0 && matrixX < width && matrixY >= 0 && matrixY < height)
                    {
                        mapMatrix[matrixX, matrixY] = 3; // Kayalar�n bulundu�u yerdeki de�eri 3 olarak g�ncelle
                    }
                }
            }
        }
    }

    public GameObject greenPrefab; // Ye�il prefab

    void FindShortestPathToChests(List<Vector2Int> chests, Vector2Int characterStartPosition)
    {
        // BFS algoritmas� i�in kullan�lacak kuyruk veri yap�s�
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(characterStartPosition);

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        visited.Add(characterStartPosition);

 
        int[,] greenPathMatrix = new int[width, height];

        // BFS algoritmas�
        while (queue.Count > 0)
        {
            Vector2Int currentPosition = queue.Dequeue();

            if (chests.Contains(currentPosition))
                break;

       

            foreach (var move in possibleMoves)
            {
                Vector2Int newPosition = currentPosition + move;

                if (CanMove(newPosition) && !visited.Contains(newPosition))
                {
                    queue.Enqueue(newPosition);
                    visited.Add(newPosition);

                    // Ye�il yolu i�aretle
                    greenPathMatrix[newPosition.x, newPosition.y] = 1;
                }
            }
        }

        // Matristeki ye�il yollar� g�ncelle
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (greenPathMatrix[x, y] == 1)
                {
                    // Ye�il prefab'� ile yol
                    Vector3 tilePosition = new Vector3(x, y, 0);
                    Instantiate(greenPrefab, tilePosition, Quaternion.identity);
                }
            }
        }
    }
    void CreatePathToTreasure()
    {
        Vector2Int characterStart = new Vector2Int(0, 0); 
        Vector2Int treasurePosition = new Vector2Int(width - 1, height - 1); // Hazine sand���n�n konumu varsay�lan de�er

        Queue<Vector2Int> toVisit = new Queue<Vector2Int>(); // Ziyaret edilecek konumlar� tutan queue
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[characterStart] = characterStart; 

        // Ba�lang�� noktas�n� kuyru�a ekle
        toVisit.Enqueue(characterStart);

        while (toVisit.Count > 0)
        {
            Vector2Int currentPos = toVisit.Dequeue();

            // Hazine sand���na ula��ld� m� kontrol et
            if (currentPos == treasurePosition)
            {
                break; // Hedefe ula��ld�
            }

            // Kom�u konumlar� kontrol et
            List<Vector2Int> neighbours = GetNeighbours(currentPos, width, height);
            foreach (var neighbour in neighbours)
            {
                if (!cameFrom.ContainsKey(neighbour) && mapMatrix[neighbour.x, neighbour.y] == -1)
                {
                    toVisit.Enqueue(neighbour);
                    cameFrom[neighbour] = currentPos; 
                    mapMatrix[neighbour.x, neighbour.y] = 0; 
                }
            }
        }

        // Yolu yeniden olu�tur
        Vector2Int current = treasurePosition;
        Stack<Vector2Int> path = new Stack<Vector2Int>();
        while (current != characterStart)
        {
            path.Push(current);
            current = cameFrom[current];
        }
     
    }

    List<Vector2Int> GetNeighbours(Vector2Int pos, int width, int height)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        Vector2Int[] possibleMoves = {
        new Vector2Int(1, 0), // sa�a
        new Vector2Int(0, 1), // yukar�
        new Vector2Int(-1, 0), // sola
        new Vector2Int(0, -1) // a�a��
    };

        foreach (Vector2Int move in possibleMoves)
        {
            Vector2Int newPos = pos + move;
            if (newPos.x >= 0 && newPos.x < width && newPos.y >= 0 && newPos.y < height)
            {
                neighbours.Add(newPos);
            }
        }

        return neighbours;
    }

    //void HighlightShortestPathToChests()
    //{
    //    // BFS algoritmas� i�in bir kuyruk olu�tur
    //    Queue<Vector2Int> queue = new Queue<Vector2Int>();

    //    // Karakterin konumunu kuyru�a ekle
    //    queue.Enqueue(characterPosition);

    //    // En k�sa yol bulundu�unda bu bool de�eri true olacak
    //    bool shortestPathFound = false;

    //    while (queue.Count > 0 && !shortestPathFound)
    //    {
    //        // Kuyruktan bir pozisyon al
    //        Vector2Int currentPosition = queue.Dequeue();

    //        // Mevcut pozisyonu i�aretleyerek tekrar ziyaret etmemek i�in 'visited' k�mesine ekle
    //        visited.Add(currentPosition);

    //        // E�er mevcut pozisyon sand�k konumlar�ndan biriyse, en k�sa yol bulundu demektir.
    //        if (IsChest(currentPosition))
    //        {
    //            shortestPathFound = true;
    //            break;
    //        }

    //        // Mevcut pozisyonun kom�u h�crelerini kontrol et
    //        foreach (var move in possibleMoves)
    //        {
    //            Vector2Int nextPosition = currentPosition + move;

    //            // E�er bir sonraki pozisyon ge�erliyse ve daha �nce ziyaret edilmediyse
    //            if (CanMove(nextPosition) && !visited.Contains(nextPosition))
    //            {
    //                // Bir sonraki pozisyonu kuyru�a ekle
    //                queue.Enqueue(nextPosition);

    //                // Matriste en k�sa yolu g�stermek i�in renklendirme yap
    //                ColorPathTile(nextPosition);
    //            }
    //        }
    //    }
    //}

    bool IsChest(Vector2Int position)
    {
        // Verilen pozisyonun bir sand�k pozisyonu olup olmad���n� kontrol et
        return mapMatrix[position.x, position.y] == 19 || mapMatrix[position.x, position.y] == 20 || mapMatrix[position.x, position.y] == 21 || mapMatrix[position.x, position.y] == 22;
    }

    void ColorPathTile(Vector2Int position)
    {
        // Verilen pozisyonun matrisindeki de�erini g�ncelle ve rengini ye�il yap
        mapMatrix[position.x, position.y] = 3; 
        GameObject tile = GameObject.Find($"Fog_{position.x}_{position.y}");
        tile.GetComponent<SpriteRenderer>().color = Color.green;
    }
    void PlaceChestsAlongPath(int chestCount, GameObject chestPrefabSummer, GameObject chestPrefabWinter, List<Vector2Int> path, int chestTypeIdentifier)
    {
        for (int i = 0; i < chestCount; i++)
        {
            int pathIndex = Random.Range(0, path.Count);
            Vector2Int chestLocation = path[pathIndex];
            path.RemoveAt(pathIndex); // Sand�k yerle�tirildikten sonra, ayn� konuma ba�ka bir sand�k yerle�tirilmesini �nlemek i�in

            GameObject selectedPrefab = chestLocation.x < width / 2 ? chestPrefabWinter : chestPrefabSummer;
            Instantiate(selectedPrefab, new Vector3(chestLocation.x, chestLocation.y, 0), Quaternion.identity);

            // Sand���n matristeki konumunu g�ncelle
            mapMatrix[chestLocation.x, chestLocation.y] = chestTypeIdentifier;
        }
    }
    void PlaceTreasureAlongThePath()
    {
        List<Vector2Int> path = new List<Vector2Int>();

        // Yolu bul ve liste olarak kaydet
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapMatrix[x, y] == 0) // Yolu temsil eden s�f�rlar� ara
                {
                    path.Add(new Vector2Int(x, y));
                }
            }
        }

        // Her sand�k t�r� i�in belirlenen say�da sand��� yerle�tir ve konumunu g�ncelle
        PlaceChestsAlongPath(goldChestCount, goldTreasureChestPrefabSummer, goldTreasureChestPrefabWinter, path, 22); // Alt�n i�in 22
        PlaceChestsAlongPath(silverChestCount, silverTreasureChestPrefabSummer, silverTreasureChestPrefabWinter, path, 21); // G�m�� i�in 21
        PlaceChestsAlongPath(bronzeChestCount, bronzeTreasureChestPrefabSummer, bronzeTreasureChestPrefabWinter, path, 19); // Bronz i�in 20
        PlaceChestsAlongPath(emeraldChestCount, emeraldTreasureChestPrefabSummer, emeraldTreasureChestPrefabWinter, path, 20); // Z�mr�t i�in 19
    }
    public GameObject fogPrefab; // Sis prefab'� i�in bir referans
    public void GenerateFog()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 tilePosition = new Vector3(x, y, 0);
                GameObject fog = Instantiate(fogPrefab, tilePosition, Quaternion.identity, transform);
                fog.name = $"Fog_{x}_{y}"; // Her sis objesine konumunu temsil eden bir isim ver
            }
        }
    }

    void Update()
    {
        // Karakterin hedefe do�ru hareketini sa�la.
        MoveCharacter();

        // Karakterin �evresindeki sisi temizle.
        ClearFogAroundCharacter();
    }

    HashSet<Vector2Int> visited = new HashSet<Vector2Int>(); // Ziyaret edilen noktalar� takip eden k�me.
    
    // Sand��a gitmek i�in BFS kullanarak en k�sa yolu bulan fonksiyon.
    //void MoveToChest()
    //{
    //    Queue<Vector2Int> queue = new Queue<Vector2Int>();
    //    Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();

    //    queue.Enqueue(characterPosition);
    //    visited.Add(characterPosition);

    //    while (queue.Count > 0)
    //    {
    //        Vector2Int current = queue.Dequeue();

    //        if (current == chestPosition)
    //            break;

    //        foreach (Vector2Int move in possibleMoves)
    //        {
    //            Vector2Int newPosition = current + move;

    //            if (CanMove(newPosition) && !visited.Contains(newPosition))
    //            {
    //                queue.Enqueue(newPosition);
    //                visited.Add(newPosition);
    //                parent[newPosition] = current;
    //            }
    //        }
    //    }

    //    // En k�sa yolu i�aretle.
    //    Vector2Int traceBack = chestPosition;
    //    while (traceBack != characterPosition)
    //    {
    //        MarkTile(traceBack, Color.green);
    //        traceBack = parent[traceBack];
    //    }
    //}

    // Karakterin yeni pozisyonunu ayarlayan fonksiyon.
    void SetCharacterPosition(Vector2Int newPosition)
    {
        
        MarkTile(characterPosition, Color.white);

        characterPosition = newPosition;
        characterGO.transform.position = new Vector3(newPosition.x, newPosition.y, 0);
        MarkTile(characterPosition, Color.green);
    }


    void MarkTile(Vector2Int position, Color color)
    {
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tile.transform.position = new Vector3(position.x, 0, position.y);
        tile.GetComponent<Renderer>().material.color = color;
    }
    void MoveCharacter()
    {
        List<Vector2Int> validMoves = new List<Vector2Int>();

        // Ge�erli ve m�mk�n olan hareketleri bul.
        foreach (var move in possibleMoves)
        {
            Vector2Int newPosition = characterPosition + move;
            if (CanMove(newPosition) && !visited.Contains(newPosition))
            {
                validMoves.Add(move);
            }
        }

        // E�er hi�bir ge�erli hareket yoksa, karakterin �nceki hareketleri s�f�rla.
        if (validMoves.Count == 0)
        {
            visited.Clear();
            validMoves = possibleMoves.Where(move => CanMove(characterPosition + move)).ToList();
        }

        // Rastgele bir ge�erli hareket se�.
        int randIndex = Random.Range(0, validMoves.Count);
        Vector2Int nextMove = validMoves[randIndex];

        // Karakterin yeni pozisyonunu hesapla ve g�ncelle.
        SetCharacterPosition(characterPosition + nextMove);

        // Hareket edilen pozisyonu 'visited' k�mesine ekle.
        visited.Add(characterPosition);
    }

    void CoverPathWithPrefab(List<Vector2Int> path, GameObject prefab)
    {
        foreach (var position in path)
        {
            Instantiate(prefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        }
    }
    bool CanMove(Vector2Int position)
    {
        // Pozisyonun harita s�n�rlar� i�inde olup olmad���n� ve bo� bir alan olup olmad���n� kontrol et.
        return position.x >= 0 && position.x < width && position.y >= 0 && position.y < height && mapMatrix[position.x, position.y] == 0;
    }


    //void SetCharacterPosition(Vector2Int newPosition)
    //{
    //    // Karakterin matristeki ve sahnedeki konumunu g�ncelle
    //    mapMatrix[characterPosition.x, characterPosition.y] = 0; // eski pozisyonu temizle
    //    characterPosition = newPosition;
    //    mapMatrix[newPosition.x, newPosition.y] = 10; // yeni pozisyonu i�aretle
    //    characterGO.transform.position = new Vector3(newPosition.x, newPosition.y, 0);
    //}

    void ClearFogAroundCharacter()
    {
        // Karakterin �evresindeki sisi temizle.
        for (int x = characterPosition.x - viewDistance; x <= characterPosition.x + viewDistance; x++)
        {
            for (int y = characterPosition.y - viewDistance; y <= characterPosition.y + viewDistance; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    GameObject fog = GameObject.Find($"Fog_{x}_{y}");
                    if (fog != null)
                    {
                        Destroy(fog); // Sisi yok et.
                                      // Burada yeni ekledi�imiz k�s�m ba�l�yor.
                        ReportDiscoveredObjects(x, y); // Ke�fedilen nesneleri raporla.
                    }
                }
            }
        }
    }


    void ReportDiscoveredObjects(int x, int y)
    {
        // mapMatrix'teki de�erlere g�re nesne t�r�n� kontrol et ve raporla.
        switch (mapMatrix[x, y])
        {
            case 1:
                Debug.Log("A�a� ke�fedildi.");
                break;
            case 2:
                Debug.Log("Da� ke�fedildi.");
                break;
            case 3:
                Debug.Log("Kaya ke�fedildi.");
                break;
            case 4:
                Debug.Log("Duvar ke�fedildi.");
                break;
            case 22:
                Debug.Log("Alt�n Sand�k ke�fedildi.");
                break;
            case 21:
                Debug.Log("G�m�� Sand�k ke�fedildi.");
                break;
            case 20:
                Debug.Log("Z�mr�t Sand�k ke�fedildi.");
                break;
            case 19:
                Debug.Log("Bronz Sand�k ke�fedildi.");
                break;
            
        
        
        }

        if (mapMatrix[x, y] == 22 || mapMatrix[x, y] == 21 || mapMatrix[x, y] == 20 || mapMatrix[x, y] == 19)
        {
            discoveredChestCount++; // Ke�fedilen sand�k say�s�n� art�r
            Debug.Log("Sand�k ke�fedildi. Toplam ke�fedilen sand�k say�s�: " + discoveredChestCount);

            // Sand�k ke�fedildikten sonra, tekrar ke�fedilmemesi i�in bu konumdaki de�eri g�ncelleyin
            mapMatrix[x, y] = 50; // Sand�k ke�fedildi olarak i�aretle (�rne�in, 5 de�eri sand�k ke�fedildi anlam�na gelsin)
        }
    }



    void GenerateMap()
    {
        // Haritay� olu�tur
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Her bir karo i�in pozisyon hesapla
                Vector3 tilePosition = new Vector3(x, y, 0);
                // X koordinat�na g�re k�� veya yaz prefab'�n� se�
                GameObject selectedPrefab = x < width / 2 ? winter : summer;
                Instantiate(selectedPrefab, tilePosition, Quaternion.identity, transform);
            }
        }
    }
}