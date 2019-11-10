using System;
using System.IO;

namespace XASM {
	
	public class Program {
		
		public static void Main ( string[] args ) {
			
			var parser = new XASMParser ( File.ReadAllText ( Path.Combine ( Directory.GetCurrentDirectory (), "main.xasm" ) ) );
			
			var vm = new XVM ();
			vm.script = parser.Compile ();
			vm.Run ();
			
		}
	}
}
