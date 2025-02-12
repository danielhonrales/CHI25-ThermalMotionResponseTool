using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicQuestion : MonoBehaviour
{

    public TrialController trialController;
    public Image image;

    public Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        Debug.Log("Swapping shape image");
        if (trialController.pattern.Trim() == "S-curve") {
            image.sprite = sprites[0];
        } else if (trialController.pattern.Trim() == "C-curve") {
            image.sprite = sprites[1];
        } else {
            image.sprite = sprites[2];
        }
    }
}
