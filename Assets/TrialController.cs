using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialController : MonoBehaviour
{
    public UserResponseTool_Client tcpClient;

    public int trialNumber;
    public string pattern;
    public List<Trial> allResponses;
    public Trial trialResponses;
    public List<GameObject> questionUIs;
    public GameObject idleScreen;
    public int currentQuestion;

    public bool testing = false;

    // Start is called before the first frame update
    void Start()
    {
        StartNewExperiment();
    }

    // Update is called once per frame
    void Update()
    {
        if (testing) {
            Test();
        } 
    }

    public void StartNewExperiment() {
        trialNumber = 0;
        allResponses = new();
        trialResponses = new();
        currentQuestion = 0;
    }

    public IEnumerator TrialStart(float time, int incomingTrialNumber, string incomingPattern) {
        Debug.Log(string.Format("Trial Start, waiting for {0}", time));
        trialNumber = incomingTrialNumber;
        pattern = incomingPattern;
        yield return new WaitForSeconds(time / 1000f);
        
        LoadResponseUI();
    }

    public void LoadResponseUI() {
        idleScreen.SetActive(false);

        trialResponses = new();
        trialResponses.number = trialNumber;
        foreach (GameObject questionUI in questionUIs) {
            questionUI.SetActive(false);
        }

        currentQuestion = 0;
        questionUIs[0].SetActive(true);
    }

    public void NextQuestion() {
        questionUIs[currentQuestion].SetActive(false);

        if (currentQuestion < questionUIs.Count - 1) {
            currentQuestion++;
            questionUIs[currentQuestion].SetActive(true);
        } else {
            EndTrial();
        }
    }

    public void SkipRestOfTrial() {
        questionUIs[currentQuestion].SetActive(false);
        currentQuestion = questionUIs.Count - 1;
        questionUIs[currentQuestion].SetActive(true);
    }

    public void EndTrial() {
        allResponses.Add(trialResponses);
        trialNumber++;

        idleScreen.SetActive(true);

        StartCoroutine(NextTrial());
    }

    public void EndExperiment() {
        List<string> serialized = new();
        foreach (Trial trial in allResponses) {
            serialized.Add(trial.ToListString());
        }
        string message = string.Join("\n", serialized);

        Debug.Log(message);
        tcpClient.SendSignal(message);
    }

    public void RecordTrialResponseFelt(string response) {
        trialResponses.responseFelt = response;
    }

    public void RecordTrialResponseTemp(string response) {
        trialResponses.responseTemp = response;
    }

    public void RecordTrialResponseSmooth(string response) {
        trialResponses.responseSmooth = response;
    }

    public void Test() {
        StartCoroutine(TrialStart(3000,1,"circle"));
        testing = false;
    }

    private IEnumerator NextTrial() {
        yield return new WaitForSeconds(2.5f);
        string message = "nexttrial";
        tcpClient.SendSignal(message);
    }

    public class Trial {
        public int number;
        public string responseFelt;
        public string responseTemp;
        public string responseSmooth;

        public string ToListString() {
            return string.Format("{0},{1},{2},{3}",
                number,
                responseFelt,
                responseTemp,
                responseSmooth
            );
        }
    }
}
