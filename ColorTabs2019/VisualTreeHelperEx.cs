using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorTabs2019
{
    public static class VisualTreeHelperEx
    {
        public static bool IsFocusedRecursive(
            this DependencyObject root
            )
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(root);

            for (var cc = 0; cc < childrenCount; cc++)
            {
                var control = VisualTreeHelper.GetChild(
                    root,
                    cc
                    );

                if (control is FrameworkElement fe)
                {
                    if (fe.IsFocused)
                    {
                        return true;
                    }
                }

                if (control.IsFocusedRecursive())
                {
                    return true;
                }
            }

            return false;
        }

        public static void GetRecursiveByType(
            this DependencyObject root,
            string type,
            ref List<FrameworkElement> result
            )
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(root);

            for (var cc = 0; cc < childrenCount; cc++)
            {
                var control = VisualTreeHelper.GetChild(
                    root,
                    cc
                    );

                if (control is FrameworkElement fe)
                {
                    if (fe.GetType().Name == type)
                    {
                        result.Add(fe);
                        continue;
                    }
                }

                control.GetRecursiveByType(type, ref result);
            }
        }

        public static void GetRecursiveByType<T>(
            this DependencyObject root,
            ref List<T> result
            )
            where T : FrameworkElement
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(root);

            for (var cc = 0; cc < childrenCount; cc++)
            {
                var control = VisualTreeHelper.GetChild(
                    root,
                    cc
                    );

                if (control is T typedControl)
                {
                    result.Add(typedControl);
                    continue;
                }

                control.GetRecursiveByType(ref result);
            }
        }

        public static FrameworkElement GetRecursiveByType<T>(
            this DependencyObject root
            )
            where T : FrameworkElement
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(root);

            for (var cc = 0; cc < childrenCount; cc++)
            {
                var control = VisualTreeHelper.GetChild(
                    root,
                    cc
                    );

                if (control is T typedControl)
                {
                    return typedControl;
                }

                var result = control.GetRecursiveByType<T>();
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static FrameworkElement GetRecursiveByType(
            this DependencyObject root,
            string type
            )
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(root);

            for (var cc = 0; cc < childrenCount; cc++)
            {
                var control = VisualTreeHelper.GetChild(
                    root,
                    cc
                    );

                if (control is FrameworkElement fe)
                {
                    if (fe.GetType().Name == type)
                    {
                        return fe;
                    }
                }

                var result = control.GetRecursiveByType(type);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static FrameworkElement GetRecursiveTextBlockByNameItsText(
            this DependencyObject root,
            string text
            )
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(root);

            for (var cc = 0; cc < childrenCount; cc++)
            {
                var control = VisualTreeHelper.GetChild(
                    root,
                    cc
                    );

                if (control is AccessText tb)
                {
                    if (tb.Text == text)
                    {
                        return tb;
                    }
                }

                var result = control.GetRecursiveTextBlockByNameItsText(text);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static FrameworkElement GetRecursiveByName(
            this DependencyObject root,
            string name
            )
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(root);

            for (var cc = 0; cc < childrenCount; cc++)
            {
                var control = VisualTreeHelper.GetChild(
                    root,
                    cc
                    );

                if (control is FrameworkElement fe)
                {
                    if (fe.Name == name)
                    {
                        return fe;
                    }
                }

                var result = control.GetRecursiveByName(name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
