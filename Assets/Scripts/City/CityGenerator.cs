using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    public float cityWidth = 20f;
    public float cityHeight = 20f;
    public float blockSize = 5f;
    public int roadInterval = 4;

    public float floorHeight = 3f;
    public int minBuildHeight = 5;
    public int maxBuildHeight = 15;

    [Range(0f, 1f)] public float towerChange = 0.02f;
    [Range(0f, 1f)] public float plazaChange = 0.05f;
    [Range(0f, 1f)] public float parkChange = 0.1f;

    private void Start()
    {
        GenerateCity();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearCity();
            GenerateCity();
        }
    }

    void GenerateCity()
    {
        CreateGround(); // ������
        CreateRoads();  // �� ����
        CreateBuildings();  // �ǹ�����
        CreateStreetLight();    // �����Y�� ��ġ
    }
    void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Ground";
        ground.transform.position = new Vector3(cityWidth * blockSize / 2f, -0.5f, cityHeight * blockSize / 2f);
        ground.transform.localScale = new Vector3(cityWidth * blockSize, 0.1f, cityHeight * blockSize);
        ground.transform.SetParent(transform);
        ground.GetComponent<Renderer>().material.color = new Color(0.3f, 0.3f, 0.3f);
    }

    void CreateRoadTile(int x, int z)       // �� Ÿ�� ����
    {
        GameObject ground = GameObject.CreatePrimitive (PrimitiveType.Cube);
        ground.name = $"Road{x}_{z}";
        ground.transform.position = new Vector3(x * blockSize , -0.05f, blockSize);
        ground.transform.localScale = new Vector3(  blockSize, 0.1f, blockSize);
        ground.transform.SetParent(transform);
        ground.GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.2f);
    }

    bool IsRoad(int x, int z)
    {
        return (x % roadInterval == 0) || (z % roadInterval == 0);
    }

    void CreateRoads()
    {
        for(int x = 0; x < cityWidth; x++)
        {
            for(int z = 0; z < cityHeight; z++)
            {
                if(IsRoad(x, z))
                {
                    CreateRoadTile(x, z);
                }
            }
        }
    }

    void CreateBuildings()
    {
        for(int x  = 0; x < cityWidth; x++)
        {
            for(int z = 0; z < cityHeight; z++)
            {
                if(!IsRoad(x, z))           //  ���� �������� Ÿ�� ���� ������ �߰�
                {
                    float rand = Random.value;

                    if(rand < towerChange)
                    {
                        CreateTower(x, z);
                    }
                    else if(rand < towerChange + plazaChange)
                    {
                        CreatePlaza(x, z);
                    }
                    else if(rand < towerChange + plazaChange + parkChange)
                    {
                        CreatePark(x, z);
                    }
                    else
                    {
                        CreateBuilding(x, z);
                    }
                }
            }
        }
    }
    void CreateBuilding(int x, int z)
    {
        int floors = Random.Range(minBuildHeight,maxBuildHeight);
        float height = floors * floorHeight;

        GameObject building = GameObject.CreatePrimitive (PrimitiveType.Cube);
        building.name = $"Building_{x}_{z}";
        building.transform.position = new Vector3(x * blockSize, height/2f,z * blockSize);
        building.transform.localScale = new Vector3(blockSize * 0.9f, height, blockSize * 0.9f);
        building.transform.SetParent(transform);
        building.GetComponent<Renderer>().material.color = GetBuildingColor(x, z);
    }

    Color GetBuildingColor(int x, int z)
    {
        float disFromCenter = Vector2.Distance(new Vector2(x, z), new Vector2(cityWidth / 2f, cityHeight / 2f));

        if (disFromCenter < cityWidth * 0.2f)
        {
            return new Color(0.6f, 0.6f, 0.7f);
        }
        else if (disFromCenter < cityWidth * 0.4f)
        {
            return new Color(0.8f, 0.7f, 0.7f);
        }
        else
        {
            return new Color(0.7f, 0.6f, 0.5f);
        }
    }

    void CreateTower(int x, int z)      // Ÿ�� ����
    {
        int floors = Random.Range(30, 50);
        float height = floors * floorHeight;

        GameObject tower = GameObject.CreatePrimitive (PrimitiveType.Cube);
        tower.name = $"Tower_{x}_{z}";
        tower.transform.position = new Vector3(x * blockSize, height / 2f, z * blockSize);
        tower.transform.localScale = new Vector3(blockSize * 0.8f, height, blockSize * 0.8f);
        tower.transform.SetParent(transform);
        tower.GetComponent<Renderer>().material.color = new Color(0.7f, 0.8f, 0.9f);

        GameObject topLight = new GameObject("TowerLight");
        topLight.transform.position = new Vector3(x * blockSize, height + 2f, z * blockSize);
        topLight.transform.SetParent(tower.transform);

        Light light = topLight.AddComponent<Light>();
        light.color = Color.red;
        light.intensity = 2f;
        light.range = 30f;
    }

    void CreatePark(int x, int z)       // ���� ����
    {
        GameObject park = GameObject.CreatePrimitive (PrimitiveType.Cube);
        park.name = $"Park{x}_{z}";
        park.transform.position = new Vector3(x * blockSize, 0.1f , z * blockSize);
        park.transform.localScale = new Vector3(blockSize * 0.8f, 0.2f, blockSize * 0.8f);
        park.transform.SetParent(transform);
        park.GetComponent<Renderer>().material.color = new Color(0.2f, 0.6f, 0.2f);

        for(int i = 0; i < 3; i++)
        {
            Vector3 treePos = new Vector3(
                x * blockSize + Random.Range(-blockSize * 0.3f, blockSize * 0.3f),
                2f,
                z * blockSize + Random.Range(-blockSize * 0.3f, blockSize * 0.3f)
                );

            GameObject tree = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
            tree.transform.position = treePos;
            tree.transform.localScale = new Vector3(1f,2f,1f);
            tree.transform.SetParent(park.transform);
            tree.GetComponent<Renderer>().material.color = new Color(0.1f, 0.4f, 0.1f);
        }
    }

    void CreatePlaza(int x, int z)
    {
        GameObject plaza = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plaza.name = $"Plaza{x}_{z}";
        plaza.transform.position = new Vector3(x * blockSize, 0.1f, z * blockSize);
        plaza.transform.localScale = new Vector3(blockSize, 0.2f, blockSize);
        plaza.transform.SetParent(transform);
        plaza.GetComponent<Renderer>().material.color = new Color(0.9f, 0.9f, 0.85f);

        GameObject fountain = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        fountain.transform.position = new Vector3(x * blockSize,1f,z * blockSize);
        fountain.transform.localScale = new Vector3(2f, 1f, 2f);
        fountain.transform.SetParent(plaza.transform);
        fountain.GetComponent<Renderer>().material.color = Color.cyan;
    }

    void CreateStreetLight(int x, int z)
    {
        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.name = $"Light{x}_{z}";
        pole.transform.position = new Vector3(x * blockSize, 5f, z * blockSize);
        pole.transform.localScale = new Vector3(0.2f, 5f, 0.2f);
        pole.transform.SetParent(transform);
        pole.GetComponent<Renderer>().material.color = Color.gray;

        GameObject bulb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bulb.transform.position = new Vector3(x * blockSize, 10f, z * blockSize);
        bulb.transform.localScale = Vector3.one * 0.5f;
        bulb.transform.SetParent(pole.transform);
        bulb.GetComponent<Renderer>().material.color = Color.yellow;

        Light light = bulb.AddComponent<Light>();
        light.color = new Color(1f, 0.95f, 0.8f);
        light.intensity = 1f;
        light.range = 20f;
    }

    void CreateStreetLight()
    {
        for(int x = 0; x < cityWidth; x += roadInterval)
        {
            for(int z = 0; z < cityHeight; z += roadInterval)
            {
                CreateStreetLight(x, z);
            }
        }
    }

    void ClearCity()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
