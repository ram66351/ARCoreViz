using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CovidData3DInstance : MonoBehaviour
{
   // [SerializeField]
    public Covid19Data CovidData_Confirmed;
    public Covid19Data CovidData_Death;
    public Covid19Data CovidData_Recovered;
    public SpriteRenderer sRenderer;
    // Start is called before the first frame update
    public Vector3 posOnSphere;
    public Transform Earth;
    public Color32 Color;
    public Color32 SelectionColor;
    public Color32 UnSelectionColor;

    public static GameObject SelectedLocation;
    public float newVal;
    //public GameObject SelectionHighlight;
    public float EnlargeScale = 3;

    public GameObject Label;
    public TextMesh infoText;
    public TextMesh locText;

    public float thresholdDistance = 0.25f;

    public delegate void ClickedOnData(Transform t);
    public static ClickedOnData ClickedOnDataEvent;

    
    public void Init(Covid19Data covidData, Transform earth, Color32 color)
    {
        
       

        //SelectionHighlight.SetActive(false);
        CovidData_Confirmed = covidData;
        Earth = earth;
        posOnSphere = AppUtils.LatLonToPositionOnSphere(CovidData_Confirmed.Lat, CovidData_Confirmed.Long, Earth.localScale.x * 2.1f);
        transform.localPosition = posOnSphere;
        Color = color;
        Vector3 dir = new Vector3(0, 1, 0);
        Vector3 crossDir = Vector3.Cross(dir, posOnSphere);
        float angle = Vector3.Angle(dir, posOnSphere);
        transform.Rotate(crossDir, angle, Space.Self);
        float limit = 500;
        float val = CovidData_Confirmed.DayByDayData[CovidData_Confirmed.DayByDayData.Length - 1];
        if (val < limit)
        {
            newVal = AppUtils.Remap(val, 0, limit, 0, 255);
        }
        else
        {
            newVal = 255;
        }
         
        UnSelectionColor = new Color32((byte)newVal, 255, 0, 255);
        sRenderer.color = UnSelectionColor;

        if (CovidData_Confirmed.ProvinceORState == "")
            locText.text = "Not mentioned";
        else
            locText.text = CovidData_Confirmed.ProvinceORState;

        locText.text += ", "+ CovidData_Confirmed.CountryORRegion;

        Label.SetActive(false);

        ClickedOnDataEvent += AmINear;
        DataBars.ClickedOnBarEvent += ShowOrHideMe;
        sRenderer.enabled = false;

        if (covidData.CountryORRegion == "US")
        {
            if (UnityEngine.Random.Range(1, 100) % 2 == 0)
            {
                gameObject.SetActive(false);
            }
            //transform.localScale /= 10;
        }
    }

    void OnApplicationQuit()
    {
        ClickedOnDataEvent -= AmINear;
        DataBars.ClickedOnBarEvent -= ShowOrHideMe;
    }

    void ShowOrHideMe(string Country)
    {
        BoxCollider bcoll = gameObject.GetComponent<BoxCollider>();
        if(Country == CovidData_Confirmed.CountryORRegion)
        {
            sRenderer.enabled = true;
            bcoll.enabled = true;
            //SelectionHighlight.SetActive(true);
        }
        else
        {
            sRenderer.enabled = false;
            bcoll.enabled = false;
            //SelectionHighlight.SetActive(false);
        }
    }

    private void AmINear(Transform t)
    {
        float dist = Vector3.Distance(t.position, transform.position);
        if(dist <thresholdDistance)
        {
            Label.SetActive(true);
        }
        else
        {
            Label.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if(SelectedLocation != gameObject)
        {
            CovidData3DInstance instance3D;
            if (SelectedLocation != null)
            {
                //SelectedLocation.transform.localScale /= 2;
                StartCoroutine(ScaleAnimation(0.1f, SelectedLocation.transform, SelectedLocation.transform.localScale, SelectedLocation.transform.localScale / EnlargeScale));
               //SelectedLocation.GetComponentInChildren<SpriteRenderer>().color = UnSelectionColor;
                instance3D = SelectedLocation.GetComponent<CovidData3DInstance>();
                //instance3D.SelectionHighlight.SetActive(false);
                instance3D.Label.SetActive(false);
                locText.color = new Color(255, 255, 255,255);
                infoText.color = new Color(255, 255, 255, 255);
            }
            SelectedLocation = gameObject;
            //SelectedLocation.transform.localScale *= 2;
            StartCoroutine(ScaleAnimation(0.4f, SelectedLocation.transform, SelectedLocation.transform.localScale, SelectedLocation.transform.localScale * EnlargeScale));
            //SelectedLocation.GetComponentInChildren<SpriteRenderer>().color = SelectionColor;
            instance3D = SelectedLocation.GetComponent<CovidData3DInstance>();
            //instance3D.SelectionHighlight.SetActive(true);
            instance3D.Label.SetActive(true);
            locText.color = new Color(255, 0, 0, 255);
            infoText.color = new Color(255, 0, 0, 255);
            //ClickedOnDataEvent(transform);

        }

        Debug.Log("Country : " + CovidData_Confirmed.CountryORRegion +", Max "+ CovidData_Confirmed.DayByDayData[CovidData_Confirmed.DayByDayData.Length -1]);

        GraphMaster.Instance.PlotConfirmed(CovidData_Confirmed);

        UpdateInfo("Confirmed : "+ CovidData_Confirmed.DayByDayData[CovidData_Confirmed.DayByDayData.Length - 1]);

        if (CovidData_Death != null)
        {
            GraphMaster.Instance.PlotDeath(CovidData_Death);
        }

        if (CovidData_Recovered != null)
        {
            GraphMaster.Instance.PlotRecovered(CovidData_Recovered);
        }

        FocusOnData();
        GraphMaster.Instance.EnableGraph(true);
    }

    IEnumerator ScaleAnimation(float dt, Transform obj, Vector3 from, Vector3 to)
    {
        //obj.localScale = from;
        float t = 0;
        while (t < 3)
        {
            yield return null;
            t += Time.deltaTime / dt;
            obj.localScale = Vector3.Lerp(from, to, t);
        }
    }

    public void FocusOnData()
    {
        Transform earth = Covid19Viz.Instance.Earth;
        Quaternion oldRotation = earth.rotation;
        earth.rotation = Quaternion.identity;
        Vector3 fromDirection = (transform.position - earth.position).normalized;
        Vector3 toDirection = (Camera.main.transform.position - earth.position).normalized;
        Quaternion fromRotation = Quaternion.Inverse(Quaternion.LookRotation(fromDirection, Vector3.up));
        Quaternion targetRotation = Quaternion.LookRotation(toDirection, Vector3.up) * fromRotation;
        StartCoroutine(AnimateEarthRotation(0.8f, earth, oldRotation, targetRotation));
    }

    IEnumerator AnimateEarthRotation(float dt, Transform earth, Quaternion from, Quaternion to)
    {
        earth.rotation = from;
        float t = 0;
        while (t < 3)
        {
            yield return null;
            t += Time.deltaTime / dt;
            earth.rotation = Quaternion.Lerp(from, to, t);
        }
        
    }

    public void UpdateInfo(string info)
    {
        infoText.text += info;
    }
}
