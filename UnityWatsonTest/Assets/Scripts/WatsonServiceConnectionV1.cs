/**
 * 
 * ############# HELLO VIRTUAL WORLD
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
* THIS IS VERY VERY VERY VERY VERY VERY VERY VERY VERY VERY VERY VERY VERY VERY VERY VERY VERY VERY ROUGH CODE - WARNING :) 
*/

using UnityEngine;

using System.Collections;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;
using System.Collections.Generic;
using UnityEngine.UI;

// added this from the TONE ANALYZER . CS file
using IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3;
using IBM.Watson.DeveloperCloud.Connection;

// and for Regex
using System.Text.RegularExpressions;

// and for Ethan's body
//using UnityStandardAssets;
//using UnityStandardAssets.Characters.ThirdPerson;


public class WatsonServiceConnectionV1 : MonoBehaviour
{
    // keys to speech to text engine
    private string _username_STT = "0080f8de-6a51-4cb6-b7ca-74dfc91e46a8";     private string _password_STT = "hQqTQuoIgYBj";
    private string _url_STT = "https://stream.watsonplatform.net/speech-to-text/api";
    // keys to watson tone analysis engine
    private string _username_TONE = "7f968f2a-4935-440a-96b2-144471fe4c5b";
    private string _password_TONE = "g2tz4PY2xqNs";
    private string _url_TONE = "https://gateway.watsonplatform.net/tone-analyzer/api";


    // text fields that get the values from speech->text and toneAnalysis
    public Text ResultsField;
    public Text EmotionText;

    // value above which we do something with the linked objects
    public float emotion_threshold;

    // Links to objects that handle changes
    [SerializeField]
    public IHandleToneChange joyHandler;
    [SerializeField]
    public IHandleToneChange sadnessHandler;
    public IHandleToneChange fearHandler;
    public IHandleToneChange disgustHandler;
    public IHandleToneChange angerHandler;
    
    public IHandleToneChange analyticalHandler;
    public IHandleToneChange confidentHandler;
    public IHandleToneChange tentativeHandler;

    private int _recordingRoutine = 0;
    private string _microphoneID = null;
    private AudioClip _recording = null;
    private int _recordingBufferSize = 1;
    private int _recordingHZ = 22050;

    private SpeechToText _speechToText;

    private ToneAnalyzer _toneAnalyzer;
    private string _toneAnalyzerVersionDate = "2017-05-26";
    private string _stringToTestTone1 = "START AND TEST - But this totally sucks! I hate beans and liver!";
    private string _stringToTestTone2 = "SECOND TEST - Failed Test Sucks";
    private bool _analyzeToneTested = false;


    void Start()
    {
        // Watson logging, interface to Speech to Text and Tone Analyzer Servises
        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials_STT = new Credentials(_username_STT, _password_STT, _url_STT);
        _speechToText = new SpeechToText(credentials_STT);
        Active = true;

        StartRecording();

        // TONE ZONE
        Credentials credentials_TONE = new Credentials(_username_TONE, _password_TONE, _url_TONE);
        _toneAnalyzer = new ToneAnalyzer(credentials_TONE);
        _toneAnalyzer.VersionDate = _toneAnalyzerVersionDate;

        // threshold above which a tone fires the handler
        emotion_threshold = 0.75f;  // for loose demo - above 75% seems to work well - may vary by signal
    }

    public bool Active
    {
        get { return _speechToText.IsListening; }
        set
        {
            if (value && !_speechToText.IsListening)
            {
                _speechToText.DetectSilence = true;
                _speechToText.EnableWordConfidence = true;
                _speechToText.EnableTimestamps = true;
                _speechToText.SilenceThreshold = 0.01f;
                _speechToText.MaxAlternatives = 0;
                _speechToText.EnableInterimResults = true;
                _speechToText.OnError = OnError;
                _speechToText.InactivityTimeout = -1;
                _speechToText.ProfanityFilter = false;
                _speechToText.SmartFormatting = true;
                _speechToText.SpeakerLabels = false;
                _speechToText.WordAlternativesThreshold = null;
                _speechToText.StartListening(OnRecognize, OnRecognizeSpeaker);
            }
            else if (!value && _speechToText.IsListening)
            {
                _speechToText.StopListening();
            }
        }
    }

