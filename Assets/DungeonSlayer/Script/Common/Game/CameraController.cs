using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

/// <summary>
/// 镜头控制，需要放置在Canvas下可以Raycast的UI上
/// </summary>
public class CameraController : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Inject][SerializeField] private CinemachineCameraOffset normalCameraOffset;

    [Tooltip("视角拖动后缓动的时间")][SerializeField] private float easeTime = 0.4f;
    [Tooltip("视角拖动后缓动的距离参数")][Range(0,0.5f)][SerializeField] private float easeDistanceFactor = 0.4f;
    
    [Tooltip("视角移动的灵敏度")][Range(0,0.1f)][SerializeField]private float moveSpeed = 0.01f; // 视角拖动速度
    [Tooltip("视角转动的灵敏度")][Range(0.0f,50.0f)][SerializeField]private float rotationSpeed = 0.01f; // 视角旋转速度

    private Vector2 lastMousePosition; // 上一帧的鼠标位置

    private void Start()
    {
        ResetCameraOffset();
    }

    private void Update()
    {
        
        // 根据鼠标输入旋转视角
        if (Input.GetMouseButtonDown(0))
        {
            normalCameraOffset.DOKill();
            lastMousePosition = Input.mousePosition;
        }
        
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        float zoomAmount = scrollWheel * zoomSpeed;
        
        // 根据滚轮输入缩放摄像机
        if (Mathf.Abs(zoomAmount) > 0.01f)
        {
            float newF = normalCameraOffset.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize +  zoomAmount;
            
            normalCameraOffset.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = Mathf.Clamp(newF, minZoom, maxZoom);;
            
        }

        if (Input.GetKey(KeyCode.Q))
        {
            var old = normalCameraOffset.transform.rotation.eulerAngles;
            old.y += Time.deltaTime * rotationSpeed;
            normalCameraOffset.transform.rotation = Quaternion.Euler(old);
        }
        
        if (Input.GetKey(KeyCode.E))
        {
            var old = normalCameraOffset.transform.rotation.eulerAngles;
            old.y -= Time.deltaTime * rotationSpeed;
            normalCameraOffset.transform.rotation = Quaternion.Euler(old);
        
        }
        
    }
    
    public float zoomSpeed = 10f; // 缩放速度
    public float minZoom = 5f;    // 最小缩放值
    public float maxZoom = 20f;   // 最大缩放值
    
    public void OnDrag(PointerEventData eventData)
    {
        // 计算鼠标移动的增量
        Vector2 delta = (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - lastMousePosition) * moveSpeed;
    
        // 设置CinemachineCameraOffset的偏移量
        CinemachineCameraOffset cameraOffset = normalCameraOffset;
        
        cameraOffset.m_Offset.x += -delta.x;
        cameraOffset.m_Offset.y += -delta.y;
        
    
        // 更新上一帧的鼠标位置
        lastMousePosition = Input.mousePosition;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 delta = (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - lastMousePosition);
    
        normalCameraOffset.DOKill();
        DOTween.To(() => 0f, (t) =>
        {
            normalCameraOffset.m_Offset.x -= delta.x*moveSpeed*(1-t)*easeDistanceFactor;
            normalCameraOffset.m_Offset.y -= delta.y*moveSpeed*(1-t)*easeDistanceFactor;
        }, 1.0f, easeTime).SetTarget(normalCameraOffset);
    }

    public void ResetCameraOffset()
    {
        normalCameraOffset.m_Offset = new Vector3(0,0,-30);
    }

    public Transform GetFollowTarget()
    {
        return normalCameraOffset.GetComponent<CinemachineVirtualCamera>().Follow;
    }
    
    public void SetFollowTarget(Transform target)
    {
        ResetCameraOffset();
        normalCameraOffset.GetComponent<CinemachineVirtualCamera>().Follow = target;
    }

    public void ShakeCamera()
    {
        GetComponent<CinemachineImpulseSource>().GenerateImpulseWithForce(0.15f);
    }
}
