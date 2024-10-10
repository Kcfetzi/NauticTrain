using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestPool : MonoBehaviour
{
    [SerializeField] private Bullet prefab;
    Queue<Bullet> bullets = new Queue<Bullet>();


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.A))
            bullets.Enqueue(prefab.Get<Bullet>());
        if (Input.GetKeyDown(KeyCode.D))
            bullets.Dequeue().Release();
    }
}
