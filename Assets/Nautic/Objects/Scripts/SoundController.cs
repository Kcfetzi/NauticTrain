using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    
    
    // id = 0
    [SerializeField] private AudioClip _bell3_5_3;
    // id = 1
    [SerializeField] private AudioClip _bell3_5_3_Gong;
    // id = 2
    [SerializeField] private AudioClip _bell5;
    // id = 3
    [SerializeField] private AudioClip _bell5_Gong_5;
    // id = 4
    [SerializeField] private AudioClip _bell5_Gong_5_Typhon;
    // id = 5
    [SerializeField] private AudioClip _bell5_Gong_5_Typhon_Short_Long_Short;
    // id = 6
    [SerializeField] private AudioClip _bell5_Typhon_Short_Short_Short;
    // id = 7
    [SerializeField] private AudioClip _typhon_Long;
    // id = 8
    [SerializeField] private AudioClip _typhon_Long_Short_Short;
    // id = 9
    [SerializeField] private AudioClip _typhon_Long_Short_Short_Short_Short;
    // id = 10
    [SerializeField] private AudioClip _typhon_Long_Short_Short_Short_Long_Short_Short;
    // id = 11
    [SerializeField] private AudioClip _typhon_Long_Long_Long;
    // id = 12
    [SerializeField] private AudioClip _typhon_Long_Long_Short_Short_Short_Short;
    

    public void PlaySound(int id)
    {
        switch (id)
        {
            case 0:
                _audioSource.clip = _bell3_5_3;
                break;
            case 1:
                _audioSource.clip = _bell3_5_3_Gong;
                break;
            case 2:
                _audioSource.clip = _bell5;
                break;
            case 3:
                _audioSource.clip = _bell5_Gong_5;
                break;
            case 4:
                _audioSource.clip = _bell5_Gong_5_Typhon;
                break;
            case 5:
                _audioSource.clip = _bell5_Gong_5_Typhon_Short_Long_Short;
                break;
            case 6:
                _audioSource.clip = _bell5_Typhon_Short_Short_Short;
                break;
            case 7:
                _audioSource.clip = _typhon_Long;
                break;
            case 8:
                _audioSource.clip = _typhon_Long_Short_Short;
                break;
            case 9:
                _audioSource.clip = _typhon_Long_Short_Short_Short_Short;
                break;
            case 10:
                _audioSource.clip = _typhon_Long_Short_Short_Short_Long_Short_Short;
                break;
            case 11:
                _audioSource.clip = _typhon_Long_Long_Long;
                break;
            case 12:
                _audioSource.clip = _typhon_Long_Long_Short_Short_Short_Short;
                break;
        }
        
        _audioSource.Play();
    }
}
