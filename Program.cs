using System;
using System.Windows.Forms;

namespace ShragaShow {
	internal static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			var path = args.Length > 0 ? args[0] : $"{Environment.GetEnvironmentVariable("USERPROFILE")}/Pictures";
			Application.Run(new Form1(path));
			Data.SaveToDisk();
		}
	}
}
