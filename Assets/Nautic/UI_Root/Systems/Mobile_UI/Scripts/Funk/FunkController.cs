using System;
using System.Collections;
using System.Collections.Generic;
using Groupup;
using TMPro;
using UnityEngine;

public class FunkController : MonoBehaviour
{
    [SerializeField] private FunkMessage _radioMessagePrefab;
    [SerializeField] private Transform _funkHolder;

    private RectTransform _ownTransform;
    private List<string> _funkMessages;

    public void Awake()
    {
        _ownTransform = GetComponent<RectTransform>();
        _funkMessages = new List<string>();
        
        ResourceManager.GetInterface<UI_RootInterface>().OnSetFunkMessage += SetFunkMessage;
    }

    public void SetFunkMessage(string msg)
    {
        _funkMessages.Add(msg);
        
        if (_funkHolder.childCount == 0)
            SpawnNextMessage();
    }

    private void SpawnNextMessage()
    {
        if (_funkMessages.Count == 0)
            return;
        
        FunkMessage funkMsg = Instantiate(_radioMessagePrefab, _ownTransform.position + new Vector3(_ownTransform.rect.x, 0, 0), Quaternion.identity, _funkHolder);
        funkMsg.Init(_funkMessages[0], SpawnNextMessage);
        _funkMessages.RemoveAt(0);
    }
}
