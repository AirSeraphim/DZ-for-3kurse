using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Otus.Serializer
{


    [Serializable]
    public class Foo
    {
        public Foo(int a, int b)
        {
            A = a;
            B = b;
        }
        public int A { get; set; }

        // Не сериализуется

        [NonSerialized]
        public int? B;


        public override string ToString()
        {
            return $"{{ A: {A}, B: {B}}}";
        }
    }


    public class Bar : Foo
    {
        public Bar(int a, int b) : base(a, b)
        { }
    }

    public class AttributeDemo
    {
        public void Show()
        {
            var bf = new BinaryFormatter();

            var before = new Foo(1, 3);

            Console.WriteLine($"before: {before}");

            using (var fs = new FileStream("demoDataFiles\\attributeDemo.bin", FileMode.Create))
            {
                bf.Serialize(fs, before);
            }

            using (var fs = new FileStream("demoDataFiles\\attributeDemo.bin", FileMode.Open))
            {
                var after = (Foo)bf.Deserialize(fs);

                Console.WriteLine($"after: {after}");
            }

        }
    }
}