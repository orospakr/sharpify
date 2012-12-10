using ShopifyAPIAdapterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary
{
    //public interface IDirtiableList<T> : IList<T> where T : IDirtiable
    //{
    //}

    public class DirtiableList<T> : List<T>, IDirtiable where T : IDirtiable
    {
        public void Reset()
        {
        }

        public bool IsClean()
        {
            var dirties = from p in this where !p.IsClean() select p;
            return dirties.Count() == 0;
        }
    }
}
