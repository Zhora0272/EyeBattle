using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Smashlab
{
    public class Format
    {
        public static string ShortenNumber(int number)
        {
            float num = number;
            string result;
            string[] scoreNames = new string[] { "", "k", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz", };
            int i;

            for (i = 0; i < scoreNames.Length; i++)
                if (num < 900)
                    break;
                else num = Mathf.Floor(num / 100f) / 10f;

            if (num == Mathf.Floor(num))
                result = num.ToString() + scoreNames[i];
            else result = num.ToString("F1") + scoreNames[i];
            return result;
        }
    }
    
}
