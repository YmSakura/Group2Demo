using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool 
{
    private static ObjectPool instance;
    private Dictionary<string,Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    private GameObject pool; 
    public static ObjectPool Instance//��������ʵ���Ա���ȡ����
    {
        get 
        { 
            if(instance == null)
            {
                instance = new ObjectPool();
            }
        return instance;
        }
    }



    public GameObject GetObject(GameObject prefab)
    {
        GameObject _object;
        string _name = prefab.name + "Pool";
        if (!objectPool.ContainsKey(_name)||objectPool[_name].Count==0)  
            //��������ڶ�Ӧ�ػ��������Ϊ0����ʵ�������������
        {
            _object = GameObject.Instantiate(prefab);
            PushObject(_object);
            if(pool == null)                                            //����޸����򴴽����ش洢�ӳ�
            {
                pool = new GameObject("ObjectPool");
            }
            GameObject childPool =GameObject.Find(_name);  //Ѱ���ӳ�
            if (!childPool)                                             //��������ڶ�Ӧ������ӳ��򴴽��ӳز������ӳذ󶨸��ӹ�ϵ
            {
                childPool = new GameObject(_name);
                childPool.transform.SetParent(pool.transform);
            }
            _object.transform.SetParent(childPool.transform);           //����������Ϊ�ӳص�����Ʒ
        }
        _object = objectPool[_name].Dequeue();                          //���ӳ���ȡ������
        _object.SetActive(true);                                        //ʹʵ������
        return _object;                                                 //����ʵ���Թ�ʹ��
    }


    //��ʵ���Ƶ�����ص���
    public void PushObject(GameObject prefab)
    {
        string _name=prefab.name.Replace("(Clone)","Pool");
        if (!objectPool.ContainsKey(_name))                             //�����Ƿ���ڶ�Ӧ����أ��������򴴽�
        {
            objectPool.Add(_name,new Queue<GameObject>());
        }
        objectPool[_name].Enqueue(prefab);                              //���·ŵ�������
        prefab.SetActive(false);                                        //ʹʵ��ʧ��
    }
}
