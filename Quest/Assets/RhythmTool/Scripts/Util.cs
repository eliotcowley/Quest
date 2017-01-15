using UnityEngine;
using System.Collections;

public static class Util
{
    private static LomontFFT fft = new LomontFFT();

    private static float[] magnitude = new float[0];
    private static float[] streched = new float[0];
    private static float[] mono = new float[0];

    public static float[] GetSpectrum(float[] samples)
    {
        if (mono.Length != samples.Length / 2)
            mono = new float[samples.Length / 2];
           
        for (int i = 0; i < mono.Length; i++)
        {
            float monoSample = (samples[i * 2] + samples[i * 2 + 1]) * .5f;
            mono[i] = monoSample * 1.4f;
        }
  
        fft.RealFFT(mono, true);

        return SpectrumMagnitude(mono);
    }
        
    /// <summary>
    /// Calculates spectral magnitude
    /// </summary>
    /// <returns>
    /// Array of spectral magnitude.
    /// </returns>
    /// <param name='spectrum'>
    /// Complex valued spectrum with complex data stored as alternating real                                       
    /// and imaginary parts
    /// </param>
    public static float[] SpectrumMagnitude(float[] spectrum)
    {
        int l = (spectrum.Length - 2) / 2;

        if (magnitude.Length != l)
            magnitude = new float[l];

        for (int i = 0; i < magnitude.Length; i++)
        {
            int ii = (i * 2) + 2;
            float x = (float)spectrum[ii];
            float y = (float)spectrum[ii + 1];
            magnitude[i] = Mathf.Sqrt((x * x) + (y * y));
        }

        return magnitude;
    }

    
    public static float Sum(float[] input, int start, int end)
    {
        float output = 0;

        for (int i = start; i < end; i++)
        {
            output += input[i];
        }

        return output;
    }

    
    public static float[] Smooth(float[] input, int windowSize)
    {
        //this is "incorrect" (smoothing happens in place, so it will use already smoothed values of preceding indices)
        //but it gives better results when used with beat tracking
        float[] output = input;

        for (int i = 0; i < output.Length; i++)
        {
            float average = 0;

            for (int ii = i - (windowSize / 2); ii < i + (windowSize / 2); ii++)
            {
                if (ii > 0 && ii < output.Length)
                    average += input[ii];
            }

            output[i] = average / windowSize;
        }

        return output;
    }
        
    public static float[] StretchArray(float[] input, int m)
    {
        if (streched.Length != input.Length * m)
            streched = new float[input.Length * m];

        for (int i = 0; i < input.Length - 1; i++)
        {
            for (int ii = 0; ii < m; ii++)
            {
                float f = (float)ii / (float)m;
                streched[(i * m) + ii] = Mathf.Lerp(input[i], input[i + 1], f);
            }
        }

        return streched;
    }
}
