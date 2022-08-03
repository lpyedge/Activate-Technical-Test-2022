using System.ComponentModel;
using System.Runtime.Serialization;

namespace Bakery
{ 
    public enum EAdminLogType : int
    {
        Login = 1,
        SiteConfig = 2,
        Product = 3,
    }
    
    public enum ESiteTemplate : int
    {
        Template1 = 1,
        Template2 = 2,
    }

}