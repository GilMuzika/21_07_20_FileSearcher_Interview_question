using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _21_07_20_FileSearcher_Interview_question.VMandLogic.composite
{
    /// <summary>
    /// leaf class
    /// </summary>
    public class Leaf : Component
    {

        public Leaf(string file, string searchPattern)
        {
            FileOrDirectory = file;
            SearchPattern = searchPattern;

        }

        public override void AddChild(Component c)
        {
            throw new NotSupportedException("Leaf element cannot add child!");
        }

        public override IList<Component> GetChildren()
        {
            return null;
        }

        public override void RemoveChild(Component c)
        {
            throw new NotSupportedException("Leaf element cannot remove child!");
        }

        public async override Task NotifyIfMatch()
        {            
            await Task.Run(() => 
            {
                ViewModel._notifyIfMatchExecutionsNumber++;

                string extension = string.Empty;
                string shortFile = ShortFileorDirectory(FileOrDirectory, '\\');                
                if (ViewModel.ExtensionInFileNameStatic == false)
                    shortFile = FileorDirectoryWithoutExtension(shortFile, '.', out extension);

                if (shortFile.Contains(SearchPattern))
                    if(ViewModel.ExtensionInFileNameStatic == false)
                        ViewModel.AddResult(shortFile+"{" + extension + "} - file");             
                    else
                        ViewModel.AddResult(shortFile + "- file");
            }); 
        }


    }
}
