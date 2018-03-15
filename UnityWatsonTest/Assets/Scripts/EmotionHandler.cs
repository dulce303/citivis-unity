using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionHandler : MonoBehaviour
{


    public enum EmotionType { Joy, Sadness, Fear, Disgust, Anger, Analytical, Confident, Tenatative };
    public EmotionType Emotion;

    // one of these for each emotion
    public GameObject joyObject;
    public GameObject sadnessObject;
    public GameObject fearObject;
    public GameObject disgustObject;
    public GameObject angerObject;
    public GameObject analyticalObject;
    public GameObject confidentObject;
    public GameObject tentativeObject;

    public float emotion_threshold;

    void Start()
    {
        // threshold above which a tone fires the handler
        emotion_threshold = 0.75f;  // for loose demo - above 75% seems to work well - may vary by signal
    }

    public void HandleEmotion(EmotionType emotionType, double score)
    {
        switch (emotionType)
        {
            case EmotionType.Joy:
                // do something to Emotion Object, can use score to alter value
                if (angerObject != null)
                {
                    if (score > emotion_threshold)
                        joyObject.SetActive(true);
                    else
                        joyObject.SetActive(false);
                }
                break;
            case EmotionType.Sadness:
                // do something to Emotion Object, can use score to alter value
                if (angerObject != null)
                {
                    if (score > emotion_threshold)
                        sadnessObject.SetActive(true);
                    else
                        sadnessObject.SetActive(false);
                }
                break;
            case EmotionType.Fear:
                // do something to Emotion Object, can use score to alter value
                if (fearObject != null)
                {
                    if (score > emotion_threshold)
                        fearObject.SetActive(true);
                    else
                        fearObject.SetActive(false);
                }
                break;
            case EmotionType.Disgust:
                // do something to Emotion Object, can use score to alter value
                if (disgustObject != null)
                {
                    if (score > emotion_threshold)
                        disgustObject.SetActive(true);
                    else
                        disgustObject.SetActive(false);
                }
                break;
            case EmotionType.Anger:
                // do something to Emotion Object, can use score to alter value
                if (angerObject != null)
                {
                    if (score > emotion_threshold)
                        angerObject.SetActive(true);
                    else
                        angerObject.SetActive(false);
                }
                break;
            case EmotionType.Analytical:
                // do something to Emotion Object, can use score to alter value
                if (analyticalObject != null)
                {
                    if (score > emotion_threshold)
                        analyticalObject.SetActive(true);
                    else
                        analyticalObject.SetActive(false);
                }
                break;
            case EmotionType.Confident:
                // do something to Emotion Object, can use score to alter value
                if (confidentObject != null)
                {
                    if (score > emotion_threshold)
                        confidentObject.SetActive(true);
                    else
                        confidentObject.SetActive(false);
                }
                break;
            case EmotionType.Tenatative:
                // do something to Emotion Object, can use score to alter value
                if (tentativeObject != null)
                {
                    if (score > emotion_threshold)
                        tentativeObject.SetActive(true);
                    else
                        tentativeObject.SetActive(false);
                }
                break;
        }
    }

    void reactWithScale(GameObject gameObject){
        
    }
}
