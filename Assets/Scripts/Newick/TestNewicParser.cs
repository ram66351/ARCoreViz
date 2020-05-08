using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNewicParser : MonoBehaviour
{
    public TextAsset treeData;
    public TextAsset covidJson;
    
    public CovidNode covidNode;
    // Start is called before the first frame update
    void Start()
    {

        covidNode = JsonUtility.FromJson<CovidNode>(covidJson.text);
        
        //var tree = new Parser("((B:0.2,(C:0.3,D:0.4)E:0.5)A:0.1)F;").ParseTree();
        //var tree = new Parser(treeData.text).ParseTree();
        //var tree = new Parser("((A:1, B:2):3, C:4)").ParseTree();
        //var leaf = tree as Leaf;
        //Debug.Log("Name : " + tree.Length + ", tree.SubBranches.Count:  " + tree.SubBranches.Count);

        //PrintNode(tree.SubBranches);        
    }

    IEnumerator WaitForLoad()
    {
        yield return new WaitForSeconds(5);
        Debug.Log(covidNode.children.Length);
    }

    public void PrintNode(List<Branch> SubBranches)
    {
        for (int i = 0; i < SubBranches.Count; i++)
        {
            var leaf = SubBranches[i] as Leaf;
            if (leaf != null)
            {
                Debug.Log("leaf name : " + leaf.Name +", Branch : "+ SubBranches[i].Length +", subtree Count : "+ SubBranches[i].SubBranches.Count);
            }

            //if (SubBranches.Count > 0)
                PrintNode(SubBranches[i].SubBranches);
           // Call same function to print the subtrees    
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
