using System;

namespace PictureAuth.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiresPictureAuthAttribute : Attribute
    {
    }
}