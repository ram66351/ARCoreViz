using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class GraphMaster : MonoBehaviour
{
    public GraphManager ConfirmedGraph;
    public GraphManager DeathGraph;
    public GraphManager RecoveredGraph;
    public Transform smallGraphContainer;

    public GraphManager overAllCountryConfirmed;
    public Transform BigGraphContainer;

    public static GraphMaster Instance;
    public Canvas canvas;
    public GameObject Globe;
    public Text State;
    public Text Country;

    public Text text_Country;
    public Text text_TotalConfirmed;
    public Text text_TotalDeath;
    public Text text_TotalRecovered;
    public Text text_startDate;
    public Text text_endDate;
    public Text text_symptoms;

    public GameObject SymptomsPanel;

    public DateTime startDate;
    public DateTime endDate;

    private Vector3 smallGraph_initPos;
    private Vector3 smallGraph_initScale;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        Covid19Viz[] covidViz = Globe.GetComponents<Covid19Viz>();;
        StartCoroutine(EnableScript(1, covidViz[1]));
        StartCoroutine(EnableScript(2, covidViz[2]));
        canvas.worldCamera = Camera.main;
        //CovidData3DInstance.ClickedOnDataEvent += EnableGraphs;
        ConfirmedGraph.gameObject.SetActive(false);
        DeathGraph.gameObject.SetActive(false);
        RecoveredGraph.gameObject.SetActive(false);
        ShowInfoPanel(false);
        BigGraphContainer.gameObject.SetActive(false);
        smallGraph_initPos = smallGraphContainer.position;
        smallGraph_initScale = smallGraphContainer.localScale;
    }

    void OnApplication()
    {
        //CovidData3DInstance.ClickedOnDataEvent -= EnableGraphs;
    }

    void EnableGraphs(Transform t)
    {
       
    }
   
    public void EnableGraph(bool enable)
    {
        ConfirmedGraph.gameObject.SetActive(enable);
        DeathGraph.gameObject.SetActive(enable);
        RecoveredGraph.gameObject.SetActive(enable);
    }


    public void PlotConfirmed(Covid19Data data)
    {
        Debug.Log(".CountryORRegion");
        ConfirmedGraph.gameObject.SetActive(true);
        ConfirmedGraph.PlotGraph(data);

        if (data.ProvinceORState == "")
            State.text = "Not mentioned";
        else
            State.text = data.ProvinceORState;

        Country.text = data.CountryORRegion;
       

    }
    
    public void PlotDeath(Covid19Data data)
    {
        Debug.Log(".PlotDeath");
        DeathGraph.gameObject.SetActive(true);
        DeathGraph.PlotGraph(data);
    }

    public void PlotRecovered(Covid19Data data)
    {
        Debug.Log(".PlotRecovered");
        RecoveredGraph.gameObject.SetActive(true);
        RecoveredGraph.PlotGraph(data);
    }

    IEnumerator EnableScript(float delay, Covid19Viz script)
    {
        yield return new WaitForSeconds(delay);
        script.enabled = true;
    }

    public void SetupDates(DateTime st, DateTime end)
    {
        startDate = st;
        endDate = end;
        text_startDate.text = startDate.ToString();
        text_endDate.text = endDate.ToString();
    }


    public void ClearAllCountryConfirmed()
    {
        LineRenderer[] lr = overAllCountryConfirmed.gameObject.GetComponentsInChildren<LineRenderer>();
        for(int i=0; i<lr.Length; i++)
        {
            Destroy(lr[i].gameObject);
        }
    }

    public int[] confArray;
    public int[] deathArray;
    public int[] recoverArray;

    public void PlotAllCountryConfirmed(int[] data, string name, Material mat,System.Action<GameObject> callback, Covid19Data.Status status)
    {
        if(status == Covid19Data.Status.recovered)
        {
            recoverArray = data;
        }
        else if(status == Covid19Data.Status.confirmed)
        {
            confArray = data;
        }
        else if(status == Covid19Data.Status.dead)
        {
            deathArray = data;
        }

        GameObject lineObject = new GameObject();
        lineObject.name = name + "_Graph";
        //lineObject.transform.SetParent(overAllCountryConfirmed.gameObject.transform);
        lineObject.transform.SetParent(BigGraphContainer);
        lineObject.transform.localPosition = new Vector3(0, 0, 0);
        lineObject.transform.localScale = new Vector3(1, 1, 1);

        LineRenderer lr = lineObject.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
            lr.material = mat;
        overAllCountryConfirmed.PointsParent = lineObject.transform;
        overAllCountryConfirmed.PlotGraph(data, lr);
        callback(lineObject);
    }

    public void PlotCountryInfo(string country, int conf, int death, int recovered, string symptoms)
    {
        BigGraphContainer.gameObject.SetActive(true);
        text_Country.text = country;
        text_TotalConfirmed.text = conf+"";
        text_TotalDeath.text = death+"";
        text_TotalRecovered.text = recovered+"";
        text_symptoms.text = symptoms;
    }

    public void ShowInfoPanel(bool enable)
    {
        SymptomsPanel.SetActive(enable);
    }


    public void PlaceGraphAtLocation(Vector3 pos)
    {
        smallGraphContainer.position = pos;
        smallGraphContainer.localScale /= 10;
    }

    public void PlaceGraphOutSide()
    {
        smallGraphContainer.position = smallGraph_initPos;
        smallGraphContainer.localScale = smallGraph_initScale;
    }
  
}
