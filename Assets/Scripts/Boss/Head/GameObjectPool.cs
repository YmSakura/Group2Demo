using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool 
{
    private static GameObjectPool poolInstance;
    public static GameObjectPool PoolInstance//��������ʵ���Ա���ȡ����
    {
        get { if(poolInstance == null)
            {
                poolInstance = new GameObjectPool();
            }
        return poolInstance;
        }
    }


    public static GameObject BulletPrefab;
    private Queue<GameObject> Pool = new Queue<GameObject>();



    public GameObject GetBullet(GameObject prefab)
    {
        GameObject _fireBall;
        if (Pool.Count==0)                  //�����������Ϊ0����ʵ�������������
        {
            _fireBall = GameObject.Instantiate(prefab);
            PushBullet(_fireBall);
        }
        _fireBall=Pool.Dequeue();           //�ӳ���ȡ������
        _fireBall.SetActive(true);          //ʹʵ������
        return _fireBall;                   //����ʵ���Թ�ʹ��
    }


    //��ʵ���Ƶ�����ص���
    public void PushBullet(GameObject prefab)
    {
        Pool.Enqueue(prefab);               //���·ŵ�������
        prefab.SetActive(false);            //ʹʵ��ʧ��
    }
}
