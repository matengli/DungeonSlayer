using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DamageNumbersPro;
using DG.Tweening;
using DungeonSlayer.Script.Common.Game;
using ModestTree;
using Ricimi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// 处理UI的内容，主要是血条之类的东西
/// </summary>
public class ActorUIContainer : MonoBehaviour
{
    private Canvas _canvas;

    private Camera _camera;

    private GameObject barPrefab;

    private Dictionary<string, Slider> barNameDict;
    private Dictionary<string, TextMeshProUGUI> barLabelDict;

    [Inject] private ActorAttributeMgr _attributeMgr;

    [Inject] private ActorBattleMgr _battleMgr;

    public void InitUI()
    {
        groupTrans = transform.Find("Canvas/Group");

        _canvas = GetComponentInChildren<Canvas>();
        _camera = Camera.main;
        
        for (int i = 0; i < groupTrans.childCount; i++)
        {
            var item = groupTrans.GetChild(i);
            item.gameObject.SetActive(false);
        }

        barNameDict = new Dictionary<string, Slider>();
        barLabelDict = new Dictionary<string, TextMeshProUGUI>();

        foreach (var name in _attributeMgr.GetAttrNames())
        {
            var bar = groupTrans.GetChild(propNameList.IndexOf(name));
            bar.gameObject.SetActive(true);
            barNameDict.Add(name, bar.GetComponent<Slider>() );
            barLabelDict.Add(name, bar.GetComponentInChildren<TextMeshProUGUI>());
            
            ModifyVal(name, _attributeMgr.GetVal(name), _attributeMgr.GetMaxVal(name));

        }
        _attributeMgr.OnModifyAttrEvent += this.OnAttrModify;

        _battleMgr.OnKilled += (DamageInfo info) =>
        {
            _canvas.gameObject.SetActive(false);
        };
        
        if(_canvas.transform.Find("name") == null)
            return;

        debugText = _canvas.transform.Find("name").GetComponent<TextMeshProUGUI>();
    }

    private void OnAttrModify(string name, float curval, float oldval, float max)
    {
        ModifyVal(name, curval, max);

        DamageNumber obj;
        if (curval > oldval)
        {
            obj = Resources.Load<DamageNumber>("Prefab/DamageNumber");
        }
        else
        {
            obj = Resources.Load<DamageNumber>("Prefab/HealNumber");
        }
        
        obj.Spawn(transform.position,    Mathf.Abs(curval - oldval));
    }

    private TextMeshProUGUI debugText;

    [Inject] private ActorStateMgr _stateMgr;
    [Inject] private ActorAbilityMgr _abilityMgr;

    [Inject] private GameConfig _gameConfig;

    // Update is called once per frame
    void LateUpdate()
    {
        if(_canvas==null)
            return;
        
        _canvas.transform.forward = _camera.transform.forward;

        if (debugText == null)
        {
            return;
        }

        if (!_gameConfig.IsShowDebugInfo)
        {
            debugText.text = "";
            return;
        }

        debugText.text = $"Pos:{transform.position}\n" +
                         $"Rotation:{transform.rotation}\n" +
                         $"DataString:{_attributeMgr.DataString}\n" +
                         $"State:{_stateMgr.GetCurrentState().Name}\n"+
                         $"Ability:{_abilityMgr.GetCurrentAbility()?.Name}\n"
                         ;
        debugText.color = _campMgr.GetCamp() == ActorCampMgr.ActorCamp.Dire ? Color.red : Color.white;
    }

    [Inject] private ActorCampMgr _campMgr;

    private string[] propNameList = { "hp"};
    private Transform groupTrans;

    public void ModifyVal(string name, float val, float maxVal = 1.0f)
    {
        if (!barNameDict.ContainsKey(name))
        {
            return;
        }

        Debug.Log($"{name}:{val}/{maxVal}");
        barNameDict[name].value = (val / maxVal) * 100.0f;

        if (barLabelDict.ContainsKey(name))
        {
            barLabelDict[name].text = val.ToString();
        }
    }

    public void SetViewActive(bool status)
    {
        if (_canvas == null)
        {
            _canvas = GetComponentInChildren<Canvas>();
        }
        
        _canvas.transform.localScale = status ? UnityEngine.Vector3.one*0.01f : UnityEngine.Vector3.zero;

    }

    private GameObject WorldUI;
}
