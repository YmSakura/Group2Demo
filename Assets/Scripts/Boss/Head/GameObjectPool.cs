using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool 
{
    private static GameObjectPool poolInstance;
    public static GameObjectPool PoolInstance//建立公有实例以便提取调用
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
        if (Pool.Count==0)                  //如果可用数量为0，则实例化后推入池中
        {
            _fireBall = GameObject.Instantiate(prefab);
            PushBullet(_fireBall);
        }
        _fireBall=Pool.Dequeue();           //从池中取出物体
        _fireBall.SetActive(true);          //使实例激活
        return _fireBall;                   //返回实例以供使用
    }


    //将实例推到对象池当中
    public void PushBullet(GameObject prefab)
    {
        Pool.Enqueue(prefab);               //重新放到队列中
        prefab.SetActive(false);            //使实例失活
    }
}
