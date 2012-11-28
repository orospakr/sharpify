using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    public class ShopifyResourceModel : IResourceModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private HashSet<string> Dirty;

        public void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (name == null)
            {
                throw new ShopifyConfigurationException("Field name is coming up null in SetProperty.  Something's wrong.");
            }
            Console.WriteLine("SETTING PROPERTY {0} to {1}", name, value);
            // thanks to http://danrigby.com/2012/03/01/inotifypropertychanged-the-net-4-5-way/
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
                field = value;
                Dirty.Add(name);
            }
        }

        public void Reset()
        {
            Dirty.Clear();
        }

        public bool IsFieldDirty(string field)
        {
            return Dirty.Contains(field);
        }

        public bool IsClean()
        {
            return Dirty.Count == 0;
        }

        private int? id;
        public int? Id
        {
            get { return id; }
            set
            {
                SetProperty(ref id, value);
            }
        }

        public ShopifyResourceModel()
        {
            Dirty = new HashSet<string>();
        }
    }
}
