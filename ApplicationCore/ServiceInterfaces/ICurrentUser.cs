using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.ServiceInterfaces
{
    public interface ICurrentUser
    {
        int UserId { get;  }
        bool isAuthenticated { get; }
        string Email { get; }
        string FullName { get; }
        bool isAdmin { get; }
        bool isSuperAdmin { get; }
    }
}
