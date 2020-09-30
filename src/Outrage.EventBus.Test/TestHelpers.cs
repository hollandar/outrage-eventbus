using System;
using System.Threading.Tasks;

namespace Outrage.EventBus.Test
{
    public static class TestHelpers
    {/// <summary>
     /// Delay until a condition is true
     /// </summary>
     /// <param name="condition">condition</param>
     /// <param name="seconds">seconds to wait, defaults to 30</param>
     /// <returns></returns>
        public static async Task DelayUntil(Func<bool> condition, int seconds = 1)
        {
            int cycles = seconds * 1000;
            while (cycles > 0 && !condition())
            {
                await Task.Delay(100);
                cycles -= 100;
            }
        }
    }
}
