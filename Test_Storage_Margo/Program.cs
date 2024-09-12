using Newtonsoft.Json;
using System.Data;
using Test_Storage_Margo.Models;

Db db = Db.getInstance();

DataTable dt = db.TakePalletsOrderByExp();

Console.WriteLine("Паллеты отсортированные по сроку годности:");
foreach (DataRow row in dt.Rows)
{
    Console.WriteLine($"\tСрок годности: {DateTime.Parse(row["Exp_Date_Pallet"].ToString()).ToString("yyyy-MM-dd")}");
    Console.WriteLine($"\tСписок паллет:");
    Console.WriteLine($"\t\tId\t\t\tВес (кг)");

    string palletsJson = row["Pallets"].ToString();
    dynamic[] palletsArray = JsonConvert.DeserializeObject<dynamic[]>(palletsJson);
    foreach (var item in palletsArray)
    {
        Console.WriteLine($"\t\t{item.Id}\t\t\t{item.Weight_Pallet_With_Boxes}");
    }
    Console.WriteLine("");
}

Console.WriteLine("3 паллеты с наибольшим сроком годности:");
dt = db.Take3Pallets();
Console.WriteLine($"\t\tId\t\t\tСрок годности\t\t\tОбъем");
foreach (DataRow row in dt.Rows)
{
    Console.WriteLine($"\t\t{row["Id"].ToString()}\t\t\t{DateTime.Parse(row["Exp_Date_Pallet"].ToString()).ToString("yyyy-MM-dd")}\t\t\t{row["Volume"].ToString()}");
}
Console.ReadLine();