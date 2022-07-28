using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunatorSharp;
using DelaunatorSharp.Unity;
using DelaunatorSharp.Unity.Extensions;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.MinimumSpanningTree;

public enum EnclosureType { Normal, StartRoom, BossRoom }

public class RandomLevelGenerator : MonoBehaviour
{
    [System.Serializable]
    public class RoomHolder
    {
        public RoomHolder(int roomID, bool isMain, GameObject roomObject, Vector2 dimensions)
        {
            this.roomID = roomID;
            this.isMain = isMain;
            this.roomObject = roomObject;
            this.dimensions = dimensions;
            type = EnclosureType.Normal;
        }

        public int roomID;
        public bool isMain = false;
        public EnclosureType type;
        public GameObject roomObject;
        public Vector2 dimensions;
        public float Area { get => dimensions.x * dimensions.y; }
    }

    public class Path
    {
        public Path(Transform start, Transform mid, Transform end)
        {
            this.start = start;
            this.mid = mid;
            this.end = end;
        }

        public Transform start;
        public Transform mid;
        public Transform end;
    }

    [Header("MainRooms")]
    public GameObject[] mainRoomPrefabs;
    public GameObject bossRoomPrefab;
    public int mainRoomCount = 4;
    private List<RoomHolder> mainRooms = new List<RoomHolder>();
    private float max_length = 0;
    private Vector3 startRoomPos;
    private Vector3 endRoomPos;
    public float startAreaSize;

    [Header("Obstacles")]
    public GameObject roomPrefab;
    public int roomCount = 150;
    public Vector2 minMaxQuadLength = new Vector2(10f, 20f);
    public AnimationCurve roomSizeDistribution;
    public float spawnRadius = 5;
    private List<RoomHolder> rooms = new List<RoomHolder>();


    [Header("Simulation")]
    public GameEvent initgeneration;
    public GameEvent seperatedRooms;
    public GameEvent finishedGeneration;
    public bool generateOnStart = false;
    public float generationTimeOut = 5f;
    private IEnumerator timeOutRoutine;
    private bool timeOutRunning = false;
    private IEnumerator generationRoutine;
    private bool generationRunning = false;

    private bool seperatingRooms = false;

    [Header("Generation")]
    public int gridScale = 1;
    public bool invertObstacles = true;
    //public GenerateMeshForWarping holePuncher;
    public Vector3 indentVerticiesOffset = new Vector3(0, -10, 0);
    public GameObject mainRoomContainer;
    public GameObject normalRoomContainer;
    [SerializeField] private TerrainGenerator terrainGen;
    private List<Path> connectingPaths = new List<Path>();
    

    [Header("Navmesh")]
    [SerializeField] private navmeshUpdate navMeshGen;

    [Header("Visualization")]
    public GameObject delaunayLineContainer;
    public GameObject mstLineContainer;
    public GameObject addedLineContainer;
    public GameObject actualPathLineContainer;
    public Vector3 lineDrawOffset = Vector3.up * 10f;
    public Material lineMaterial;
    public float lineWidth = 1f;

    private List<GameObject> lineObjects = new List<GameObject>();

    [Header("Delaunator")]
    [Range(0f, 1f)]public float readdEdgePercentage = 0.1f;
    private Delaunator delaunator;
    private UndirectedGraph<Vector3, Edge<Vector3>> delaunayGraph = new UndirectedGraph<Vector3, Edge<Vector3>>();
    private UndirectedGraph<Vector3, Edge<Vector3>> mstGraph = new UndirectedGraph<Vector3, Edge<Vector3>>();
    private UndirectedGraph<Vector3, Edge<Vector3>> pathGraph = new UndirectedGraph<Vector3, Edge<Vector3>>();
    private IEnumerable<Edge<Vector3>> minimumSpanningTree;
    private List<Edge<Vector3>> allGraphEdges = new List<Edge<Vector3>>();

    private Dictionary<Edge<Vector3>, RoomHolder[]> mainPathEdgesRooms = new Dictionary<Edge<Vector3>, RoomHolder[]>();

    [Header("Player")]
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject[] playerWeaponsToSpawnAtStart;
    private Vector3 playerSpawnPosition;

    private void Start()
    {
        if (generateOnStart) StartCoroutine(InitalGenDelay());
        playerSpawnPosition = Vector3.zero;
    }

