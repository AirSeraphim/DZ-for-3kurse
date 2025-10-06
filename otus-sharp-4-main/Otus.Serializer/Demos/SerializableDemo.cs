using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Otus.Serializer
{

    
    public class GameSave : ISerializable
    {
        public GameSave()
        {
        }

        public int A { get; set; }

        public DateTime? SaveTimestamp { get; set; }

        // Из ISerializable, вызывается при сериализации
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Console.WriteLine("Serializing");
         info.AddValue("saveTimestamp", DateTime.Now);
            // ...
        }

        // Особый конструктор - при десериализации
        public GameSave(SerializationInfo info, StreamingContext context)
        {
            Console.WriteLine("Deserializing");
            SaveTimestamp=info.GetDateTime("saveTimestamp");
        }

        public override string ToString()
        {
           return $"SaveTimestamp: {SaveTimestamp}";
        }
    }

    public class SerializableDemo
    {
        public void Show()
        {
             var bf = new BinaryFormatter();

            var before = new GameSave();

            Console.WriteLine($"before: {before}");

            using (var fs = new FileStream("demoDataFiles\\serDemo.bin", FileMode.Create))
            {
                bf.Serialize(fs, before);
            }
Console.Write("Ждем: " );
            Console.ReadLine();
            using (var fs = new FileStream("demoDataFiles\\serDemo.bin", FileMode.Open))
            {
                var after = (GameSave)bf.Deserialize(fs);

                Console.WriteLine($"after: {after}");
            }
        }
    }
}