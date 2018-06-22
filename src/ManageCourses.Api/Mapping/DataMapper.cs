using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Api.Mapping
{
    public static class DataMapper
    {
        private static readonly Dictionary<string, string> _dictData = new Dictionary<string, string>();
        static DataMapper()
        {
            _dictData.Add("HE", "Higher education programme");
            _dictData.Add("SD", "School Direct training programme");
            _dictData.Add("SS", "School Direct (salaried) training programme");
            _dictData.Add("SC", "SCITT programme ");
            _dictData.Add("TA", "PG Teaching Apprenticeship");
            _dictData.Add("empty", "Recommendation for QTS");
            _dictData.Add("PF", "Professional");
            _dictData.Add("PG", "Postgraduate");
            _dictData.Add("BO", "Professional/Postgraduate");
            _dictData.Add("F", "Full time");
            _dictData.Add("P", "Part time");
        }

        public static string GetStringData(string key)
        {
            var returnVal = string.Empty;
            var k = key;
            if (string.IsNullOrWhiteSpace(key))
                k = "empty";

            if (_dictData.ContainsKey(k))
            {
                returnVal = _dictData[k];
            }
            return returnVal;
        }
    }
}
