using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCountries : MonoBehaviour
{
    public TextAsset csvData;
    public bool isDataReceived = false;
    public ShowExtraInfo showInfo;
    public Dictionary<string, Vector2> CountryDict = new Dictionary<string, Vector2>();
    private string[,] Grid;
    public static LoadCountries Instance;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
       
    }

    void Start()
    {
        showInfo.enabled = false;
        CSVLoader.CSVLoaderThread(csvData.text, ReceiveData);

    }

    void ReceiveData(string[,] data)
    {
        Grid = data;
        Debug.Log("Received Data ! " + Grid.GetUpperBound(0) + ", " + Grid.GetUpperBound(1));
        isDataReceived = true;
    }

    void LateUpdate()
    {
        if (isDataReceived)
        {
            PopulateCountries();
        }
    }

    void PopulateCountries()
    {
        int row = Grid.GetUpperBound(0);
        int col = Grid.GetUpperBound(1);

        for (int i = 1; i < col; i++)
        {
            float lat, lon;
            if (!float.TryParse(Grid[1, i], out lat))
            {
                Debug.LogError("Error in Lat at Country : "+ Grid[0,i] +" lat :"+ lat);
            }

            if (!float.TryParse(Grid[2, i], out lon))
            {
                Debug.LogError("Error in lon at Country : " + Grid[0, i] + " lon :" + lon);
            }

            CountryDict.Add(Grid[0,i], new Vector2(lat, lon));
        }

        isDataReceived = false;
        showInfo.enabled = true;
        Debug.Log("Total Countries : "+ CountryDict.Count);
    }
}
