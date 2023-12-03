using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class enemyArea : NetworkObjectPool
{
    enemyBehavier[] childs;
    [SerializeField] float rangeDetectOfEach = 5f;
    [SerializeField] Transform seccondChase;
    [SerializeField] Transform currentPlayerChase;
    void loadChild()
    {
        int length = transform.childCount;
        childs = new enemyBehavier[length];
        for (int i = 0; i < length; i++)
        {
            childs[i] = transform.GetChild(i).gameObject.GetComponent<enemyBehavier>();
            Debug.Log("childs " + i + " : " + childs[i].name);
        }
    }
    void chase(Transform go)
    {
        Debug.Log("chase player " + go.name);
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].chasePlayer(go);
        }
    }
    #region mono
    private void Start()
    {
        loadChild();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger enter ::" + other);
        //tat ca deu xoay nhin player? 
        var plobj = other.gameObject.transform;
        Debug.Log("childs cout :" + childs.Length);
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].gameObject.transform.LookAt(plobj);
        }
        // comment to test under line
        //if (other.gameObject != PlayerController.Instance.player) return;
        if (!other.gameObject.TryGetComponent(out ControllReceivingSystem _)) return;
        //xu li cac child chase player
        if (currentPlayerChase == null)
        {
            currentPlayerChase = other.gameObject.transform;
            chase(currentPlayerChase);
        }
        else
        {
            Debug.Log("da track player khac nen se them vao second chase");
            seccondChase = other.gameObject.transform;
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Character1ControlSystem _)) return;
        if (seccondChase == null)
        {
            currentPlayerChase = null;
            return;
        }
        chase(seccondChase);
        currentPlayerChase = seccondChase;
    }
    #endregion
}
