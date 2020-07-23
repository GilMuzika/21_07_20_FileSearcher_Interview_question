using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21_07_20_FileSearcher_Interview_question.VMandLogic.composite
{

    /// <summary>
    /// Composite And Builder Design Pattern With A Tree
    /// https://www.c-sharpcorner.com/article/composite-design-pattern/
    /// </summary>
    public abstract class Component
    {        
        protected string SearchPattern { get; set; }

        public string FileOrDirectory { get; protected set; }

        public abstract void AddChild(Component c);

        public abstract void RemoveChild(Component c);

        public abstract IList<Component> GetChildren();

        public abstract Task NotifyIfMatch();

        public string ShortFileorDirectory(string fileOrDirectory, char indexChar)
        {
            int startInd = fileOrDirectory.LastIndexOf(indexChar) + 1;
            return fileOrDirectory.Substring(startInd, fileOrDirectory.Length - startInd);
        }
        public string FileorDirectoryWithoutExtension(string fileOrDirectory, char indexChar, out string extension)
        {
            string extensionInner = string.Empty;
            int startInd = fileOrDirectory.LastIndexOf(indexChar);
            if (startInd == -1)
            {
                extension = extensionInner;
                return fileOrDirectory;
            }
            else
            {
                extensionInner = fileOrDirectory.Substring(startInd + 1, fileOrDirectory.Length - startInd - 1);
            }

            string afterSub = fileOrDirectory.Substring(0, startInd);
            extension = extensionInner;
            return afterSub;
        }

    }
}
