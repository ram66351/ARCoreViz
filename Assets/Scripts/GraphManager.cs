using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphManager : MonoBehaviour
{
    [SerializeField]
    private Sprite circleSprite;
    public Transform graphContainer;
    public Transform PointsParent;
    public static GraphManager Instance;
    public LineRenderer lr;
    public Covid19Data CovidData;
    public float speed = 0.1f;
    public RectTransform refLine;
    public bool isPlay = false;

    float dx = 0;
    float dy = 0;
    RectTransform rectTransform;

    public Text Day;
    public Text Date;
    public Text Confirmed;
    public Slider SpeedSlider;
    public Text Latest;

    public Sprite s_Play;
    public Sprite s_Stop;
    public Image ButtonIcon;

    public delegate void BroadcastPlayAnimation(bool play);
    public static BroadcastPlayAnimation PlayAnimationEvent;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        if(GetComponent<LineRenderer>())
            lr = GetComponent<LineRenderer>();
        //speed = SpeedSlider.minValue;
        GraphManager.PlayAnimationEvent += PlayAnimation;
        
    }

    void OnApplication()
    {
        GraphManager.PlayAnimationEvent -= PlayAnimation;
    }

    public void CreateGraphPt(Vector2 anchoredPosition, int index, LineRenderer lr)
    {
        GameObject graphPt = new GameObject("dot", typeof(Image));
        graphPt.transform.SetParent(PointsParent, false);
        graphPt.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = graphPt.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(5,5);
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.anchorMax = new Vector2(0, 0);
        lr.SetPosition(index, graphPt.transform.localPosition);
    }

    public void PlotGraph(Covid19Data covidData)
    {
        CovidData = covidData;
        PlotGraph(CovidData.DayByDayData, lr);
        //int[] data = covidData.DayByDayData;
        //Latest.text = data[data.Length -1] + "";
        //ClearGraph();
        //rectTransform = graphContainer.GetComponent<RectTransform>();
        //dx = rectTransform.sizeDelta.x / data.Length;
        
        //if (data[data.Length - 1] != 0)
        //    dy = rectTransform.sizeDelta.y / data[data.Length - 1];
        //lr.positionCount = data.Length;
        //lr.startWidth = 0.01f;
        //lr.endWidth = 0.01f;
        //for (int i = 0; i<data.Length; i++)
        //{
        //    float x = dx * (i+1);
        //    float y = dy * data[i];
        //    CreateGraphPt(new Vector2(x, y), i);
        //}
    }
    int[] myData;

    public void PlotGraph(int[] data, LineRenderer lr)
    {
        myData = data;
        Latest.text = myData[myData.Length - 1] + "";
        ClearGraph();
        rectTransform = graphContainer.GetComponent<RectTransform>();
        dx = rectTransform.sizeDelta.x / myData.Length;

        if (myData[myData.Length - 1] != 0)
            dy = rectTransform.sizeDelta.y / myData[myData.Length - 1];
        lr.positionCount = myData.Length;
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        for (int i = 0; i < myData.Length; i++)
        {
            float x = dx * (i + 1);
            float y = dy * myData[i];
            CreateGraphPt(new Vector2(x, y), i, lr);
        }
    }

    void ClearGraph()
    {
        int children = PointsParent.transform.childCount;
        for(int i = children -1; i > 0; i--)
        {
            Destroy(PointsParent.transform.GetChild(i).gameObject);
        }
    }

    public void PlayButtonPressed()
    {
        isPlay = !isPlay;
        PlayAnimationEvent(isPlay);

        if (isPlay)
            ButtonIcon.sprite = s_Stop;
        else
            ButtonIcon.sprite = s_Play;
    }

    private void PlayAnimation(bool play)
    {
        isPlay = play; 
        if(isPlay && gameObject.active)
            StartCoroutine(Play());
    }

    int index = 0;

    IEnumerator Play()
    {
        
        yield return new WaitForSeconds(speed);
        float _dx = refLine.anchoredPosition.x + dx;

        if (_dx > rectTransform.sizeDelta.x -1)// || index > CovidData.DayByDayData.Length)
        {
            _dx = 0;
            index = 0;
        }
        refLine.anchoredPosition = new Vector3(_dx, refLine.anchoredPosition.y);
        Day.text = index + 1 +"";

        if (CovidData != null)
        {
            Date.text = CovidData.StartDate.AddDays(index) + "";
            Confirmed.text = myData[index] + "";
        }
        else
        {
            Date.text = GraphMaster.Instance.startDate.AddDays(index).ToString();
            if (GraphMaster.Instance.confArray != null)
                Confirmed.text = "Conf : " + GraphMaster.Instance.confArray[index] + "\n";

            if (GraphMaster.Instance.deathArray != null)
                Confirmed.text += "Death : " + GraphMaster.Instance.deathArray[index] + "\n";

           if (GraphMaster.Instance.recoverArray != null)
                Confirmed.text += "Recover : " + GraphMaster.Instance.recoverArray[index] + "\n";
        }


        if (isPlay)
        {
            ++index;
            StartCoroutine(Play());
        }
    }

    public void OnValueChanged(float val)
    {
        speed = val;
    }
}
