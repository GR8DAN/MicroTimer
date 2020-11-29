using System;
using System.Linq;

namespace MicroTimerTest
{
    public static class Dice
    {
        //Store number of rolls of each number
        static long[] rolls = { 0, 0, 0, 0, 0, 0 };
        //Use random for rolling
        static readonly Random roller = new Random();
        //Store the roll;
        static int lastRoll = 0;
        //Perform a dice roll
        public static void Roll()
        {
            //Roll a number 0 to 5
            lastRoll = roller.Next(0, 6);
            //Increment rolled count
            //and correct 0 to 5 -> 1 to 6
            ++rolls[lastRoll++];
        }
        //Get last rolled number
        public static int LastRoll => lastRoll;
        //Totals of each number rolled
        public static long[] Rolls => rolls;
        //Total of all rolls performed
        public static long TotalRolls => rolls.Sum();
        //Total value of all rolled numbers
        public static long[] TotalsOfRolledNumbers => rolls.Select((number, index) => number * (index + 1)).ToArray();
        //Total of all values rolled
        public static long TotalValues => TotalsOfRolledNumbers.Sum();
        //Mean number rolled (double cast required to prevent integer only division) 
        public static double MeanValueRolled => TotalValues / (double)TotalRolls;
    }
}
