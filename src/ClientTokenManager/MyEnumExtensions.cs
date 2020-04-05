using System.ComponentModel;

namespace Tomand.ClientTokenManager
{
    public static class MyEnumExtensions
    {
        public static string ToDescriptionString(this GrantType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
            .GetType()
            .GetField(val.ToString())
            .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    } 
}