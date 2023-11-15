using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class enemyArea : NetworkObjectPool
{
    enemyBehavier[] childs;
    [SerializeField] float rangeDetectOfEach = 5f;
    [SerializeField] Stack<GameObject> chaseStack;
    [SerializeField] GameObject currentPlayerChase;
    void loadChild()
    {
        int length = transform.childCount;
        childs = new enemyBehavier[length];
        for (int i = 0; i < length; i++)
        {
            childs[i] = transform.GetChild(i).gameObject.GetComponent<enemyBehavier>();
        }
    }
    void chase(GameObject go)
    {

    }
    #region mono
    private void Start()
    {
        //
        loadChild();
    }

    private void OnTriggerEnter(Collider other)
    {
        //tat ca deu xoay nhin player? 
        var plobj = other.gameObject.transform;
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].gameObject.transform.LookAt(plobj);
        }
        if (!other.gameObject.TryGetComponent(out Character1ControlSystem _)) return;
        //xu li cac child chase player
        if (currentPlayerChase == null)
        {
            chase(other.gameObject);
            currentPlayerChase = other.gameObject;
        }
        else
        {
            chaseStack.Push(other.gameObject);
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Character1ControlSystem _)) return;
        if (chaseStack.Count <= 0)
        {
            currentPlayerChase = null;
            return;
        }
        GameObject chasePl = chaseStack.Pop();
        chase(chasePl);
        currentPlayerChase = chasePl;
    }
    #endregion
}
