using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgeGraph : MonoBehaviour
{
    public static AgeGraph Instance;
    public GameObject[] AgeBars;
    // Start is called before the first frame update
   void Awake()
    {
        Instance = this;
        ResetGraph();
    }

    public void Plot(Dictionary<string, int> dict)
    {
        ResetGraph();
        int index = 0;
        foreach(var entry in dict)
        {
            if(entry.Key == "0_to_9")
            {
                index = 0;
            }
            else if(entry.Key == "10_to_19")
            {
                index = 1;
            }
            else if (entry.Key == "20_to_29")
            {
                index = 2;
            }
            else if (entry.Key == "30_to_39")
            {
                index = 3;
            }
            else if (entry.Key == "40_to_49")
            {
                index = 4;
            }
            else if (entry.Key == "50_to_59")
            {
                index = 5;
            }
            else if (entry.Key == "60_to_69")
            {
                index = 6;
            }
            else if (entry.Key == "70_to_79")
            {
                index = 7;
            }
            else
            {
                index = 8;
            }

            RectTransform rect = AgeBars[index].GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, dict[entry.Key]);
        }

    }  

    void ResetGraph()
    {
        for(int i=0; i<AgeBars.Length; i++)
        {
            RectTransform rect = AgeBars[i].GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 0);
        }
    }
}
