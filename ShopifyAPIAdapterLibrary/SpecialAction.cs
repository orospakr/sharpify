using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary
{
    /// <summary>
    /// A property of this this placed on a IResourceModel
    /// in order to indicate a special action.
    /// 
    /// Then, they are specified to IResourceView#CallAction()
    /// using the "as MemberExpression" pattern.
    /// </summary>
    public class SpecialAction
    {
        SpecialAction()
        {
            throw new ShopifyConfigurationException("SpecialActions should never be created.  They only exist to provide a type-safe property type indicating special, per-resource instance callable actions.");
        }
    }
}
