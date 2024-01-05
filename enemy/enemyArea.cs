using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class enemyArea : MonoBehaviour
{
    enemyBehavier[] childs;
    [SerializeField] float rangeDetectOfEach = 5f;
    [SerializeField] Transform seccondChase;
    [SerializeField] Transform currentPlayerChase;
    void loadChild()
    {
        childs = GetComponentsInChildren<enemyBehavier>();
    }
    void chase(Transform go)
    {
        Debug.Log("chase player " + go.name);
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].chasePlayer(go);
        }
    }
    void unchase()
    {
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].returnToPosition();
        }
    }
    #region mono
    private void Start()
    {
        loadChild();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != PlayerController.Instance.player) return;
        Debug.Log("trigger enter ::" + other);
        if (other.gameObject != PlayerController.Instance.player) return;
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
        if (other.gameObject != PlayerController.Instance.player) return;
        if (seccondChase == null)
        {
            currentPlayerChase = null;
            unchase();
            return;
        }
        chase(seccondChase);
        currentPlayerChase = seccondChase;
    }
    #endregion
}