    private IEnumerator InitalGenDelay()
    {
        yield return new WaitForEndOfFrame();
        TestGen();
    }

    private void Update()
    {
        if (seperatingRooms)
        {
            seperatingRooms = false;
            foreach(var room in rooms)
            {
                NormalRoom roomScript = room.roomObject.GetComponent<NormalRoom>();

                if (roomScript && roomScript.GetColliders().Count > 0)
                {
                    seperatingRooms = true;
                    break;
                }
            }

            if (!seperatingRooms)
            {
                List<Transform> mrt = new List<Transform>();
                foreach(var r in mainRooms)
                {
                    mrt.Add(r.roomObject.transform);
                }

                ContinueGenerationAfterSeperation();
            }
        }
    }

    public void TestGen()
    {
        //stop the timeout if it is still running, eventhough it shouldn't
        if (timeOutRunning)
        {
            StopCoroutine(timeOutRoutine);
            timeOutRunning = false;
        }

        //timeout
        timeOutRoutine = GenerationTimeout();
        StartCoroutine(timeOutRoutine);

        initgeneration.Raise();

        for(int i = rooms.Count - 1; i >= 0; i--)
        {
            Destroy(rooms[i].roomObject);
        }

        for (int i = lineObjects.Count - 1; i >= 0; i--)
        {
            Destroy(lineObjects[i]);
        }

        //clear graphs and objects
        rooms.Clear();
        lineObjects.Clear();
        pathGraph.Clear();
        mainRooms.Clear();
        connectingPaths.Clear();
        mainPathEdgesRooms.Clear();
        delaunayGraph.Clear();
        mstGraph.Clear();
        seperatingRooms = false;

        PlaceRoomsInCircle();
    }

    private IEnumerator GenerationTimeout()
    {
        timeOutRunning = true;
        yield return new WaitForSecondsRealtime(generationTimeOut);
        timeOutRunning = false;

        if (generationRunning)
        {
            StopCoroutine(generationRoutine);
            generationRunning = false;
        }

        TestGen();
    }

    public void PlaceRoomsInCircle()
    {
        Time.timeScale = 10;

        //force room at idx 0 to spawn, because the others cause issues with their unique colliders rn, and this is the quickest "fix" and we're runing out of time
        GameObject firstRoom = Instantiate(mainRoomPrefabs[0], mainRoomContainer.transform);
        firstRoom.transform.position = new Vector3(0, firstRoom.transform.position.y, 0);
        rooms.Add(new RoomHolder(rooms.Count, true, firstRoom, Vector2.one));
        mainRooms.Add(new RoomHolder(rooms.Count, true, firstRoom, Vector2.one));

        for (int i = 1; i < mainRoomCount; i++)
        {
            Vector2 pos = TruePointInCircle(Vector3.zero, spawnRadius);
            GameObject mainRoom = Instantiate(mainRoomPrefabs[Random.Range(0, mainRoomPrefabs.Length)], mainRoomContainer.transform);
            if (!mainRoom.GetComponent<NormalRoom>()) mainRoom.AddComponent<NormalRoom>();
            mainRoom.transform.position = new Vector3(pos.x, mainRoom.transform.position.y, pos.y);
            rooms.Add(new RoomHolder(rooms.Count, true, mainRoom, Vector2.one));
            mainRooms.Add(new RoomHolder(rooms.Count, true, mainRoom, Vector2.one));
        }

        for (int i = 0; i < roomCount - mainRoomCount; i++)
        {
            Vector2 pos = TruePointInCircle(Vector3.zero, spawnRadius);
            Vector2 length = RndRoomDimensions();
            GameObject room = Instantiate(roomPrefab, normalRoomContainer.transform);
            room.transform.position = new Vector3(pos.x, room.transform.position.y, pos.y);
            room.transform.localScale = new Vector3(length.x, room.transform.localScale.y, length.y);
            rooms.Add(new RoomHolder(rooms.Count, false, room, length));
        }

        foreach (var room in mainRooms)
        {
            MainRoom mr = room.roomObject.GetComponent<MainRoom>();
            mr.PrepForGeneration();
        }

        StartCoroutine(DelaySeperationBool());
    }

    private IEnumerator DelaySeperationBool()
    {
        yield return new WaitForEndOfFrame();
        seperatingRooms = true;
    }

