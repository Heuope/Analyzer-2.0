using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft
{
    class SpenMetric
    {
        public string Variables { get; set; }
        public string Spen { get; set; }

        public SpenMetric(Dictionary<string, int> metr)
        {            
            var strBuilder = new StringBuilder();

            foreach (var item in metr.Keys)
                strBuilder.Append(item + '\n');

            strBuilder.Append("Summary Spen =");

            Variables = strBuilder.ToString();

            var str = new StringBuilder();
            int temp = 0;
            foreach (var item in metr.Values)
            {
                temp += item;
                str.Append(item.ToString() + '\n');
            }

            str.Append(temp.ToString());
            Spen = str.ToString();
        }
    }
}
