using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speech_Recognition_test
{
    public partial class Game
    {
        public static partial class Player
        {
            public static class Pack
            {
                public class Item
                {
                    public string Name { get; }
                    public uint Count { get; set; }

                    public Item(string name, uint count = 0)
                    {
                        Name = name;
                        Count = count;
                    }
                }

                public class Compartment
                {
                    public uint Index;
                    public string Name;
                    public List<Item> Contents;

                    public Compartment(string name)
                    {
                        Name = name;
                        Index = 0;
                        Contents = new List<Item>();
                    }

                    public bool UseItem(string itemName)
                    {
                        var itemIndex = Contents.FindIndex(x => x.Name == itemName);
                        if (itemIndex < 0)
                            return false;

                        //TODO: scroll till item is found

                        var item = Contents[itemIndex];
                        if (item.Count > 1)
                            item.Count--;
                        else if (item.Count == 1)
                            Contents.RemoveAt(itemIndex);
                        return true;
                    }

                    public void AddItem(string itemName)
                    {
                        var item = Contents.FirstOrDefault(x => x.Name == itemName);
                        if (item == null)
                            Contents.Add(new Item(itemName));
                        else
                            item.Count++;
                    }
                }

                private static int currentCompartment = 0;
                private static readonly Compartment Items = new Compartment("Items");
                private static readonly Compartment Balls = new Compartment("Balls");
                private static readonly Compartment KeyItems = new Compartment("KeyItems");
                private static readonly Compartment HmTms = new Compartment("HM/TM");
                private static readonly Compartment[] Compartments = {Items, Balls, KeyItems, HmTms};

                public static void ResetCompartments()
                {
                    foreach (var comp in Compartments)
                        comp.Index = 0;
                }

                public static bool UseItem(string itemName, string compartment)
                {
                    //TODO: shift to compartment
                    var r = Compartments.FirstOrDefault(x => x.Name == compartment)?.UseItem(itemName);
                    return r.HasValue && r.Value;
                }

                public static void AddItem(string itemName, string compartment)
                {
                    var c = Compartments.FirstOrDefault(x => x.Name == compartment);
                    if (c == null)
                        throw new ArgumentException("Compartment does not exist");
                    c.AddItem(itemName);
                }
            }
        }
    }
}