    public void ContinueGenerationAfterSeperation()
    {
        //think it's redundant but shouldn't hurt just to be safe, don't have the time to properly test for redundancy rn
        if (generationRunning)
        {
            StopCoroutine(generationRoutine);
            generationRunning = false;
        }

        generationRoutine = ContinueGeneration();
        StartCoroutine(generationRoutine);
    }

    public IEnumerator ContinueGeneration()
    {
        generationRunning = true;

        Time.timeScale = 1;
        foreach (var room in mainRooms)
        {
            MainRoom mr = room.roomObject.GetComponent<MainRoom>();
            mr.RemoveNormalRoomComponent();
            mr.SeperatedRoomState();
        }

        GridSnapRooms();
        Delunate();
        DelaunayToGraph();
        MinimumSpanningTreeForGraph();
        ReaddRandomEdgesToMST();
        CreateStartArea();
        ReplacelastRoomWithBossRoom();

        seperatedRooms.Raise();

        GeneratePathConnections();
        TossRooms();
        navMeshGen.BakeNavmeshAtRuntime();

        yield return new WaitForEndOfFrame();
        terrainGen.RegenerateTerrain();

        finishedGeneration.Raise();
        PlayerController.Instance.transform.position = playerSpawnPosition;

        if (timeOutRoutine != null)
        {
            StopCoroutine(timeOutRoutine);
        }

        SpawnPlayerWeapons();
        generationRunning = false;
    }

    private void SortRoomsByAreaDescending()
    {
        rooms.Sort((x, y) => y.Area.CompareTo(x.Area));
    }

    private void GridSnapRooms()
    {
        foreach(var room in rooms)
        {
            Transform t = room.roomObject.transform;
            t.position = t.position.SnapToGrid(gridScale);
        }
    }

    private void Delunate()
    {
        List<RoomHolder> mainRooms = new List<RoomHolder>();
        foreach (var room in rooms)
        {
            if (room.isMain) mainRooms.Add(room);
        }

        Vector2[] mainRoomsTransforms = new Vector2[mainRooms.Count];
        for (int i = 0; i < mainRooms.Count; i++)
        {
            mainRoomsTransforms[i] = new Vector2(mainRooms[i].roomObject.transform.position.x, mainRooms[i].roomObject.transform.position.z);
        }

        delaunator = new Delaunator(mainRoomsTransforms.ToPoints());

        delaunator.ForEachTriangleEdge(edge =>
        {
            CreateLine(delaunayLineContainer.transform, $"TriangleEdge - {edge.Index}", new Vector3[] { edge.P.OwnToVector3() + lineDrawOffset, edge.Q.OwnToVector3() + lineDrawOffset }, new Color(255, 0, 0, 100), lineWidth, 0);
        });
    }

    private void DelaunayToGraph()
    {
        if (delaunator == null) return;

        delaunator.ForEachTriangleEdge(edge =>
        {
            delaunayGraph.AddVerticesAndEdge(new Edge<Vector3>(edge.P.OwnToVector3(), edge.Q.OwnToVector3()));
        });
    }

    private void MinimumSpanningTreeForGraph()
    {
        minimumSpanningTree = delaunayGraph.MinimumSpanningTreePrim(e => Vector3.Distance(e.Source, e.Target));

        foreach(var edge in minimumSpanningTree)
        {
            //convert mst Ienumerator to graph
            mstGraph.AddVertex(edge.Source);
            mstGraph.AddVertex(edge.Target);
            foreach(var delEdge in delaunayGraph.Edges)
            {
                if(edge.Source == delEdge.Source && edge.Target == delEdge.Target)
                {
                    //mstGraph.AddEdge(new Edge<Vector3>(edge.Source, edge.Target));
                    mainPathEdgesRooms.Add(delEdge, GetRoomHoldersAtEdge(delEdge));
                    mstGraph.AddEdge(delEdge);
                }
            }

            CreateLine(mstLineContainer.transform, $"TriangleEdge", new Vector3[] { edge.Source + lineDrawOffset + Vector3.up, edge.Target + lineDrawOffset + Vector3.up }, Color.green, 1f, 0);
        }
    }

