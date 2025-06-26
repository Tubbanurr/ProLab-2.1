using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MapGenerator : MonoBehaviour
{
    public GameObject winter; // Kýþ temasý için prefab
    public GameObject summer; // Yaz temasý için prefab
    public int width = 20; // Haritanýn geniþliði
    public int height = 20; // Haritanýn yüksekliði
    public int mountainCount = 1; // Oluþturulacak dað sayýsý
    public int mountainAreaSize = 10; // Daðlarýn spawn edileceði alanýn boyutu (15x15)
    public GameObject mountainPrefabWinter; // Kýþ dað prefab'ý
    public GameObject mountainPrefabSummer; // Yaz dað prefab'ý
    public GameObject rockPrefabWinter; // Kýþ kaya prefab'ý
    public GameObject rockPrefabSummer; // Yaz kaya prefab'ý
    public int rockCount = 2;
    public int minTreeSize = 2; // Minimum aðaç boyutu
    public int maxTreeSize = 5; // Maksimum aðaç boyutu
    public int treeCount = 2; // Oluþturulacak aðaç sayýsý
    public GameObject treePrefabSummer; // Yaz aðacý prefab'ý
    public GameObject treePrefabWinter; // Kýþ aðacý prefab'ý
    public int wallCount = 5;
    public GameObject wallPrefabWinter; // Kýþ duvar prefab'ý
    public GameObject wallPrefabSummer; // Yaz duvar prefab'ý
    public int birdCount = 10;
    public GameObject winterBirdPrefab; // Kýþ kuþu prefabý
    public GameObject summerBirdPrefab; // Yaz kuþu prefabý
    public GameObject redPathPrefab; // Kýrmýzý yol prefab'ý
    public GameObject yellowPathPrefab; // sarý yol prefab'ý
    public GameObject winterBeePrefab; // Kýþ arýsý prefab'ýnýzý buraya sürükleyin.
    public GameObject summerBeePrefab; // Yaz arýsý prefab'ýnýzý buraya sürükleyin.
    public int beeCount = 10;
    public GameObject characterPrefab;
    private int[,] mapMatrix; // Harita matrisi
    public int treasureChestCount = 2;
    public GameObject treasureChestPrefab;
    public GameObject goldTreasureChestPrefabSummer; // Altýn sandýk yaz prefabý
    public GameObject goldTreasureChestPrefabWinter; // Altýn sandýk kýþ prefabý
    public GameObject silverTreasureChestPrefabSummer; // Gümüþ sandýk yaz prefabý
    public GameObject silverTreasureChestPrefabWinter; // Gümüþ sandýk kýþ prefabý
    public GameObject bronzeTreasureChestPrefabSummer; // Bronz sandýk yaz prefabý
    public GameObject bronzeTreasureChestPrefabWinter; // Bronz sandýk kýþ prefabý
    public GameObject emeraldTreasureChestPrefabSummer; // Bronz sandýk yaz prefabý
    public GameObject emeraldTreasureChestPrefabWinter; // Bronz sandýk kýþ prefabý
    public int goldChestCount = 1; // Altýn sandýk sayýsý
    public int silverChestCount = 1; // Gümüþ sandýk sayýsý
    public int bronzeChestCount = 1; // Bronz sandýk sayýsý
    public int emeraldChestCount = 1;
    public int konumx, konumy;
    public Vector2Int characterPosition;
    public GameObject characterGO;
    public int viewDistance = 3;
    private GameObject characterInstance;
    private int totalChestCount; // Toplam sandýk sayýsý (hedef)
    private int discoveredChestCount = 0;
    private Vector2Int chestPosition;
    List<Vector2Int> possibleMoves = new List<Vector2Int> {
        new Vector2Int(1, 0),   // Saða
        new Vector2Int(-1, 0),  // Sola
        new Vector2Int(0, 1),   // Yukarý
        new Vector2Int(0, -1)   // Aþaðý
    };


    void Start()
    {
        mapMatrix = new int[width, height]; // Harita matrisini baþlat
        InitializeMapMatrix(); // Matrisi baþlangýç deðerleriyle doldur
        totalChestCount = goldChestCount + silverChestCount + bronzeChestCount + emeraldChestCount;
        GenerateMap();
        GenerateMountains();
        GenerateTrees();
        GenerateBirds();
        GenerateBees();
        GenerateRocks();
        GenerateWalls();
        GenerateCharacter();
        CreatePathToTreasure(); // Yolu oluþtur
        PlaceTreasureAlongThePath();
        GenerateFog();
        //ClearFogAroundCharacter();
        Camera.main.orthographicSize = height / 2;
        Camera.main.transform.position = new Vector3(width / 2, height / 2, -10);
    }

    void GenerateCharacter()
    {
        // Karakterin rastgele baþlangýç konumunu belirle
        int startX = Random.Range(0, width);
        int startY = Random.Range(0, height);
        characterPosition = new Vector2Int(startX, startY); // Karakterin baþlangýç konumunu güncelle

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
                break; // Hedefe ulaþýldý
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

        // Karakterin baþlangýç konumunu matriste 10 olarak iþaretle
        mapMatrix[characterPosition.x, characterPosition.y] = 10;

        // Karakteri sahnede görsel olarak oluþtur
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

            // Bird sýnýfýndan türetiyoruz.
            Bird birdComponent = birdGO.AddComponent<Bird>();
            

            // Hareket alanýný ve kýrmýzý yolu oluþtur
            for (int y = (int)birdY - 5; y <= (int)birdY + 5; y++)
            {
                if (y >= 0 && y < height)
                {
                    Instantiate(redPathPrefab, new Vector3(birdX, y, 0), Quaternion.identity);
                    mapMatrix[(int)birdX, y] = 6; // Matriste kuþun hareket edebileceði alaný iþaretle
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

            // Arýnýn hareket alanýný ve sarý yolu oluþtur (Sað-sol hareket için)
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

                // Seçilen konumun ve çevresinin kontrolü
                for (int xOffset = -1; xOffset <= 10; xOffset++)
                {
                    for (int yOffset = -1; yOffset <= 1; yOffset++)
                    {
                        int checkX = Mathf.RoundToInt(wallX) + xOffset;
                        int checkY = Mathf.RoundToInt(wallY) + yOffset;

                        // Harita sýnýrlarýný kontrol et
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

                // Duvar prefabýný seç ve yerleþtir
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
            // Aðacýn boyutunu rastgele belirle
            float treeScaleFactor = Random.Range(minTreeSize, maxTreeSize + 1);

            // Aðacýn konumunu rastgele belirle
            float treeX = Random.Range(0, width - treeScaleFactor);
            float treeY = Random.Range(0, height - treeScaleFactor);
            Vector3 treePosition = new Vector3(treeX, treeY, 0);

            GameObject selectedPrefab = treeX < width / 2 ? treePrefabWinter : treePrefabSummer;

            // Seçilen aðaç prefabýný oluþtur ve konumlandýr
            GameObject treeGO = Instantiate(selectedPrefab, treePosition, Quaternion.identity, transform);
            treeGO.transform.localScale = new Vector3(treeScaleFactor, treeScaleFactor, 1); // Aðacýn boyutunu ayarla

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
                        mapMatrix[matrixX, matrixY] = 1; // Aðaçlarýn bulunduðu yerdeki deðeri 1 olarak güncelle
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
                mapMatrix[x, y] = -1; // Engelsiz alanlarý -1 ile baþlat
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
            // Mountain bileþeninin özelliklerini ayarla
            mountainComponent.targetWidth = mountainScaleFactor;
            mountainComponent.targetHeight = mountainScaleFactor;

            // Matriste daðýn kapladýðý alaný güncelle
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
                        mapMatrix[matrixX, matrixY] = 2; // Daðlarýn bulunduðu yerdeki deðeri 2 olarak güncelle
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
            rockComponent.size = rockSize; // Kaya boyutunu rastgele seçilen deðere ayarla

            // Matriste kayanýn kapladýðý alaný güncelle
            for (int x = 0; x < rockSize; x++)
            {
                for (int y = 0; y < rockSize; y++)
                {
                    int matrixX = Mathf.RoundToInt(rockPosition.x) + x;
                    int matrixY = Mathf.RoundToInt(rockPosition.y) + y;
                    if (matrixX >= 0 && matrixX < width && matrixY >= 0 && matrixY < height)
                    {
                        mapMatrix[matrixX, matrixY] = 3; // Kayalarýn bulunduðu yerdeki deðeri 3 olarak güncelle
                    }
                }
            }
        }
    }

    public GameObject greenPrefab; // Yeþil prefab

    void FindShortestPathToChests(List<Vector2Int> chests, Vector2Int characterStartPosition)
    {
        // BFS algoritmasý için kullanýlacak kuyruk veri yapýsý
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(characterStartPosition);

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        visited.Add(characterStartPosition);

 
        int[,] greenPathMatrix = new int[width, height];

        // BFS algoritmasý
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

                    // Yeþil yolu iþaretle
                    greenPathMatrix[newPosition.x, newPosition.y] = 1;
                }
            }
        }

        // Matristeki yeþil yollarý güncelle
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (greenPathMatrix[x, y] == 1)
                {
                    // Yeþil prefab'ý ile yol
                    Vector3 tilePosition = new Vector3(x, y, 0);
                    Instantiate(greenPrefab, tilePosition, Quaternion.identity);
                }
            }
        }
    }
    void CreatePathToTreasure()
    {
        Vector2Int characterStart = new Vector2Int(0, 0); 
        Vector2Int treasurePosition = new Vector2Int(width - 1, height - 1); // Hazine sandýðýnýn konumu varsayýlan deðer

        Queue<Vector2Int> toVisit = new Queue<Vector2Int>(); // Ziyaret edilecek konumlarý tutan queue
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[characterStart] = characterStart; 

        // Baþlangýç noktasýný kuyruða ekle
        toVisit.Enqueue(characterStart);

        while (toVisit.Count > 0)
        {
            Vector2Int currentPos = toVisit.Dequeue();

            // Hazine sandýðýna ulaþýldý mý kontrol et
            if (currentPos == treasurePosition)
            {
                break; // Hedefe ulaþýldý
            }

            // Komþu konumlarý kontrol et
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

        // Yolu yeniden oluþtur
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
        new Vector2Int(1, 0), // saða
        new Vector2Int(0, 1), // yukarý
        new Vector2Int(-1, 0), // sola
        new Vector2Int(0, -1) // aþaðý
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
    //    // BFS algoritmasý için bir kuyruk oluþtur
    //    Queue<Vector2Int> queue = new Queue<Vector2Int>();

    //    // Karakterin konumunu kuyruða ekle
    //    queue.Enqueue(characterPosition);

    //    // En kýsa yol bulunduðunda bu bool deðeri true olacak
    //    bool shortestPathFound = false;

    //    while (queue.Count > 0 && !shortestPathFound)
    //    {
    //        // Kuyruktan bir pozisyon al
    //        Vector2Int currentPosition = queue.Dequeue();

    //        // Mevcut pozisyonu iþaretleyerek tekrar ziyaret etmemek için 'visited' kümesine ekle
    //        visited.Add(currentPosition);

    //        // Eðer mevcut pozisyon sandýk konumlarýndan biriyse, en kýsa yol bulundu demektir.
    //        if (IsChest(currentPosition))
    //        {
    //            shortestPathFound = true;
    //            break;
    //        }

    //        // Mevcut pozisyonun komþu hücrelerini kontrol et
    //        foreach (var move in possibleMoves)
    //        {
    //            Vector2Int nextPosition = currentPosition + move;

    //            // Eðer bir sonraki pozisyon geçerliyse ve daha önce ziyaret edilmediyse
    //            if (CanMove(nextPosition) && !visited.Contains(nextPosition))
    //            {
    //                // Bir sonraki pozisyonu kuyruða ekle
    //                queue.Enqueue(nextPosition);

    //                // Matriste en kýsa yolu göstermek için renklendirme yap
    //                ColorPathTile(nextPosition);
    //            }
    //        }
    //    }
    //}

    bool IsChest(Vector2Int position)
    {
        // Verilen pozisyonun bir sandýk pozisyonu olup olmadýðýný kontrol et
        return mapMatrix[position.x, position.y] == 19 || mapMatrix[position.x, position.y] == 20 || mapMatrix[position.x, position.y] == 21 || mapMatrix[position.x, position.y] == 22;
    }

    void ColorPathTile(Vector2Int position)
    {
        // Verilen pozisyonun matrisindeki deðerini güncelle ve rengini yeþil yap
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
            path.RemoveAt(pathIndex); // Sandýk yerleþtirildikten sonra, ayný konuma baþka bir sandýk yerleþtirilmesini önlemek için

            GameObject selectedPrefab = chestLocation.x < width / 2 ? chestPrefabWinter : chestPrefabSummer;
            Instantiate(selectedPrefab, new Vector3(chestLocation.x, chestLocation.y, 0), Quaternion.identity);

            // Sandýðýn matristeki konumunu güncelle
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
                if (mapMatrix[x, y] == 0) // Yolu temsil eden sýfýrlarý ara
                {
                    path.Add(new Vector2Int(x, y));
                }
            }
        }

        // Her sandýk türü için belirlenen sayýda sandýðý yerleþtir ve konumunu güncelle
        PlaceChestsAlongPath(goldChestCount, goldTreasureChestPrefabSummer, goldTreasureChestPrefabWinter, path, 22); // Altýn için 22
        PlaceChestsAlongPath(silverChestCount, silverTreasureChestPrefabSummer, silverTreasureChestPrefabWinter, path, 21); // Gümüþ için 21
        PlaceChestsAlongPath(bronzeChestCount, bronzeTreasureChestPrefabSummer, bronzeTreasureChestPrefabWinter, path, 19); // Bronz için 20
        PlaceChestsAlongPath(emeraldChestCount, emeraldTreasureChestPrefabSummer, emeraldTreasureChestPrefabWinter, path, 20); // Zümrüt için 19
    }
    public GameObject fogPrefab; // Sis prefab'ý için bir referans
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
        // Karakterin hedefe doðru hareketini saðla.
        MoveCharacter();

        // Karakterin çevresindeki sisi temizle.
        ClearFogAroundCharacter();
    }

    HashSet<Vector2Int> visited = new HashSet<Vector2Int>(); // Ziyaret edilen noktalarý takip eden küme.
    
    // Sandýða gitmek için BFS kullanarak en kýsa yolu bulan fonksiyon.
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

    //    // En kýsa yolu iþaretle.
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

        // Geçerli ve mümkün olan hareketleri bul.
        foreach (var move in possibleMoves)
        {
            Vector2Int newPosition = characterPosition + move;
            if (CanMove(newPosition) && !visited.Contains(newPosition))
            {
                validMoves.Add(move);
            }
        }

        // Eðer hiçbir geçerli hareket yoksa, karakterin önceki hareketleri sýfýrla.
        if (validMoves.Count == 0)
        {
            visited.Clear();
            validMoves = possibleMoves.Where(move => CanMove(characterPosition + move)).ToList();
        }

        // Rastgele bir geçerli hareket seç.
        int randIndex = Random.Range(0, validMoves.Count);
        Vector2Int nextMove = validMoves[randIndex];

        // Karakterin yeni pozisyonunu hesapla ve güncelle.
        SetCharacterPosition(characterPosition + nextMove);

        // Hareket edilen pozisyonu 'visited' kümesine ekle.
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
        // Pozisyonun harita sýnýrlarý içinde olup olmadýðýný ve boþ bir alan olup olmadýðýný kontrol et.
        return position.x >= 0 && position.x < width && position.y >= 0 && position.y < height && mapMatrix[position.x, position.y] == 0;
    }


    //void SetCharacterPosition(Vector2Int newPosition)
    //{
    //    // Karakterin matristeki ve sahnedeki konumunu güncelle
    //    mapMatrix[characterPosition.x, characterPosition.y] = 0; // eski pozisyonu temizle
    //    characterPosition = newPosition;
    //    mapMatrix[newPosition.x, newPosition.y] = 10; // yeni pozisyonu iþaretle
    //    characterGO.transform.position = new Vector3(newPosition.x, newPosition.y, 0);
    //}

    void ClearFogAroundCharacter()
    {
        // Karakterin çevresindeki sisi temizle.
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
                                      // Burada yeni eklediðimiz kýsým baþlýyor.
                        ReportDiscoveredObjects(x, y); // Keþfedilen nesneleri raporla.
                    }
                }
            }
        }
    }


    void ReportDiscoveredObjects(int x, int y)
    {
        // mapMatrix'teki deðerlere göre nesne türünü kontrol et ve raporla.
        switch (mapMatrix[x, y])
        {
            case 1:
                Debug.Log("Aðaç keþfedildi.");
                break;
            case 2:
                Debug.Log("Dað keþfedildi.");
                break;
            case 3:
                Debug.Log("Kaya keþfedildi.");
                break;
            case 4:
                Debug.Log("Duvar keþfedildi.");
                break;
            case 22:
                Debug.Log("Altýn Sandýk keþfedildi.");
                break;
            case 21:
                Debug.Log("Gümüþ Sandýk keþfedildi.");
                break;
            case 20:
                Debug.Log("Zümrüt Sandýk keþfedildi.");
                break;
            case 19:
                Debug.Log("Bronz Sandýk keþfedildi.");
                break;
            
        
        
        }

        if (mapMatrix[x, y] == 22 || mapMatrix[x, y] == 21 || mapMatrix[x, y] == 20 || mapMatrix[x, y] == 19)
        {
            discoveredChestCount++; // Keþfedilen sandýk sayýsýný artýr
            Debug.Log("Sandýk keþfedildi. Toplam keþfedilen sandýk sayýsý: " + discoveredChestCount);

            // Sandýk keþfedildikten sonra, tekrar keþfedilmemesi için bu konumdaki deðeri güncelleyin
            mapMatrix[x, y] = 50; // Sandýk keþfedildi olarak iþaretle (Örneðin, 5 deðeri sandýk keþfedildi anlamýna gelsin)
        }
    }



    void GenerateMap()
    {
        // Haritayý oluþtur
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Her bir karo için pozisyon hesapla
                Vector3 tilePosition = new Vector3(x, y, 0);
                // X koordinatýna göre kýþ veya yaz prefab'ýný seç
                GameObject selectedPrefab = x < width / 2 ? winter : summer;
                Instantiate(selectedPrefab, tilePosition, Quaternion.identity, transform);
            }
        }
    }
}