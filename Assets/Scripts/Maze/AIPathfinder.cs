using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPathfinder : MonoBehaviour
{
    [Header("AI 설정")]
    public float moveSpeed = 3f;
    public Color aiColor = Color.blue;

    [Header("경로 시각화")]
    public bool showPath = true;
    public Color pathPreviewColor = Color.green;

    private List<MazeCell> currentPath;
    private int pathIndex = 0;
    private bool isMoving = false;
    private Vector3 targetPostion;

    void Start()
    {
        GetComponent<Renderer>().material.color = aiColor;
        targetPostion = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            StartPathfinding();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPostion();
        }
        if (isMoving)
        {
            MoveAlongPath();
        }
    }

    List<MazeCell> GetAccessibleNeighbors(MazeCell cell)
    {
        List<MazeCell> neighbors = new List<MazeCell>();
        MazeGenerator gen = MazeGenerator.Instance;

        // 왼쪽
        if (cell.x > 0 && !cell.leftWall.activeSelf)
            neighbors.Add(gen.GetCell(cell.x - 1, cell.z));
        // 오른쪽
        if (cell.x < gen.width - 1 && !cell.rightWall.activeSelf)
            neighbors.Add(gen.GetCell(cell.x + 1, cell.z));
        // 아래
        if (cell.z > 0 && !cell.bottomWall.activeSelf)
            neighbors.Add(gen.GetCell(cell.x, cell.z -1));
        // 위
        if (cell.z < gen.height && !cell.topWall.activeSelf)
            neighbors.Add(gen.GetCell(cell.x, cell.z + 1));

        return neighbors;
    }

    // 방문 상태 초기화
    void ResetVisited()
    {
        MazeGenerator gen = MazeGenerator.Instance;

        for(int x = 0; x < gen.width; x++)
        {
            for(int z = 0; z < gen.height; z++)
            {
                MazeCell cell = gen.GetCell(x, z);
                cell.visited = false;
            }
        }
    }

    // 위치 초기화
    public void ResetPostion()
    {
        transform.position = new Vector3(0, transform.position.y, 0);
        targetPostion = transform.position;
        isMoving = false;
        pathIndex = 0;

        // 경로 색상 지우기
        if(currentPath != null)
        {
            foreach(MazeCell cell in currentPath)
            {
                cell.SetColor(Color.white);
            }
        }

        currentPath = null;
    }

    // BFS 알고리즘으로 경로 찾기
    List<MazeCell> FindPathBFS(MazeCell start, MazeCell end)
    {
        ResetVisited();

        Queue<MazeCell> queue = new Queue<MazeCell>();
        Dictionary<MazeCell,MazeCell> parentMap = new Dictionary<MazeCell, MazeCell>();

        start.visited = true;
        queue.Enqueue(start);
        parentMap[start] = null;

        bool found = false;

        // BFS 탐색
        while (queue.Count > 0)
        {
            MazeCell current = queue.Dequeue();

            if(current == end)
            {
                found = true;
                break;
            }

            List<MazeCell> neighbors = GetAccessibleNeighbors(current);

            foreach(MazeCell neighbor in neighbors)
            {
                if (!neighbor.visited)
                {
                    neighbor.visited = true;
                    queue.Enqueue(neighbor);
                    parentMap[neighbor] = current;
                }
            }
        }
        if (found)
        {
            List<MazeCell> path = new List<MazeCell>();
            MazeCell current = end;


            while (current != null)
            {
                path.Add(current);
                current = parentMap[current];
            }

            path.Reverse();
            return path;
        }

        return null;
    }
    
    public void StartPathfinding()
    {
        MazeGenerator gen = MazeGenerator.Instance;

        // 현재 위치에서 가장 가까운 셀 찾기
        int startX = Mathf.RoundToInt(transform.position.x / gen.cellSize);
        int startZ = Mathf.RoundToInt(transform.position.z / gen.cellSize);

        MazeCell start = gen.GetCell(startX, startZ);
        MazeCell end = gen.GetCell(gen.width - 1, gen.height - 1);

        if(start ==  null || end == null)
        {
            Debug.LogError("시작점이나 끝점이 없습니다");
            return;
        }
        
        currentPath = FindPathBFS(start,end);

        if(currentPath != null && currentPath.Count > 0)
        {
            Debug.Log($"경로 찾기 성공 거리 : {currentPath.Count}");

            if (showPath)
            {
                ShowPathPreview();
            }
            pathIndex = 0;
            isMoving = true;
        }
    }

    void ShowPathPreview()
    {
        foreach (MazeCell cell in currentPath)
        {
            cell.SetColor(pathPreviewColor);
        }
    }

    void MoveAlongPath()
    {
        if(pathIndex >= currentPath.Count)
        {
            Debug.Log("목표 도착");
            isMoving = false;
            return;
        }
        MazeCell targetCell = currentPath[pathIndex];
        targetPostion = new Vector3(targetCell.x * MazeGenerator.Instance.cellSize,transform.position.y,targetCell.z * MazeGenerator.Instance.cellSize);
        transform.position = Vector3.MoveTowards(transform.position,targetPostion,moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPostion) < 0.01f)
        {
            transform.position = targetPostion;
            pathIndex++;
        }
    }
}