    private void ReaddRandomEdgesToMST()
    {
        int totalEdgeCount = delaunayGraph.EdgeCount;
        int keptEdgeCount = mstGraph.EdgeCount;
        int edgesToreaddCount = Mathf.CeilToInt((totalEdgeCount - keptEdgeCount) * readdEdgePercentage); 
        
        for(int i = 0; i < edgesToreaddCount; i++)
        {
            int rnd = Random.Range(0, totalEdgeCount - keptEdgeCount);
            int rndTemp = 0;
            foreach(var edge in delaunayGraph.Edges)
            {
                if (!mstGraph.ContainsEdge(edge))
                {
                    if(rndTemp == rnd)
                    {
                        mstGraph.AddVerticesAndEdge(edge);
                        CreateLine(addedLineContainer.transform, $"TriangleEdge", new Vector3[] { edge.Source + lineDrawOffset + Vector3.up, edge.Target + lineDrawOffset + Vector3.up }, Color.blue, 1f, 0);
                        mainPathEdgesRooms.Add(edge, GetRoomHoldersAtEdge(edge));
                        totalEdgeCount = delaunayGraph.EdgeCount;
                        keptEdgeCount = mstGraph.EdgeCount;
                        break;
                    }
                    else
                    {
                        rndTemp++;
                    }
                }
            }
        }
    }

    private void GeneratePathConnections()
    {
        foreach(var edge in mstGraph.Edges)
        {
            RoomHolder[] roomsOfEdge = mainPathEdgesRooms[edge]; //the two rooms that are connected to the current edge (path)

            //finds the door of room 0 that is closest to room 1, and opens it
            Transform[] doorTransformsToCheck = roomsOfEdge[0].roomObject.GetComponent<MainRoom>().DoorTransforms;
            Transform door1 = doorTransformsToCheck.ClosestTransformToTarget(roomsOfEdge[1].roomObject.transform.position);
            foreach(var doorTransform in doorTransformsToCheck)
            {
                if (doorTransform.position == door1.position)
                {
                    Door door = doorTransform.GetComponent<Door>();
                    door.OpenDoor();
                    door.IsEntrance = true;
                }
            }

            //finds the door of room 1 that is closest to room 0, and opens it
            doorTransformsToCheck = roomsOfEdge[1].roomObject.GetComponent<MainRoom>().DoorTransforms;
            Transform door2 = doorTransformsToCheck.ClosestTransformToTarget(roomsOfEdge[0].roomObject.transform.position);
            foreach (var doorTransform in doorTransformsToCheck)
            {
                if (doorTransform.position == door2.position)
                {
                    Door door = doorTransform.GetComponent<Door>();
                    door.OpenDoor();
                    door.IsEntrance = true;
                }
            }

            //add the path (edge) to graph of only path connections & draw the line for visualization
            pathGraph.AddVerticesAndEdge(new Edge<Vector3>(new Vector3(door1.position.x, 0, door1.position.z), new Vector3 (door2.position.x, 0, door2.position.z)));
            float relativePathHeight = terrainGen.PerlinHeightOffset;
            CreateLine(actualPathLineContainer.transform, "path", new Vector3[] { door1.position, door2.position }, new Color(relativePathHeight, relativePathHeight, relativePathHeight), terrainGen.pathWidth, 0, true);
        }
    }

    private void CreateStartArea()
    {
        FindStartAndEndVertices(mstGraph);
        RoomHolder startRoom = GetRoomAtPosition(startRoomPos);
        startRoom.type = EnclosureType.StartRoom;

        Vector3 totalDirectionFromOtherRooms = Vector3.zero;
        foreach(var room in mainRooms)
        {
            totalDirectionFromOtherRooms += (startRoomPos - room.roomObject.transform.position).normalized;
        }

        //get the cardinal direction of the startroom away from other rooms, to place starting area
        totalDirectionFromOtherRooms.Normalize();
        totalDirectionFromOtherRooms.y = 0;
        Vector3 cardinal = totalDirectionFromOtherRooms.ToCardinalDirection();

        Transform[] doorTransformsToCheck = startRoom.roomObject.GetComponent<MainRoom>().DoorTransforms;
        Door door = doorTransformsToCheck.ClosestTransformToTarget(startRoomPos + cardinal * 1000f).GetComponent<Door>();
        door.OpenDoor();
        door.IsEntrance = true;

        Vector3 doorPos = door.transform.position;
        Vector3 startAreaPos = doorPos + cardinal * startAreaSize;

        // find and set players spawn position
        playerSpawnPosition = (doorPos + startAreaPos) / 2 + Vector3.up;

        float relativePathHeight = terrainGen.PerlinHeightOffset;
        CreateLine(actualPathLineContainer.transform, "startArea", new Vector3[] { doorPos, startAreaPos }, new Color(relativePathHeight, relativePathHeight, relativePathHeight), startAreaSize, 0, true);
    }

