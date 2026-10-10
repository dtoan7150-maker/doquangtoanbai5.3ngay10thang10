
using System;
using System.Windows.Forms;

namespace Bai5._3_DanhSachSanPham
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}
