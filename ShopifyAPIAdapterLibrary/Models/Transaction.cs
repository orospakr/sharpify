using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Models
{
    /// <summary>
    /// http://api.shopify.com/transactions.html
    /// 
    /// http://wiki.shopify.com/Transaction_%28API%29
    /// </summary>
    public class Transaction : ShopifyResourceModel
    {
        private Double _Amount;
        public Double Amount
        {
            get { return _Amount; }
            set {
                SetProperty(ref _Amount, value);
            }
        }


        // TODO: Enum?
        // UNKNOWN
        private string _Authorization;
        public string Authorization
        {
            get { return _Authorization; }
            set {
                SetProperty(ref _Authorization, value);
            }
        }


        // TODO: Enum?
        private string _Gateway;
        public string Gateway
        {
            get { return _Gateway; }
            set {
                SetProperty(ref _Gateway, value);
            }
        }


        // TODO: Enum?
        // UNKNOWN
        private string _Kind;
        public string Kind
        {
            get { return _Kind; }
            set {
                SetProperty(ref _Kind, value);
            }
        }


        private IHasOne<Order> _Order;
        public IHasOne<Order> Order
        {
            get { return _Order; }
            set {
                SetProperty(ref _Order, value);
            }
        }


        // TODO: Enum?
        private string _Status;
        public string Status
        {
            get { return _Status; }
            set {
                SetProperty(ref _Status, value);
            }
        }


        private bool? _Test;
        public bool? Test
        {
            get { return _Test; }
            set {
                SetProperty(ref _Test, value);
            }
        }


        private Receipt _Receipt;
        public Receipt Receipt
        {
            get { return _Receipt; }
            set {
                SetProperty(ref _Receipt, value);
            }
        }


        public Transaction()
        {
        }
    }
}
