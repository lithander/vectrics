using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vectrics
{
    public class ContinuousFloat
    {
        public ContinuousFloat(float value, float cps = 1.0f)
        {
            _changePerSecond = cps;
            _value = value;
        }

        public ContinuousFloat(float value, float cps, float min, float max)
        {
            _changePerSecond = cps;
            _value = value;
            _minValue = min;
            _maxValue = max;
        }

        public float _value;
        private float _changePerSecond = 5.0f;
        private float _minValue = float.MinValue;
        private float _maxValue = float.MaxValue;

        public float MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public float MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        public float ChangePerSecond
        {
            get { return _changePerSecond; }
            set { _changePerSecond = value; }
        }

        public void Set(float value)
        {
            _value = value;
        }

        public void Change(float value, float deltaTime)
        {
            Change(value, deltaTime, _changePerSecond);
        }

        public void Change(float value, float deltaTime, float changePerSecond)
        {
            float dv = value - _value;
            _value += Math.Min(deltaTime * changePerSecond, Math.Abs(dv)) * Math.Sign(dv);
            _value = CgMath.Clamp(_value, _minValue, _maxValue);
        }

        public float Value
        {
            get { return _value; }
        }
    }
}
