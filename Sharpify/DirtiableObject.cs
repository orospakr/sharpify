﻿using Sharpify.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sharpify
{
    public class DirtiableObject : IGranularDirtiable
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
                if (PropertyChanged != null)
                {
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

        private bool CheckForDirtyDirtiable(PropertyInfo prop)
        {
            var dirtiable = prop.GetValue(this) as IDirtiable;
            if (dirtiable != null)
            {
                if (!dirtiable.IsClean()) return true;
            }
            return false;
        }

        public virtual bool IsFieldDirty(string field)
        {
            if (CheckForDirtyDirtiable(this.GetType().GetProperty(field))) return true;

            return Dirty.Contains(field);
        }

        public virtual bool IsFieldDirty(PropertyInfo field)
        {
            if (CheckForDirtyDirtiable(field)) return true;
            return Dirty.Contains(field.Name);
        }

        public virtual bool IsClean()
        {
            foreach (var prop in this.GetType().GetProperties())
            {
                if (CheckForDirtyDirtiable(prop)) return false;
            }
            return Dirty.Count == 0;
        }

        public DirtiableObject()
        {
            Dirty = new HashSet<string>();
        }
    }
}