    private void TossRooms()
    {
        List<GameObject> foundRooms = new List<GameObject>();
        foreach(var edge in pathGraph.Edges)
        {
            Vector3 dir = edge.Source - edge.Target;
            RaycastHit[] hits = Physics.RaycastAll(edge.Target, dir, dir.magnitude, ~0);
            for (int i = rooms.Count - 1; i >= 0; i--)
            {
                if (!rooms[i].isMain)
                {
                    foreach(var hit in hits)
                    {
                        if (hit.transform.gameObject == rooms[i].roomObject)
                        {
                            foundRooms.Add(hit.transform.gameObject);
                            break;
                        }
                    }
                }
            }
        }

        //remove obstacle above start area if there is one
        Collider[] hitRooms = Physics.OverlapSphere(playerSpawnPosition, startAreaSize);

        foreach(var col in hitRooms)
        {
            if (col.transform.GetComponent<NormalRoom>())
            {
                foundRooms.Add(col.transform.gameObject);
            }
        }


        for (int i = rooms.Count - 1; i >= 0; i--)
        {
            if (!rooms[i].isMain)
            {
                bool partOfFound = false;

                foreach (var room in foundRooms)
                {
                    if (room == rooms[i].roomObject)
                    {
                        partOfFound = true;
                        break;
                    }
                }

                if (partOfFound == invertObstacles)
                {
                    Destroy(rooms[i].roomObject);
                    rooms.Remove(rooms[i]);
                }
            }
        }
    }

    private Vector2 TruePointInCircle(Vector3 origin, float radius)
    {
        Vector2 point = Vector3.zero;
        point.x = Random.Range(-1f, 1f) * radius + origin.x;
        point.y = Random.Range(-1f, 1f) * radius + origin.z;
        return point;
    }

    private Vector2 RndRoomDimensions()
    {
        Vector2 length = Vector3.zero;
        length.x = minMaxQuadLength.x + Random.Range(0f, Mathf.Abs(minMaxQuadLength.y - minMaxQuadLength.x)) * roomSizeDistribution.Evaluate(Random.Range(0f, 1f));
        length.y = minMaxQuadLength.x + Random.Range(0f, Mathf.Abs(minMaxQuadLength.y - minMaxQuadLength.x)) * roomSizeDistribution.Evaluate(Random.Range(0f, 1f));

        //round to multiple of grid size
        length = length.SnapToGrid(gridScale);

        return length;
    }

    private RoomHolder[] GetRoomHoldersAtEdge(Edge<Vector3> edge)
    {
        RoomHolder[] roomPair = new RoomHolder[2];
        int roomIdx = 0;

        foreach (var room in rooms)
        {
            Vector3 roomPos = room.roomObject.transform.position;
            roomPos.y = 0;
            if (roomPos == edge.Source || roomPos == edge.Target)
            {
                roomPair[roomIdx] = room;
                roomIdx++;
                if (roomIdx == roomPair.Length) break;
            }
        }

        return roomPair;
    }

