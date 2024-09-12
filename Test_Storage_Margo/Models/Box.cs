using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Storage_Margo.Models
{
    internal class Box : Obj
    {
        public DateTime ProductionDate { get; set; }
        public DateTime ExpirationDate { get; set; } 

        public Box(int width, int height, int depth, int weight, DateTime expiration = new DateTime(), DateTime production = new DateTime())
        {
            Width = width;
            Height = height;
            Depth = depth;
            Weight = weight;
            if (production == new DateTime())
            {
                ProductionDate = new DateTime();
            }
            if (expiration == new DateTime() && ProductionDate == new DateTime())
            {
                throw new ArgumentException("Укажите срок годности коробки или дату производства.");
            }
            if (expiration != new DateTime())
            {
                ExpirationDate = expiration;
            }
            else if (expiration == new DateTime() && ProductionDate != new DateTime())
            {
                ExpirationDate = ProductionDate.AddDays(100);
            }
        }

        public bool CheckPalletSize (int idPallet)
        {
            Db db = Db.getInstance();
            DataTable dt = db.CheckPallet(idPallet);
            if (Width <= int.Parse(dt.Rows[0]["Width"].ToString()) && Depth <= int.Parse(dt.Rows[0]["Depth"].ToString()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void SaveToDb (int idPallet = -1)
        {
            if (idPallet != -1)
            {
                if (CheckPalletSize(idPallet))
                {
                    Db db = Db.getInstance();
                    Dictionary<string, string> parametrs = new Dictionary<string, string>();
                    parametrs.Add("@Width", this.Width.ToString());
                    parametrs.Add("@Height", this.Height.ToString());
                    parametrs.Add("@Depth", this.Depth.ToString());
                    parametrs.Add("@Weight", this.Weight.ToString());
                    parametrs.Add("@Production_Date", ProductionDate.ToString("yyyy-MM-dd"));
                    parametrs.Add("@Expiration_Date", ExpirationDate.ToString("yyyy-MM-dd"));
                    parametrs.Add("@Id_Pallet", idPallet.ToString());
                    this.Id = db.AddRow("Boxes", parametrs);
                }
                else
                {
                    throw new ArgumentException("Коробка не помещается в выбранную паллету.");
                }

            }
            else
            {
                Db db = Db.getInstance();
                Dictionary<string, string> parametrs = new Dictionary<string, string>();
                parametrs.Add("@Width", this.Width.ToString());
                parametrs.Add("@Height", this.Height.ToString());
                parametrs.Add("@Depth", this.Depth.ToString());
                parametrs.Add("@Weight", this.Weight.ToString());
                parametrs.Add("@Production_Date", ProductionDate.ToString("yyyy-MM-dd"));
                parametrs.Add("@Expiration_Date", ExpirationDate.ToString("yyyy-MM-dd"));
                parametrs.Add("@Id_Pallet", "null");
                this.Id = db.AddRow("Boxes", parametrs);
            }
        }
    }
}
