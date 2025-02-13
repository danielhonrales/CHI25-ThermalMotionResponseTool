using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        trialResponses = new();
        trialResponses.number = trialNumber;
        yield return new WaitForSeconds(time / 1000f);
        
        LoadResponseUI();
    }

    public void LoadResponseUI() {
        idleScreen.SetActive(false);

        
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

    public void RecordTrialResponse(string response) {
        trialResponses.responses.Add(response);
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
        public List<string> responses = new();

        public string ToListString() {
            return string.Format("{0},{1}",
                number,
                string.Join(",", responses)
            );
        }
    }
}