    private void StartRecording()
    {
        if (_recordingRoutine == 0)
        {
            UnityObjectUtil.StartDestroyQueue();
            _recordingRoutine = Runnable.Run(RecordingHandler());
        }
    }

    private void StopRecording()
    {
        if (_recordingRoutine != 0)
        {
            Microphone.End(_microphoneID);
            Runnable.Stop(_recordingRoutine);
            _recordingRoutine = 0;
        }
    }

    private void OnError(string error)
    {
        Active = false;

        Log.Debug("ExampleStreaming.OnError()", "Error! {0}", error);
    }

    private IEnumerator RecordingHandler()
    {
        Log.Debug("ExampleStreaming.RecordingHandler()", "devices: {0}", Microphone.devices);
        _recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);
        yield return null;      // let _recordingRoutine get set..

        if (_recording == null)
        {
            StopRecording();
            yield break;
        }

        bool bFirstBlock = true;
        int midPoint = _recording.samples / 2;
        float[] samples = null;

        while (_recordingRoutine != 0 && _recording != null)
        {
            int writePos = Microphone.GetPosition(_microphoneID);
            if (writePos > _recording.samples || !Microphone.IsRecording(_microphoneID))
            {
                Log.Error("ExampleStreaming.RecordingHandler()", "Microphone disconnected.");

                StopRecording();
                yield break;
            }

            if ((bFirstBlock && writePos >= midPoint)
                || (!bFirstBlock && writePos < midPoint))
            {
                // front block is recorded, make a RecordClip and pass it onto our callback.
                samples = new float[midPoint];
                _recording.GetData(samples, bFirstBlock ? 0 : midPoint);

                AudioData record = new AudioData();
                record.MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples));
                record.Clip = AudioClip.Create("Recording", midPoint, _recording.channels, _recordingHZ, false);
                record.Clip.SetData(samples, 0);

                _speechToText.OnListen(record);

