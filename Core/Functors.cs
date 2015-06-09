using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vectrics
{
    public static class Functor
    {
        //bound-factor looks like a inverted parabola with peak at 0.5 and -0.5 and 0 at -1, 0 and 1
        public static Func<float, float> DoubleExponentialSmoothingUnitBounded(float alpha, float beta)
        {
            float smooth = 0.0f;
            float trend = 0.0f;
            return (x) =>
            {
                float abs = Math.Abs(smooth);
                float bound = 4 * abs * (1 - abs);
                float prev = smooth;
                smooth = alpha * x + (1.0f - alpha) * (prev + trend * bound);
                trend = beta * (smooth - prev) + (1 - beta) * trend;
                return smooth;
            };
        }

        //bound-factor looks like gauss distribution with peak at 0.5 and -0.5 and 0 at -1, 0 and 1
        public static Func<float, float> DoubleExponentialSmoothingUnitGaussBounded(float alpha, float beta)
        {
            float smooth = 0.0f;
            float trend = 0.0f;
            return (x) =>
            {
                float abs = Math.Abs(smooth);
                float bound = 4 * abs * (1 - abs);
                float boundedTrend = trend * bound * bound;
                float prev = smooth;
                smooth = alpha * x + (1.0f - alpha) * (prev + boundedTrend);
                trend = beta * (smooth - prev) + (1 - beta) * trend;
                return smooth;
            };
        }

        //simple exponential smoothing
        public static Func<float, float> SimpleExponentialSmoothing(float alpha)
        {
            float smooth = 0.0f;
            return (x) =>
            {
                smooth = (1.0f - alpha) * smooth + alpha * x;
                return smooth;
            };
        }

        //exponential smoothing with trending
        public static Func<float, float> DoubleExponentialSmoothing(float alpha, float beta)
        {
            float smooth = 0.0f;
            float trend = 0.0f;
            float prev = 0.0f;
            return (x) =>
            {
                prev = smooth;
                smooth = alpha * x + (1.0f - alpha) * (prev + trend);
                trend = beta * (smooth - prev) + (1 - beta) * trend;
                return smooth;
            };
        }

        //simple moving average with no weights applied
        public static Func<float, float> SimpleMovingAverage(int period)
        {
            float[] window = new float[period];
            int index = 0;
            float sum = 0;
            return (x) =>
            {
                sum -= window[index];
                window[index++] = x;
                if (index >= period)
                    index = 0;
                sum += x;
                return sum / period;
            };
        }

        //simple moving average with no weights applied
        public static Func<float, float> Towards(float maxDelta, float startValue = 0.0f)
        {
            float bounds = maxDelta;
            float value = startValue;
            return (x) =>
            {
                float delta = CgMath.Clamp(x - value, -bounds, bounds);
                value += delta;
                return value;
            };
        }
    }
}
