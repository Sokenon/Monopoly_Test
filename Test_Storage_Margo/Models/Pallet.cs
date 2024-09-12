using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Storage_Margo.Models
{
    internal class Pallet : Obj
    {
        Box[]? Boxes { get; set; } = null;

        public Pallet (int width, int height, int depth, int weight = 30)
        {
            this.Weight = weight;
            this.Height = height;
            this.Depth = depth;
            this.Width = width;
        }
        public void SaveToDb()
        {
            Db db = Db.getInstance();
            Dictionary<string, string> parametrs = new Dictionary<string, string>();
            parametrs.Add("@Width", this.Width.ToString());
            parametrs.Add("@Height", this.Height.ToString());
            parametrs.Add("@Depth", this.Depth.ToString());
            parametrs.Add("@Weight", this.Weight.ToString());
            this.Id = db.AddRow("Pallets", parametrs);

        }
        public void PutBoxes ()
        {
            Db db = Db.getInstance();
            DataTable dt = db.TakeBoxes(this.Id);
            Box[] boxes = new Box[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Box box = new Box(int.Parse(dt.Rows[i]["Width"].ToString()), int.Parse(dt.Rows[i]["Height"].ToString()), int.Parse(dt.Rows[i]["Depth"].ToString()), int.Parse(dt.Rows[i]["Weight"].ToString()), DateTime.Parse(dt.Rows[i]["Production_Date"].ToString()), DateTime.Parse(dt.Rows[i]["Expiration_Date"].ToString()));
                box.Id = int.Parse(dt.Rows[i]["Id"].ToString());
                boxes[i] = box;
            }
        }

    }
}
