using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    // 맵 배열. (여러 맵 저장을 위해 선언)
    public CommandMap[] commandMaps;
    // 맵 인덱스
    public int mapIndex;

    // 현재 맵 제어 클래스
    CommandMap currentMap;

    // 타일
    public Transform tilePrefab;
    // 장애물
    public Transform obstaclePrefab;
    // 투명하고, 충돌 판정이 있는 벽 (이동 제한용)
    public Transform transparentWallPrefab;

    // NavMesh 바닥
    public Transform navmeshFloor;
    // NavMesh Mask. (가장자리 이동 불가 지역 설정)
    public Transform navmeshMaskPrefab;


    /* 기본 맵 설정 */

    // 최대 맵사이즈(Navmesh바닥의)
    public Vector2 maxMapSize;
    // 타일 사이즈
    public float tileSize;
    // 외곽선 비율
    [Range(0,1)]
    public float outlinePercent;


    /* 타일 정보를 저장할 List, Queue */

    // 모든 타일맵의 좌표를 저장할 List
    List<Coord> allTileCoords;
    // Shuffle된 타일맵의 모든 좌표를 저장할 Queue.
    Queue<Coord> shuffledTileCoords;
    // Shuffle된 타일맵의 장애물 없는 좌표를 저장할 Queue.
    Queue<Coord> shuffledOpenTileCoords;


    /* 장애물 관련 변수 */

    // 렌덤 장애물 생성을 허용하는지 판단하는 변수
    public bool applyRandomObstacle = false;
    // 랜덤 장애물 높이 지정을 허용하는지 판단하는 변수
    public bool applyRandomHeight = false;
    // 고정된 장애물 높이.
    public float FixedObstacleHeight;

    // 장애물 벽 높이.
    public float obstacleWallHeight = 10;


    /* 외부에 맵 좌표를 가져오기 위한 저장 변수*/
    Transform[,] tileMap;



    void Start()
    {
        GenerateMap();
    }


    // 맵 생성하는 메소드
    public void GenerateMap()
    {
        // 맵 배열로부터, 현재 맵 불러오기.
        currentMap = commandMaps[mapIndex];

        // 맵 유효성 체크
        currentMap.CheckMapSize();

        // tileMap Transform 생성.
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];

        // 현재 맵d의 Seed값에 대한 렌덤 값 구하기.
        System.Random prng = new System.Random(currentMap.seed);
        // BoxCollider 설정. (맵 전체 바닥에 대한 충돌체)
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, 0.05f, currentMap.mapSize.y);

        // 맵 사이즈 설정에 대한 예외처리.
        if (currentMap.mapSize.x > maxMapSize.x)
        {
            print("최대 맵 사이즈 보다 큰 값 입력!");
            currentMap.mapSize.x = (int)maxMapSize.x;
            return;
        }
        if (currentMap.mapSize.y > maxMapSize.y)
        {
            print("최대 맵 사이즈 보다 큰 값 입력!");
            currentMap.mapSize.y = (int)maxMapSize.y;
            return;
        }

        /* 좌표를 생성 후 List에 저장. */
        allTileCoords = new List<Coord>();

        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }


        /* 저장된 타일 좌표를 Shuffle. */
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));


        /* 맵홀더 오브젝트를 생성. */

        string holderName = "Generated Map";

        // holderNmae을 가진 오브젝트 아래에 자식객체들이 있으면,
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        // 새로운 mapHolder 지정.
        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;


        /* 타일을 직접 생성하고, 외곽선을 만드는 부분 */

        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                // 타일 외곽선대비 전체 크기 설정.
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                // 새로운 타일 부모를 mapHolder로 지정.
                newTile.parent = mapHolder;
                // TileMap에도 Tile 저장.
                tileMap[x, y] = newTile;
            }

        }


        /* 장애물을 생성하는 부분 */

        if (applyRandomObstacle)
        {
            bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];
            int obstacleCount = (int) (currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
            int currentObstacleCount = 0;

            // 새로운 Coords 리스트 생성.
            // 장애물 생성된 좌표를 빼기 위함.
            List<Coord> allOpenCoords = new List<Coord>(allTileCoords);

            for (int i = 0; i < obstacleCount; i++)
            {
                Coord randomCoord = GetRandomCoord();
                obstacleMap[randomCoord.x, randomCoord.y] = true;
                currentObstacleCount++;

                // randomCoord가 맵 중앙 좌표가 아니고, 맵 전체가 접근 가능하면,
                if (randomCoord != currentMap.mapCenter && MapIsFullyAccesible(obstacleMap, currentObstacleCount))
                {

                    // 장애물 높이 설정.
                    float obstacleHeight;

                    // 렌덤 높이 설정 유무.
                    if (applyRandomHeight)
                    {
                        obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                    }
                    else
                    {
                        obstacleHeight  = FixedObstacleHeight;
                    }

                    
                    Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                    Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2, Quaternion.identity) as Transform;

                    newObstacle.parent = mapHolder;
                    // 장애물 크기 설정.
                    newObstacle.localScale = new Vector3( (1 - outlinePercent) * tileSize, obstacleHeight , (1 - outlinePercent) * tileSize);

                    // 장애물 색깔 변경 설정.
                    Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                    Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                    float ColorPercent = randomCoord.y / (float)currentMap.mapSize.y;

                    obstacleMaterial.color = Color.Lerp(currentMap.foreGroundObstacleColor, currentMap.backGroundObstacleColor, ColorPercent);
                    obstacleRenderer.sharedMaterial = obstacleMaterial;

                    // OpenCoords 리스트에서 장애물 생성 좌표를 제거.
                    allOpenCoords.Remove(randomCoord);
                }
                else
                {
                    // 장애물을 생성하지 않았으므로, 다시 이전 갯수로 복구.
                    // 더불어 장애물이 생성되면 안되므로, obstacleMap False로 지정.
                    obstacleMap[randomCoord.x, randomCoord.y] = false;
                    currentObstacleCount--;
                }

            }

            // allOpenCoords에 있는 좌표를 Shuffle.
            shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));
        }




        /* 네브메쉬를 제어하는 부분 */

        // NavMeshFloor는 접근이 가능하도록 하는 바닥.
        // NavMeshMask는 접근이 불가능하게 가리우는 지역.
        // (1 - outlinePercent)

        // 왼쪽 접근 불가 지역.
        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;
        // 왼쪽 벽
        Transform obstacleWallLeft = Instantiate(transparentWallPrefab, Vector3.left * (currentMap.mapSize.x / 2f + 0.5f) * tileSize, Quaternion.identity) as Transform;
        obstacleWallLeft.parent = mapHolder;
        obstacleWallLeft.localScale = new Vector3(1, obstacleWallHeight, currentMap.mapSize.y) * tileSize;

        // 오른쪽 접근 불가 지역.
        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;
        // 오른쪽 벽
        Transform obstacleWallRight = Instantiate(transparentWallPrefab, Vector3.right * (currentMap.mapSize.x / 2f + 0.5f) * tileSize, Quaternion.identity) as Transform;
        obstacleWallRight.parent = mapHolder;
        obstacleWallRight.localScale = new Vector3(1, obstacleWallHeight, currentMap.mapSize.y) * tileSize;

        // 위쪽 접근 불가 지역.
        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y)/2f) * tileSize;
        // 위쪽 벽
        Transform obstacleWallTop = Instantiate(transparentWallPrefab, Vector3.forward * (currentMap.mapSize.y / 2f + 0.5f) * tileSize, Quaternion.identity) as Transform;
        obstacleWallTop.parent = mapHolder;
        obstacleWallTop.localScale = new Vector3(currentMap.mapSize.x + 2f, obstacleWallHeight, 1) * tileSize;

        // 아랫쪽 접근 불가 지역.
        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;
        // 아랫쪽 벽
        Transform obstacleWallBot = Instantiate(transparentWallPrefab, Vector3.back * (currentMap.mapSize.y / 2f + 0.5f) * tileSize, Quaternion.identity) as Transform;
        obstacleWallBot.parent = mapHolder;
        obstacleWallBot.localScale = new Vector3(currentMap.mapSize.x + 2f, obstacleWallHeight, 1) * tileSize;

        // 맵 높은 곳 이탈 방지 투명한 높이 지붕 벽(뚜껑)
        // 잦은 콜라이더 충돌로 인해 비생성(2018.08.23)
        /*
        Transform obstacleRoof = Instantiate(transparentWallPrefab, Vector3.up * (obstacleWallHeight/2 + 0.5f), Quaternion.identity) as Transform;
        obstacleRoof.parent = mapHolder;
        obstacleRoof.localScale = new Vector3(currentMap.mapSize.x + 2f, (1 - outlinePercent), currentMap.mapSize.y + 2f) * tileSize;
        */

        // 전체 최대 맵 사이즈 조절.
        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;

    }

    // 맵 전체가 접근 가능한지 판별하는 메소드
    // Apply Flood-Fill Algorithm
    bool MapIsFullyAccesible(bool[,] obstacleMap, int currentObstacleCount)
    {
        // 맵을 접근 했는지 판별하는 Flag.
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        // 좌표에 대한 Queue
        Queue<Coord> queue = new Queue<Coord>();

        // 중앙좌표 부터 시작.
        queue.Enqueue(currentMap.mapCenter);
        // 중앙좌표 체크 플래그 표시.
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;
        // 접근 가능했던 타일 수
        int AccesibleTileCount = 1; //중앙 좌표는 이미 접근가능하므로, 1.


        /* Flood-Fill Algorithm. */

        while (queue.Count > 0)
        {
            // queue의 아이템을 꺼내는 동시에 queue내의 아이템 삭제.
            Coord tile = queue.Dequeue();

            // 근접 4개의 이웃 좌표 체크.
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    // 이웃 좌표 할당.
                    int neighborX = tile.x + x;
                    int neighborY = tile.y + y;

                    // 대각선 방향은 체크를 하지 않음.
                    if (x == 0 || y == 0)
                    {
                        // 이웃좌표가 맵 내에 들어있다면, (맵 바깥 쪽 경우 제외)
                        if (neighborX >= 0 && neighborX < obstacleMap.GetLength(0) && neighborY >= 0 && neighborY < obstacleMap.GetLength(1))
                        {
                            // 전에 체크한 좌표가 아니고, 장애물 타일이 아니라면,
                            // 장애물 타일은 제외시켰으므로, 장애물 타일에는 막히게 된다.
                            if (!mapFlags[neighborX, neighborY] && !obstacleMap[neighborX, neighborY])
                            {
                                // 해당 좌표 체크.
                                mapFlags[neighborX, neighborY] = true;
                                queue.Enqueue(new Coord(neighborX, neighborY));
                                AccesibleTileCount++;
                            }    

                        }

                    }
                }

            }
        }

        // 실제 장애물 없이 존재하는 타일 갯수
        // 타일 모두가 접근이 가능하다면, 
        // 실제 장애물 없는 타일 갯수와 위의 접근 가능 타일 갯수는 동일해야 한다.
        // 참고) 접근가능이란 : 장애물로 사방이 막혀있지 않아, 모든 빈 타일은 접근이 가능한 것.

        int targetAccesibleTilecCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);

        return targetAccesibleTilecCount == AccesibleTileCount;
    }


    // 좌표를 실제 게임 Vector3좌표로 반환하는 메소드
    Vector3 CoordToPosition(int x, int y)
    {
        // 참) 홀수 일때도 정확한 위치 계산을 위하여 2f로 나눔.
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }


    // 셔플된 큐의 렌덤 좌표로부터 다음 아이템을 얻어 반환하는 메소드
    public Coord GetRandomCoord()
    {
        // Queue의 가장 첫번째 아이템 추출.
        Coord randomCoord = shuffledTileCoords.Dequeue();
        // 추출한 아이템을 Queue의 제일 뒤로 이동.
        shuffledTileCoords.Enqueue(randomCoord);

        return randomCoord;
    }

    // 단순히 저장되어 있는 타일들을 가져오는 메소드
    public Transform[,] GetTileMap()
    {
        return tileMap;
    }

    // 무작위 오픈 타일을 가져오기 위한 메소드
    public Transform GetRandomOpenTile()
    {
        // Queue의 가장 첫번째 아이템 추출.
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        // 추출한 아이템을 Queue의 제일 뒤로 이동.
        shuffledOpenTileCoords.Enqueue(randomCoord);

        return tileMap[randomCoord.x, randomCoord.y];
    }

    [System.Serializable]
    public class CommandMap
    {
        // 맵 사이즈 제어 변수.
        public Coord mapSize;


        // 랜덤 씨드 제어 변수.
        public int seed;
        // 맵 내 장애물 비율 제어 변수.
        [Range (0,1)]
        public float obstaclePercent;
        // 장애물 최소 높이 제어 변수.
        public float minObstacleHeight;
        // 장애물 최대 높이 제어 변수.
        public float maxObstacleHeight;
        // 장애물 전면부 색깔 제어 변수.
        public Color foreGroundObstacleColor;
        // 장애물 후면부 색깔 제어 변수.
        public Color backGroundObstacleColor;

        // 맵 제한 크기
        int mapSizeXLimit = 200;
        int mapSizeYLimit = 200;



        // 맵 중앙 좌표 제어 변수.
        public Coord mapCenter
        {
            get
            {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }


        // 생성자에서 맵 유효성 테스트
        public CommandMap()
        {
            CheckMapSize();
        }


        // 맵 크기 유효성 체크 메소드
        public void CheckMapSize()
        {
            if (mapSize.x > mapSizeXLimit || mapSize.y > mapSizeYLimit)
            {
                mapSize.x = mapSizeXLimit;
                mapSize.y = mapSizeYLimit;
            }
        }
    }
}
