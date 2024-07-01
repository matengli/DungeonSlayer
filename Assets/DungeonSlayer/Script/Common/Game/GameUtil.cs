using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using Zenject;

public class GameUtil : MonoBehaviour
{
    [SerializeField] private GameObject ToolTip;

    public TextMeshPro AddTextToWorldPosition(Vector3 position, string content)
    {
        var text = Instantiate(Resources.Load<GameObject>("Prefab/DebugText"), position, Quaternion.identity);
        var t = text.GetComponentInChildren<TextMeshPro>();
        t.text = content;
        return t;
    }

    public MeshRenderer AddQuadToWorldPostion(Vector3 position)
    {
        var text = Instantiate(Resources.Load<GameObject>("Prefab/Quad"), position, Quaternion.identity);
        var t = text.GetComponentInChildren<MeshRenderer>();
        return t;
    }

    public Vector3 GetMouseWorldPosition()
    {
        foreach (var item in Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 1000.0f, LayerMask.GetMask("ClickPlane")))
        {
            if (item.collider.gameObject.name == "ClickPlane")
            {
                return item.point;
            }
        }
        
        return Vector3.zero;
    }

    public void ModifySectorMeshAtPoint(GameObject go,float radius, float angle, Vector3 position, int segments=50)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        // 中心点
        vertices[0] = Vector3.zero;

        // 扇形边界点
        float startAngle = -Mathf.Deg2Rad * (angle*0.5f - 90.0f);
        float angleStep = Mathf.Deg2Rad * angle / segments;
        for (int i = 1; i <= segments + 1; i++)
        {
            float radian = i * angleStep + startAngle;
            float x = Mathf.Cos(radian);
            float z = Mathf.Sin(radian);
            vertices[i] = new Vector3(x, 0f, z) * radius;
        }

        // 扇形三角形索引
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 2;
            triangles[i * 3 + 2] = i + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        go.GetComponent<MeshFilter>().mesh = mesh;

        go.GetComponent<Renderer>().material = Resources.Load<Material>("Mat/DafaultTransparentColor");

        go.transform.position = position;
    }

    public GameObject CreateSectorMeshAtPoint(float radius, float angle, Vector3 position, int segments=50)
    {
        var go = new GameObject();
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();

        ModifySectorMeshAtPoint(go, radius, angle, position, segments);

        return go;
    }
    
    public GameObject CreateArrowAtPoint(Vector3 start, Vector3 end, Color color)
    {
        var go = Instantiate(Resources.Load<GameObject>("Prefab/Arrow"));
        go.transform.position = start;
        go.transform.LookAt(end);
        go.transform.localScale = new Vector3(1, 1, (end - start).magnitude);

        foreach (var renderer in go.GetComponentsInChildren<Renderer>())
        {
            renderer.material.color = color;
        }
        
        return go;
    }

    private Vector3 originalToolTipPos = Vector3.zero;
    public void SetRectTransformWithWorldPosition(RectTransform rect, Vector3 worldPosition)
    {
        // 将世界坐标转换为屏幕坐标
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
        
        // 将屏幕坐标转换为UGUI坐标
        Vector2 uiPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect.parent as RectTransform, screenPosition, null, out uiPosition);
        
        // 设置RectTransform的坐标
        rect.anchoredPosition = uiPosition + new Vector2((rect.pivot.x-0.5f) * rect.sizeDelta.x, (rect.pivot.y-0.5f) * rect.sizeDelta.y);
    }

    public List<RaycastHit> RaycastSectorRange(Vector3 origin, Vector3 forwardDir, float disRange ,float degRange, int LayerMask, Collider ignoreCollider)
    {
        List<RaycastHit> hits = new List<RaycastHit>();

        List<Vector3> lines = new List<Vector3>();
        
        for (int i = 0; i< 100; i++)
        {
            float y = -degRange / 2 + i*(degRange/99);
            Vector3 dir =  Quaternion.Euler(0,y,0) * forwardDir.normalized;
            
            lines.Add(dir*disRange + origin);
            lines.Add(origin);
                
            var result = Physics.RaycastAll(origin, dir, disRange, LayerMask);

            foreach (var item in result)
            {
                if(item.collider==ignoreCollider)
                    continue;
                
                hits.Add(item);
            }
        }
        
        return hits;
    }
    
    // [Inject(Id = "PlayerInfo")] private TextMeshProUGUI _playerInfo;

    public void ShowPlayerInfo(string lis)
    {
        // _playerInfo.text = lis;
    }

    private Vector3 mousePosition;
    /// <summary>
    /// 检查是否点击了鼠标位置，不包含UI
    /// </summary>
    /// <returns></returns>
    public bool CheckClickPos()
    {
        if (IsHoverOverUI())
            return false;
        
        if (Input.GetMouseButtonDown(0))
        {
            mousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if((mousePosition-Input.mousePosition).magnitude>50.0f)
                return false;

            return true;
        }

        return false;
    }
    
    [Inject] private CameraController _cameraController;
    
    /// <summary>
    /// 当前是否鼠标位置在UI之上
    /// </summary>
    /// <returns></returns>
    public bool IsHoverOverUI()
    {
        var lastRaycastResult = FindAnyObjectByType<InputSystemUIInputModule>().GetLastRaycastResult(0);
        if (lastRaycastResult.gameObject != _cameraController.gameObject)
        {
            return true;
        }

        return false;
    }
}
