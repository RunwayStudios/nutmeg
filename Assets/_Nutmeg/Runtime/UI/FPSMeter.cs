using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FPSMeter : MonoBehaviour
    {
        [SerializeField] private bool calculateFramerate = true;
        [SerializeField] private int frameBuffer = 30;
        [SerializeField] private Text displayFieldAverageFPS;
        [SerializeField] private Text displayFieldAverageMS;
        [SerializeField] private Text displayFieldMinMS;
        [SerializeField] private Text displayFieldMaxMS;

        private List<float> last30Frames;

        // Start is called before the first frame update
        void Start()
        {
            if (displayFieldAverageFPS == null ||
                displayFieldAverageFPS == null ||
                displayFieldAverageFPS == null ||
                displayFieldAverageFPS == null)
            {
                Debug.LogError("missing Text field references");
                calculateFramerate = false;
            }

            last30Frames = new List<float>(frameBuffer);
            for (int i = 0; i < frameBuffer; i++)
            {
                last30Frames.Add(.1f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!calculateFramerate)
                return;
            
            last30Frames.RemoveAt(0);
            last30Frames.Add(Time.deltaTime);

            float min = 10000f;
            float max = 0f;
            float sum = 0f;
            for (int i = 0; i < frameBuffer; i++)
            {
                sum += last30Frames[i];

                if (last30Frames[i] < min)
                    min = last30Frames[i];

                if (last30Frames[i] > max)
                    max = last30Frames[i];
            }

            displayFieldAverageFPS.text = $"{1 / (sum / frameBuffer):0.0}";
            displayFieldAverageMS.text = $"{sum / frameBuffer * 1000:0.0}";
            displayFieldMinMS.text = $"{min * 1000:0.0}";
            displayFieldMaxMS.text = $"{max * 1000:0.0}";
        }
    }
}