                bFirstBlock = !bFirstBlock;
            }
            else
            {
                // calculate the number of samples remaining until we ready for a block of audio, 
                // and wait that amount of time it will take to record.
                int remaining = bFirstBlock ? (midPoint - writePos) : (_recording.samples - writePos);
                float timeRemaining = (float)remaining / (float)_recordingHZ;

                yield return new WaitForSeconds(timeRemaining);
            }
        }

        yield break;
    }



    // TONE ZONE
    private IEnumerator Examples()
    {
        //  Analyze tone
        if (!_toneAnalyzer.GetToneAnalyze(OnGetToneAnalyze, OnFail, _stringToTestTone1))
            Log.Debug("ExampleToneAnalyzer.Examples()", "Failed to analyze!");

        while (!_analyzeToneTested)
            yield return null;

        Log.Debug("ExampleToneAnalyzer.Examples()", "Tone analyzer examples complete.");
    }


    // Handle response from Tone Analysis 
    private void OnGetToneAnalyze(ToneAnalyzerResponse resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleToneAnalyzer.OnGetToneAnalyze()", "{0}", customData["json"].ToString());

        ResultsField.text = (customData["json"].ToString());  // works but long and cannot read

        // Log Analysis Repsonse
        Log.Debug("$$$$$ TONE LOG 0 ANGER", "{0}", resp.document_tone.tone_categories[0].tones[0].score); // ANGER resp.document_tone.tone_categories [0].tones [0].score);
        Log.Debug("$$$$$ TONE LOG 1 DISGUST", "{0}", resp.document_tone.tone_categories[0].tones[1].score); // DISGUST
        Log.Debug("$$$$$ TONE LOG 2 FEAR", "{0}", resp.document_tone.tone_categories[0].tones[2].score); // FEAR
        Log.Debug("$$$$$ TONE LOG 3 JOY", "{0}", resp.document_tone.tone_categories[0].tones[3].score); // JOY
        Log.Debug("$$$$$ TONE LOG 4 SAD", "{0}", resp.document_tone.tone_categories[0].tones[4].score); // SADNESS

        Log.Debug("$$$$$ TONE ANALYTICAL", "{0}", resp.document_tone.tone_categories[1].tones[0].score); // ANALYTICAL
        Log.Debug("$$$$$ TONE CONFIDENT", "{0}", resp.document_tone.tone_categories[1].tones[1].score); //  CONFIDENT
        Log.Debug("$$$$$ TONE TENTATIVE", "{0}", resp.document_tone.tone_categories[1].tones[2].score); //  TENTATIVE

        // EMOTION
        // using indexes to arrays is poor technique for linking to specific Emotion values
        // but we dont have better examples.  Perhaps a string compare, but thats more time
        if (resp.document_tone.tone_categories[0].tones[0].score > emotion_threshold)
        {
            EmotionText.text = "Anger";
            
            string categoryId = resp.document_tone.tone_categories[0].category_id;
            string toneId = resp.document_tone.tone_categories[0].tones[0].tone_id;
            double score = resp.document_tone.tone_categories[0].tones[0].score;
            
            angerHandler.handleToneChange(categoryId, toneId, score);
        }
        else if (resp.document_tone.tone_categories[0].tones[1].score > emotion_threshold)
        {
            EmotionText.text = "Disgust";
            
            string categoryId = resp.document_tone.tone_categories[0].category_id;
            string toneId = resp.document_tone.tone_categories[0].tones[1].tone_id;
            double score = resp.document_tone.tone_categories[0].tones[1].score;
            
            disgustHandler.handleToneChange(categoryId, toneId, score);

        }
        else if (resp.document_tone.tone_categories[0].tones[2].score > emotion_threshold)
        {
            EmotionText.text = "Fear";
            
            string categoryId = resp.document_tone.tone_categories[0].category_id;
            string toneId = resp.document_tone.tone_categories[0].tones[2].tone_id;
            double score = resp.document_tone.tone_categories[0].tones[2].score;
            
            fearHandler.handleToneChange(categoryId, toneId, score);
        }
        else if (resp.document_tone.tone_categories[0].tones[3].score > emotion_threshold)
        {
            EmotionText.text = "Joy";
        
            string categoryId = resp.document_tone.tone_categories[0].category_id;
            string toneId = resp.document_tone.tone_categories[0].tones[3].tone_id;
            double score = resp.document_tone.tone_categories[0].tones[3].score;
            
            joyHandler.handleToneChange(categoryId, toneId, score);

        }
        else if (resp.document_tone.tone_categories[0].tones[4].score > emotion_threshold)
        {
            EmotionText.text = "Sadness";

            string categoryId = resp.document_tone.tone_categories[0].category_id;
            string toneId = resp.document_tone.tone_categories[0].tones[4].tone_id;
            double score = resp.document_tone.tone_categories[0].tones[4].score;
            
            sadnessHandler.handleToneChange(categoryId, toneId, score);

        }

        // Language tone - https://www.ibm.com/watson/developercloud/tone-analyzer/api/v3/
        else if (resp.document_tone.tone_categories[1].tones[0].score > emotion_threshold)
        {
            EmotionText.text = "Analytical";
            string categoryId = resp.document_tone.tone_categories[1].category_id;
            string toneId = resp.document_tone.tone_categories[1].tones[0].tone_id;
            double score = resp.document_tone.tone_categories[1].tones[0].score;
            
            analyticalHandler.handleToneChange(categoryId, toneId, score);
        }
        else if (resp.document_tone.tone_categories[1].tones[1].score > emotion_threshold)
        {
            EmotionText.text = "Confident";

            string categoryId = resp.document_tone.tone_categories[1].category_id;
            string toneId = resp.document_tone.tone_categories[1].tones[1].tone_id;
            double score = resp.document_tone.tone_categories[1].tones[1].score;
            
            confidentHandler.handleToneChange(categoryId, toneId, score);
        }
        else if (resp.document_tone.tone_categories[1].tones[2].score > emotion_threshold)
        {
            EmotionText.text = "Tentative";
            string categoryId = resp.document_tone.tone_categories[1].category_id;
            string toneId = resp.document_tone.tone_categories[1].tones[2].tone_id;
            double score = resp.document_tone.tone_categories[1].tones[2].score;
            
            tentativeHandler.handleToneChange(categoryId, toneId, score);
        }


        // OTHER TEXT - Formatting for On Screen dump - LATER - pretty this up to use standard DESERIALIZE methods and table
        string RAW = (customData["json"].ToString());  // works but long and cannot read
                                                       //RAW = string.Concat("Tone Response \n", RAW); 
        RAW = Regex.Replace(RAW, "tone_categories", " \\\n");
        RAW = Regex.Replace(RAW, "}", "} \\\n");
        RAW = Regex.Replace(RAW, "tone_id", " ");
        RAW = Regex.Replace(RAW, "tone_name", " ");
        RAW = Regex.Replace(RAW, "score", " ");
        RAW = Regex.Replace(RAW, @"[{\\},:]", "");
        RAW = Regex.Replace(RAW, "\"", "");
        ResultsField.text = RAW;

        _analyzeToneTested = true;
    }


    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleRetrieveAndRank.OnFail()", "Error received: {0}", error.ToString());
    }


    // For Speech Recognition -- commands to UI/Experience
    private void OnRecognize(SpeechRecognitionEvent result)
    {
        if (result != null && result.results.Length > 0)
        {
            foreach (var res in result.results)
            {
                // Commands
                /* at present we dont recognize any Commands, so this section is commented out
                and decimated the code, so if you want it back read Ryan's Example 4 Robot
                foreach (var alt in res.alternatives)
                {
                    string text = string.Format("{0} ({1}, {2:0.00})\n", alt.transcript, res.final ? "Final" : "Interim", alt.confidence);
                    Log.Debug("ExampleStreaming.OnRecognize()", text);
                    ResultsField.text = text;

                   // ENTERING THE TONE ZONE - when the utterance contains this word
                    if (alt.transcript.Contains("reset"))
                    {
                        ResetAction();
                    }

                }
                */

                // Log Keywords
                if (res.keywords_result != null && res.keywords_result.keyword != null)
                {
                    foreach (var keyword in res.keywords_result.keyword)
                    {
                        Log.Debug("ExampleStreaming.OnRecognize()", "keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time);
                        //ResultsField.text = "tone analyzed! 222";
                    }
                }

                // Log Alternative Words
                if (res.word_alternatives != null)
                {
                    foreach (var wordAlternative in res.word_alternatives)
                    {
                        Log.Debug("ExampleStreaming.OnRecognize()", "Word alternatives found. Start time: {0} | EndTime: {1}", wordAlternative.start_time, wordAlternative.end_time);
                        foreach (var alternative in wordAlternative.alternatives)
                            Log.Debug("ExampleStreaming.OnRecognize()", "\t word: {0} | confidence: {1}", alternative.word, alternative.confidence);
                    }
                }
            }
        }
    }

    private void OnRecognizeSpeaker(SpeakerRecognitionEvent result)
    {
        if (result != null)
        {
            foreach (SpeakerLabelsResult labelResult in result.speaker_labels)
            {
                Log.Debug("ExampleStreaming.OnRecognize()", string.Format("speaker result: {0} | confidence: {3} | from: {1} | to: {2}", labelResult.speaker, labelResult.from, labelResult.to, labelResult.confidence));
            }
        }
    }
}
