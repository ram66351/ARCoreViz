using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EdgeComponent : MonoBehaviour
{
    private LineRenderer lr;
    public float initWidth;
    public Transform StartNode;
    public Transform EndNode;
    public DateTime startDate;
    public DateTime endDate;
    public string str_startDate;
    public string str_endDate;
    public int spreadTime;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init(Transform startNode, Transform endNode)
    {
        lr = GetComponent<LineRenderer>();
        initWidth = lr.startWidth;
        CovidNetworkManager.ScalingNowEvent += adjustWidth;
        StartNode = startNode;
        EndNode = endNode;
        transform.SetParent(StartNode);
        StartCoroutine(Realign());

        startDate = StartNode.GetComponent<NodeInstance>().MyCollectionDate;
        endDate = EndNode.GetComponent<NodeInstance>().MyCollectionDate;

        if (startDate != null && endDate !=null)
            spreadTime = (endDate - startDate).Minutes;
        
    }

    void OnApplicationQuit()
    {
        CovidNetworkManager.ScalingNowEvent -= adjustWidth;
    }

    void adjustWidth(float dtScale)
    {
        lr.widthMultiplier = dtScale;
        adjustPosition();
        Debug.Log("Scaling now -> adjustWidth");
    }

    IEnumerator Realign()
    {
        yield return new WaitForSeconds(5);
        adjustPosition();
    }

    void adjustPosition()
    {
        lr.SetPosition(0, Vector3.zero);
        lr.SetPosition(1, EndNode.transform.localPosition);// - StartNode.transform.position);
    }
}
