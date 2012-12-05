using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    public class ShopifyResourceModel : DirtiableObject, IResourceModel
    {
        private int? id;
        public int? Id
        {
            get { return id; }
            set
            {
                SetProperty(ref id, value);
            }
        }

        private bool _Existing = false;

        public bool IsNew()
        {
            return !_Existing;
        }

        public void SetExisting()
        {
            _Existing = true;
        }

        private DateTime _CreatedAt;
        public DateTime CreatedAt
        {
            get { return _CreatedAt; }
            set
            {
                SetProperty(ref _CreatedAt, value);
            }
        }

        private DateTime _UpdatedAt;
        public DateTime UpdatedAt
        {
            get { return _UpdatedAt; }
            set
            {
                SetProperty(ref _UpdatedAt, value);
            }
        }

        public ShopifyResourceModel()
        {
            
        }
    }
}
