using System;
using System.Collections.Generic;
using System.Text;

public class BinaryHash : IHashableData
{
    public string HashData(string data)
    {
        return Encode(data);
    }

    public string DeHashData(string data)
    {
        return Decode(data);
    }

    public string Encode(string data)
    {
        string hash = String.Empty;
        
        var bytes = Encoding.Default.GetBytes(data);

        int index = 0;
        
        foreach (var item in bytes)
        {
            var stringItem = item.ToString();

            var removedString = stringItem.Remove(stringItem.Length - 1);

            var lastByte = item % 10;
            
            hash += removedString + " " + lastByte + UnityEngine.Random.Range(1,9);
        }
        
        return hash;
    }
    
    public string Decode(string data)
    {
        ///decode example test mode

        List<byte> byteData = new List<byte>();

        StringBuilder encoding = new StringBuilder();

        int skipIndex = 0;
        
        foreach (var item in data)
        {
            if (skipIndex > 0)
            {
                if (skipIndex == 2)
                {
                    skipIndex -= 1;
                    
                    encoding.Append(item);
                    continue;
                }
                if(skipIndex == 1)
                {
                    skipIndex -= 1;
                    
                    byteData.Add(Convert.ToByte(encoding.ToString()));
                    encoding.Clear();
                    
                    continue;
                }
            }
            
            if (item.ToString() == " ")
            {
                skipIndex = 2;
            }
            else 
            {
                encoding.Append(item);
            }
        }

        return Encoding.ASCII.GetString(byteData.ToArray());
    }
}