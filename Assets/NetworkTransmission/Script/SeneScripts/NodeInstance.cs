using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class NodeInstance : MonoBehaviour
{
    private int myLevel;
    public string ID;
    public GameObject NodeRep;
    public GameObject Edge; 
    bool AmIvisible = true;
    public NodeMetaInfo metaInfo;
    public TextMesh Country;
    public string Date;
    public DateTime MyCollectionDate;

    public Color myColor  = Color.white; 
    public bool AmInPath = false;

    public float initWidth = 0.01f;
    Billboard billboard;
    // Start is called before the first frame update
    void Awake()
    {
        CovidNetworkManager.ResetAllPathEvent += ResetPathColor;
        FilterTab.OnLevelChangedEvent += ShowOrHideBasedOnLevel;
    }


    public void Init(int level, string id, GameObject edge)
    {
        myLevel = level;
        ID = id;
        Edge = edge;

        billboard = gameObject.GetComponent<Billboard>();
        billboard.enabled = false;

        if (CovidNetworkManager.Node_MetaInfo_Dictionary.ContainsKey(ID))
        {
            metaInfo = CovidNetworkManager.Node_MetaInfo_Dictionary[ID];
        }
        else
        {
            metaInfo = new NodeMetaInfo();

        }
        //Assigning Countryname as label
        if(metaInfo != null)
        {
            Date = metaInfo.Collection_Data;

            if (Date != "" && Date != null)
                MyCollectionDate = DateTime.Parse(Date); 
            
            if (metaInfo.Country == null || metaInfo.Country == "")
            {
                Country.text = "Unknown";
            }
            else
            {
                Country.text = metaInfo.Country;
                //Assigning Colors based on Countries
                Debug.Log("CountryInfo : " + metaInfo.Country);
                if (CovidNetworkManager.CountryAndCounts.ContainsKey(metaInfo.Country))
                {
                    NetworkCountries countryInfo = CovidNetworkManager.CountryAndCounts[metaInfo.Country];
                    Material mat = countryInfo.mat;
                    Renderer rand = NodeRep.GetComponent<Renderer>();
                    mat.mainTexture = rand.material.mainTexture;
                    rand.material = mat;

                    //myColor = countryInfo.Color;
                    if (Edge != null)
                    {
                        LineRenderer lr = Edge.GetComponent<LineRenderer>();
                        lr.SetColors(countryInfo.Color, countryInfo.Color);
                        initWidth = lr.startWidth;
                        myColor = lr.startColor;
                        Edge.GetComponent<GraphLineTextureAnimation>().enabled = false;
                    }
                        
                }
            }            
        }
        Country.gameObject.SetActive(false);

        
        
    }

    void OnApplicationQuit()
    {
        FilterTab.OnLevelChangedEvent -= ShowOrHideBasedOnLevel;
    }

    void Start()
    {
        
    }

    public void ShowOrHideBasedOnLevel(int level)
    {
        if(myLevel <= level)
        {
            AmIvisible = true;
        }
        else
        {
            AmIvisible = false;
        }

        NodeRep.SetActive(AmIvisible);

        if(Edge != null)
            Edge.SetActive(AmIvisible);

       
    }

    
    void OnTriggerEnter(Collider coll)
    {
        Country.gameObject.SetActive(true);
        billboard.enabled = true;
        if (Edge != null)
        {
            Edge.GetComponent<GraphLineTextureAnimation>().enabled = true;
        }
    }

    void OnTriggerExit(Collider coll)
    {
        Country.gameObject.SetActive(false);
        billboard.enabled = false;
        if (Edge != null)
        {
            Edge.GetComponent<GraphLineTextureAnimation>().enabled = false;
        }
    }

    void OnMouseDown()
    {
        //Debug.Log(gameObject.name);
        //if(metaInfo != null)
        //  Debug.Log("clicked on " + metaInfo.Country);

        //NodeInstance[] nodeInstance = gameObject.GetComponentsInChildren<NodeInstance>();
        //for(int i=0; i<nodeInstance.Length; i++)
        //{
        //    nodeInstance[i].ShowOrHideBasedOnLevel(myLevel + 1);
        //}
       TrackPath(transform);
    }

    void TrackPath (Transform currentNode)
    {
        if (currentNode == null)
        {
            return;
        }

        if(currentNode.GetComponent<NodeInstance>())
        {
            NodeInstance nodeInstanceComp = currentNode.GetComponent<NodeInstance>();
            nodeInstanceComp.AmInPath = true;

            if(nodeInstanceComp.Edge)
            {
                LineRenderer lr = nodeInstanceComp.Edge.GetComponent<LineRenderer>();
                lr.SetColors(Color.red, Color.red);
                lr.startWidth = (initWidth * 3f);
                lr.endWidth = (initWidth * 3f);
                TrackPath(currentNode.parent);
            }
        }  
    }

    void ResetPathColor()
    {
        if(AmInPath)
        {
            if(Edge != null)
            {
                LineRenderer lr = Edge.GetComponent<LineRenderer>();
                lr.SetColors(myColor, myColor);
                lr.startWidth = initWidth;
                lr.endWidth = initWidth;
            }
        }
    }
}
