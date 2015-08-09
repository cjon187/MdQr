using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdQr
{
    class global
    {
       public static string connstring = String.Format("Server={0};Port={1};" +
                    "User Id={2};Password={3};Database={4};",
                    "maindev.ddns.net", "5432", "jon",
                    "jon", "jon");


    }
}
