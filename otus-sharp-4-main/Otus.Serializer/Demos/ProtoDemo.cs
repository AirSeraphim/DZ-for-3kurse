namespace Otus.Serializer.Demos
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using ProtoBuf;
    using ProtoBuf.Meta;

    [Serializable]
    [ProtoContract]
    public class TestMe
    {
        public TestMe()
        {
            Doubles = new List<double>();
        }

        [ProtoMember(1)]
        public List<double> Doubles { get; set; }
    }

    public class ProtoDemo
    {
        public void Show()
        {



            var r = new Random();
            var l = new List<TestMe>();

            for (var j = 0; j < 1000; j++)
            {
                var t = new TestMe();
                for (var i = 0; i < 10000; i++)
                {
                    t.Doubles.Add(r.NextDouble() * 10000);
                }
                l.Add(t);
            }


            var bf = new BinaryFormatter();
            var sw = new Stopwatch();
            sw.Start();
            using (var fs = new FileStream("binary.bin", FileMode.Create))
            {
                bf.Serialize(fs, l);
            }
            sw.Stop();

            Console.WriteLine($"Binary: {sw.ElapsedMilliseconds}");

            var model = RuntimeTypeModel.Create();
            model.Add(typeof(List<TestMe>), true);
            sw.Reset();
            sw.Start();


            using (var fs = new FileStream("proto.bin", FileMode.Create))
            {
                model.Serialize(fs, l);
            }
            sw.Stop();

            Console.WriteLine($"Proto: {sw.ElapsedMilliseconds}");


            sw.Reset();
            sw.Start();


            using (var fs = new FileStream("proto.bin", FileMode.Create))
            {
                model.Serialize(fs, l);
            }
            sw.Stop();

            Console.WriteLine($"Proto: {sw.ElapsedMilliseconds}");

        }
    }
}