    private void CreateLine(Transform container, string name, Vector3[] points, Color color, float width, int order = 1, bool heightmapRelevant = false)
    {
        var lineGameObject = new GameObject(name);

        if (heightmapRelevant)
        {
            int terrainHeightmapMask = LayerMask.NameToLayer("TerrainHeightMap");
            lineGameObject.layer = terrainHeightmapMask;
        }

        lineObjects.Add(lineGameObject);
        lineGameObject.transform.parent = container;
        var lineRenderer = lineGameObject.AddComponent<LineRenderer>();

        lineRenderer.SetPositions(points);

        lineRenderer.material = lineMaterial ?? new Material(Shader.Find("Standard"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.sortingOrder = order;
    }

    private void FindStartAndEndVertices(UndirectedGraph<Vector3, Edge<Vector3>> graph)
    {
        int nodes = graph.VertexCount;
        int src = 1;
        max_length = 0;

        foreach(var vert in graph.Vertices)
        {
            //initialize visited dict that keeps track of visited vertices declare its values
            Dictionary<Vector3, bool> visited = new Dictionary<Vector3, bool>();
            foreach(var v in graph.Vertices)
            {
                visited.Add(v, false);
            }

            // Call DFS for vertex (node) vert
            DFS(graph, vert, vert, 0, visited, src);
            src++;
        }

        CreateLine(actualPathLineContainer.transform, "longest", new Vector3[] { startRoomPos + lineDrawOffset, endRoomPos + lineDrawOffset}, new Color(0, 0, 0), lineWidth, 0);
    }

    private void DFS(UndirectedGraph<Vector3, Edge<Vector3>> graph, Vector3 vertex, Vector3 startVertex, float prev_len, Dictionary<Vector3, bool> visited, int src)
    {
        // Mark the src node visited
        visited[vertex] = true;

        // curr_len is for length of cable from src city to its adjacent city
        float curr_len = 0;

        // Traverse all adjacent nodes
        foreach(var adjVert in graph.AdjacentVertices(vertex))
        {
            Edge<Vector3> currentEdge;
            graph.TryGetEdge(vertex, adjVert, out currentEdge);

            // If node or city is not visited
            if (!visited[adjVert])
            {
                // Total length of cable from src city to its adjacent
                curr_len = prev_len + Vector3.Distance(currentEdge.Target, currentEdge.Source);

                // Call DFS for adjacent city
                DFS(graph, adjVert, startVertex, curr_len, visited, src);
            }

            // If total cable length till now greater than previous length & adjacent is not connected to the start then update it (and the start and end rooms)
            if (max_length < curr_len && !graph.ContainsEdge(startVertex, adjVert))
            {
                startRoomPos = startVertex;
                endRoomPos = adjVert;
                max_length = curr_len;
            }

            // make curr_len = 0 for next adjacent
            curr_len = 0;
        }
    }

    private RoomHolder GetRoomAtPosition(Vector3 pos)
    {
        if (mainRooms.Count == 0) return null;

        foreach(var mainRoom in mainRooms)
        {
            if (mainRoom.roomObject.transform.position == pos) return mainRoom;
        }

        return null;
    }

    private void ReplacelastRoomWithBossRoom()
    {
        GameObject toDestroy = null;
        //remove the replaced room out of lists (for some reason normal remove not working hence I'm using remove at)
        for(int i = 0; i < mainRooms.Count; i++)
        {
            if (mainRooms[i].roomObject.transform.position == endRoomPos)
            {
                toDestroy = mainRooms[i].roomObject;
                mainRooms.RemoveAt(i);
                break;
            }
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].roomObject.transform.position == endRoomPos)
            {
                rooms.RemoveAt(i);
                break;
            }
        }

        Destroy(toDestroy);

        GameObject bossObject = Instantiate(bossRoomPrefab, mainRoomContainer.transform);
        bossObject.transform.position = endRoomPos;
        RoomHolder bossRoomHolder = new RoomHolder(rooms.Count, true, bossObject, Vector2.one);
        bossRoomHolder.type = EnclosureType.BossRoom;
        rooms.Add(bossRoomHolder);
        mainRooms.Add(bossRoomHolder);

        //spawn in the boss in it's room
        MainRoom mr = bossRoomHolder.roomObject.GetComponent<MainRoom>();
        mr.RemoveNormalRoomComponent();
        mr.SeperatedRoomState();

        foreach (var e in mstGraph.Edges)
        {
            if (e.Source == endRoomPos || e.Target == endRoomPos)
            {
                if (mainPathEdgesRooms.ContainsKey(e))
                {
                    mainPathEdgesRooms[e] = GetRoomHoldersAtEdge(e);
                }
            }
        }
    }

    private void SpawnPlayerWeapons()
    {
        foreach(var weapon in playerWeaponsToSpawnAtStart)
        {
            Vector2 pos = TruePointInCircle(playerSpawnPosition, 4f);
            Vector3 weaponPos = new Vector3(pos.x, playerSpawnPosition.y + 2f, pos.y);
            Instantiate(weapon, weaponPos, Quaternion.identity);
        }
    }
}
