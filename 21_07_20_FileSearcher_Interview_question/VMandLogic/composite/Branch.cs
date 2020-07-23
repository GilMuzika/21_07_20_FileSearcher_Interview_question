using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _21_07_20_FileSearcher_Interview_question.VMandLogic.composite
{
    /// <summary>
    /// Composite class
    /// </summary>
    public class Branch : Component
    {

        public Branch(string directory, string searchPattern)
        {
            FileOrDirectory = directory;
            SearchPattern = searchPattern;
        }

        private IList<Component> _children = new List<Component>();

        public override void AddChild(Component c)
        {
            _children.Add(c);
        }

        public override IList<Component> GetChildren()
        {
            return _children;
        }

        public override void RemoveChild(Component c)
        {
            _children.Remove(c);
        }

        public async override Task NotifyIfMatch()
        {


            await Task.Run(async() => 
            {
                ViewModel._notifyIfMatchExecutionsNumber++;

                foreach (Component s in _children)
                {                   
                    await s.NotifyIfMatch();
                }
                string shortDirectory = ShortFileorDirectory(FileOrDirectory, '\\');

                if (shortDirectory.Contains(SearchPattern))
                    ViewModel.AddResult(shortDirectory + "- dir");
            });
        }

    }
}
