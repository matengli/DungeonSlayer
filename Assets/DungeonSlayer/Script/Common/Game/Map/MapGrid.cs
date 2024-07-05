using System;
using TMPro;
using UnityEngine;

namespace DungeonSlayer.Script.Common.Game.Map
{
public class MapGrid<TObject>
{
    private int width;
    private int height;
    private float cellSize;
    private TObject[,] gridArray;
    private Vector3 posOffset;
    
    public event Action<int, int, TObject> OnGridValueChange; 

    public MapGrid(int width, int height, float cellSize, Vector3 centerOffset)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridArray = new TObject[width, height];

        GameObject parent = new GameObject("GridParent");
        parent.transform.position = Vector3.zero;
        parent.transform.rotation = Quaternion.identity;
        
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x,y] = default(TObject);

                var result = GameObject.FindObjectOfType<GameUtil>().AddQuadToWorldPostion(GetWorldPosition(x, y));
                result.transform.parent.parent = parent.transform;
            }
        }

        posOffset = centerOffset;
    }

    public Vector3 GetWorldPosition(int x,int y)
    {
        return new Vector3(x, 0, y) * cellSize + posOffset;
    }

    public void GetGridXY(Vector3 position, out int x, out int y)
    {
        var offset = new Vector3(position.x, 0, position.z) - posOffset - new Vector3(-cellSize/2, 0 ,-cellSize/2);
        x = Mathf.FloorToInt(offset.x/cellSize);
        y = Mathf.FloorToInt(offset.z/cellSize);
    }

    public void SetValue(int x, int y, TObject val)
    {
        if(x >= gridArray.GetLength(0)||y >= gridArray.GetLength(1)||x<0||y<0)
            return;
        
        gridArray[x, y] = val;

        TriggerGridValueChange(x, y);
    }

    public void TriggerGridValueChange(int x, int y)
    {
        if (OnGridValueChange != null) OnGridValueChange(x, y, gridArray[x, y]);
    }

    public void SetValue(Vector3 worldPos, TObject val)
    {
        int x, y;
        GetGridXY(worldPos, out x, out y);
        
        SetValue(x,y, val);
    }

    public TObject GetValue(int x, int y)
    {
        if(x >= gridArray.GetLength(0)||y >= gridArray.GetLength(1)||x<0||y<0)
            return default(TObject);

        return gridArray[x, y];
    }

    public TObject GetValue(Vector3 worldPos)
    {
        int x, y;
        GetGridXY(worldPos, out x, out y);
        
        return GetValue(x,y);
    }

    public void DrawGizmo()
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                var pos = GetWorldPosition(x, y);
                var dis = cellSize / 2;
                Debug.DrawLine(pos+new Vector3(-dis, 0 , dis), pos+new Vector3(dis, 0 , dis));
                Debug.DrawLine(pos+new Vector3(-dis, 0 , dis), pos+new Vector3(-dis, 0 , -dis));
                Debug.DrawLine(pos+new Vector3(-dis, 0 , -dis), pos+new Vector3(dis, 0 , -dis));
                Debug.DrawLine(pos+new Vector3(dis, 0 , -dis), pos+new Vector3(dis, 0 , dis));
                
            }
        }
    }

    public int GetWidth()
    {
        return width;
    }
    
    public int GetHeight()
    {
        return height;
    }
}

}