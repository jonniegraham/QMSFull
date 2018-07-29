using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Utilities
{
    public class Utilities
    {
        private static readonly Random Random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Climbs visual tree and returns the first instance of a UIElement type T, or null.
        /// </summary>
        /// <typeparam name="T">Type of Ancestor UIElement searched for</typeparam>
        /// <param name="childElement">Child element</param>
        /// <returns>First ancestor UIElement instance of type T, or null</returns>
        public static T FindVisualParent<T>(UIElement childElement) where T : UIElement
        {
            var parent = childElement;
            while (parent != null)
            {
                if (parent is T correctlyTyped)
                {
                    return correctlyTyped;
                }
                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }
    }
}
