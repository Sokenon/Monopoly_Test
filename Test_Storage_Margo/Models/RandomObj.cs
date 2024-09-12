using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Storage_Margo.Models
{
    internal class RandomObj
    {
        int Count;
        public Box[] randBoxes = null;
        public Pallet[] randPallets = null;

        public RandomObj (int count, string obj)
        {
            Count = count;
            if (obj == "Pallet")
            {
                Pallet[] pallets = new Pallet[Count];
                int[] width = new int[] { 150, 200, 180, 170, 160 };
                int[] height = new int[] { 20, 25, 15 };
                int[] depth = new int[] { 100, 140, 120 };
                Random rnd = new Random();

                for (int i = 0; i < Count; i++)
                {
                    Pallet pallet = new Pallet(width[rnd.Next(0, 4)], height[rnd.Next(0, 2)], depth[rnd.Next(0, 2)]);
                    pallets[i] = pallet;
                }
                randPallets = pallets;
            }
            if (obj == "Box")
            {
                Box[] boxes = new Box[Count];
                int[] width = new int[] { 50, 25, 80, 170, 155, 90, 125 };
                int[] height = new int[] { 25, 60, 100, 50, 70 };
                int[] depth = new int[] { 50, 25, 80, 170, 140, 90, 125 };
                int[] weight = new int[] { 40, 77, 101, 68, 44, 95, 124 };
                DateTime[] expiration = new DateTime[] { new DateTime(2025, 01, 10), new DateTime(2025, 04, 14), new DateTime(2026, 10, 11), new DateTime(2024, 12, 31), new DateTime(2025, 08, 25) };
                Random rnd = new Random();

                for (int i = 0; i < Count; i++)
                {
                    Box box = new Box(width[rnd.Next(0, 6)], height[rnd.Next(0, 4)], depth[rnd.Next(0, 6)], weight[rnd.Next(0, 6)], expiration[rnd.Next(0, 4)]);
                    boxes[i] = box;
                }
                randBoxes = boxes;
            }

        }
    }
}
