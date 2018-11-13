using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClass.Game
{
    public class Countdown
    {
        public enum CountdownType
        {
            S_COUNTDOWN_NO_LIMIT,
            S_COUNTDOWN_USE_SPECIFIED,
            S_COUNTDOWN_USE_DEFAULT,
            S_COUNTDOWN_USE_ALL
        }
        public CountdownType Type { set; get; }

        public float Current;
        public float Max;
        public Countdown(CountdownType type = CountdownType.S_COUNTDOWN_NO_LIMIT, float current = 0, float max = 0)
        {
            Type = type;
            Current = current;
            Max = max;
        }
        public bool hasTimedOut()
        {
            if (Type == CountdownType.S_COUNTDOWN_NO_LIMIT)
                return false;
            else
                return Current >= Max;
        }
    };
}
