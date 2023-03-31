using System;
using Toolit.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(LibraryPath))]
namespace Toolit.iOS
{
    public class LibraryPath : ILibraryPath
    {
        public string GetLibraryPath()
        {
            // We need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms (they don't want non-user-generated data in Documents).
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            return System.IO.Path.Combine(documentsPath, "..", "Library");
        }
    }
}