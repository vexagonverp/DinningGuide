using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dinning_Guide.Helper
{
    public static class CustomerRole
    {
        public const string Administrator = " Administrator";
        public const string Owner = "Owner";
        public const string User = "User";
        public const string All = Administrator + "," + Owner + "," + User;

    }
}