using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Otus.Serializer
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TestFile 
    {

        public TestFile()
        {
            Doubles = new List<double>();
        }
        
        public List<double> Doubles { get; set; }


        [IgnoreDataMember]
        [XmlIgnore]
        [JsonIgnore]
        public DateTime SaveDate{get;private set;}

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SaveDate", DateTime.Now);
        }

        public TestFile(SerializationInfo info, StreamingContext context)
        {
            SaveDate = info.GetDateTime("SaveDate");
        }
    }
}