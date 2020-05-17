using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realtime.LITJson;
using UnityEngine.SceneManagement;

public class CovidNetworkManager : MonoBehaviour
{
    public static int MAX_LEVEL = 15;
    public static int CurrentLevel;
    public TextAsset data_covidJson;
    public TextAsset data_nodePos;
    public TextAsset text_metaInfo;
    public GameObject prefab_Node;
    public GameObject prefab_Edge;
    public CovidNode root_CovidNode;
    public GameObject Go_RootNode;

  
    //public MetaInfoContainer metaInfoContainer;
    public NodeMetaInfo[] obj_metaInfo;

    public Dictionary<string, Vector3> node_Position_Dictionary;
    public static Dictionary<string, NodeMetaInfo> Node_MetaInfo_Dictionary = new Dictionary<string, NodeMetaInfo>();
    public delegate void CheckScalingNow(float dtScale);
    public static CheckScalingNow ScalingNowEvent;
    public static Dictionary<string, NetworkCountries> CountryAndCounts = new Dictionary<string, NetworkCountries>();
    // Start is called before the first frame update
    void Start()
    {
        root_CovidNode = JsonMapper.ToObject<CovidNode>(data_covidJson.text);
        node_Position_Dictionary = JsonMapper.ToObject<Dictionary<string, Vector3>>(data_nodePos.text);
        Debug.Log("Total Nodes : " + node_Position_Dictionary.Count);

        obj_metaInfo = JsonMapper.ToObject<NodeMetaInfo[]>(text_metaInfo.text);
        for(int i=0; i<obj_metaInfo.Length; i++)
        {
            if(!Node_MetaInfo_Dictionary.ContainsKey(obj_metaInfo[i].Strain))
            {
                Node_MetaInfo_Dictionary.Add(obj_metaInfo[i].Strain, obj_metaInfo[i]);
            }

            if(CountryAndCounts.ContainsKey(obj_metaInfo[i].Country))
            {
                CountryAndCounts[obj_metaInfo[i].Country].NumberOfAffected += 1;
            }
            else
            {
                Color32 color = new Color32(
                (byte) Random.Range(0, 255f),
                (byte) Random.Range(0, 255f),
                (byte) Random.Range(0, 255f),
                (byte) 255f
                );

                NetworkCountries countryInfo = new NetworkCountries(obj_metaInfo[i].Country, color, 1);
                CountryAndCounts.Add(obj_metaInfo[i].Country, countryInfo);
            }
        }

        Debug.Log("Total countries : "+ CountryAndCounts.Count);

        foreach(var entry in CountryAndCounts)
        {
            Debug.Log(entry.Key +" "+ entry.Value.NumberOfAffected);
        }

        //Generating RootNode 
        Vector3 pos = Vector3.zero;
        if(node_Position_Dictionary.ContainsKey(root_CovidNode.name))
        {
            pos = node_Position_Dictionary[root_CovidNode.name];
            Go_RootNode = Instantiate(prefab_Node);//, Vector3.zero, Quaternion.identity) as GameObject;
            Go_RootNode.transform.SetParent(transform);
            Go_RootNode.transform.localPosition = pos; // Vector3.zero;
            Go_RootNode.GetComponent<NodeInstance>().Init(0, root_CovidNode.name, null);
            StartCoroutine(LoadNetwork());
        }
        else
        {
            Debug.LogError("RootNode name not found in dictionary !");
        } 
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touches.Length > 1 || Input.GetMouseButton(1))
            ScalingNowEvent(transform.parent.localScale.x);
    }

    IEnumerator LoadNetwork()
    {
        yield return new WaitForSeconds(1);
        GenerateDescedants(1, Go_RootNode.transform, root_CovidNode);
    }

    void GenerateDescedants(int level, Transform startNode, CovidNode node)
    {
        startNode.name = node.name;
        if (level > MAX_LEVEL)
        {
            return;
        }

        if (node.children == null)
        {
            return;
        }

        for (int i = 0; i < node.children.Count; i++)
        {
            Vector3 pos = Vector3.zero;
            if (node_Position_Dictionary.ContainsKey(node.children[i].name))
            {
                float factor = 100;
                
                pos = node_Position_Dictionary[node.children[i].name]/factor;
                GameObject descedantNode = Instantiate(prefab_Node);// pos, Quaternion.identity) as GameObject;
                descedantNode.transform.SetParent(startNode.transform);
                descedantNode.transform.localPosition = pos;
                //descedantNode.transform.localScale /= 10;
                GameObject edge = Instantiate(prefab_Edge);//, startNode.position, Quaternion.identity);
                //
                edge.transform.SetParent(startNode.transform);
                edge.transform.localPosition = Vector3.zero;
                edge.GetComponent<EdgeComponent>().Init(startNode, descedantNode.transform);
                LineRenderer lr = edge.GetComponent<LineRenderer>();
                lr.SetPosition(0, Vector3.zero);
                lr.SetPosition(1, descedantNode.transform.localPosition);// / factor);
                lr.startWidth = lr.endWidth = 0.008f;
                descedantNode.GetComponent<NodeInstance>().Init(level, node.children[i].name, edge);

                //DrawEdge
                //graph.NewEdge(startNode, descedantNode);          

                if (node.children[i] != null)
                {
                    GenerateDescedants(level + 1, descedantNode.transform, node.children[i]);
                }

            }
            else
            {
                Debug.LogError("Node name : "+ node.children[i].name + ",  not found in dictionary !");
            }
        }
    }

    public delegate void ResetAllPath();
    public static ResetAllPath ResetAllPathEvent;

    public void ResetPath()
    {
        ResetAllPathEvent();
    }

    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("Network_ObjectManipulation", LoadSceneMode.Single);
    }

    public void LoadGlobeScene()
    {
        SceneManager.LoadScene("Globe_ObjectManipulation 1", LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
