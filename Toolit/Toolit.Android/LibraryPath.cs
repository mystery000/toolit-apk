using System;
using Toolit.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(LibraryPath))]
namespace Toolit.Droid
{
    public class LibraryPath : ILibraryPath
    {
        public string GetLibraryPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
    }
}