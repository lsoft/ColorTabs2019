using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ColorTabs2019.Options;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace ColorTabs2019
{
    public class Backgrounder
    {
        private readonly CancellationTokenSource _cts;

        private readonly Events _events;
        private readonly DTEEvents _dteEvents;
        private readonly SolutionEvents _solutionEvents;
        private readonly ColorProvider _colorProvider;

        private readonly ConcurrentDictionary<TabItem, object> _headerDict = new();

        public Backgrounder(DTE2 dte)
        {
            if (dte is null)
            {
                throw new ArgumentNullException(nameof(dte));
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            _colorProvider = new ColorProvider();
            _cts = new CancellationTokenSource();

            _events = dte.Events;
            _dteEvents = _events.DTEEvents;
            _solutionEvents = _events.SolutionEvents;

            _dteEvents.OnBeginShutdown += () => _cts.Cancel();
            _solutionEvents.AfterClosing += () => _colorProvider.Reset();
        }

        public async Task ScanAsync()
        {
            try
            {
                const int DefaultWaitTimeout = 1000;
                const int IncreaseWaitTimeout = 1000;

                var waitTimeout = DefaultWaitTimeout;
                var mw = Application.Current.MainWindow;

                var disabled = false;

                //search for tab list panel
                while (true)
                {
                    try
                    {
                        await Task.Delay(waitTimeout, _cts.Token);

                        if (_cts.Token.IsCancellationRequested)
                        {
                            return;
                        }

                        if (!General.Instance.Enabled)
                        {
                            disabled = true;
                            continue;
                        }

                        //if (disabled)
                        //{
                        //    _headerDict.Clear();
                        //    disabled = false;
                        //}

                        var tabListHost = mw.GetRecursiveByName("PART_TabListHost");
                        if (tabListHost == null)
                        {
                            continue;
                        }

                        var tabs = new List<FrameworkElement>();
                        tabListHost.GetRecursiveByType("DocumentTabItem", ref tabs);

                        if (tabs.Count <= 0)
                        {
                            continue;
                        }

                        foreach (var tab in tabs)
                        {
                            var tabItem = tab as TabItem;
                            if (tabItem == null)
                            {
                                continue;
                            }

                            if (tabItem.HeaderTemplate == null)
                            {
                                //this tab already processed

                                //check for its title updated
                                if (_headerDict.TryGetValue(tabItem, out var header))
                                {
                                    var headerTextBlock = ((tabItem.Header as StackPanel).Children[2] as TextBlock);
                                    if (headerTextBlock != null)
                                    {
                                        var ourTitle = headerTextBlock.Text;
                                        var originalHeader = GetTabTile(header);
                                        if (ourTitle != originalHeader)
                                        {
                                            headerTextBlock.Text = originalHeader;
                                        }
                                    }
                                }

                                continue;
                            }

                            if (!_headerDict.TryGetValue(tabItem, out _))
                            {
                                _headerDict[tabItem] = tabItem.Header;
                            }

                            tabItem.HeaderTemplate = null;
                            tabItem.Header = new StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                Children =
                                {
                                    new Rectangle
                                    {
                                        HorizontalAlignment = HorizontalAlignment.Left,
                                        VerticalAlignment = VerticalAlignment.Stretch,
                                        Fill = new SolidColorBrush(_colorProvider.DetermineColor(tabItem).Item1),
                                        Height = 10,
                                        Width = 10,
                                        Margin = new Thickness(0)
                                    },
                                    new Rectangle
                                    {
                                        HorizontalAlignment = HorizontalAlignment.Left,
                                        VerticalAlignment = VerticalAlignment.Stretch,
                                        Fill = new SolidColorBrush(_colorProvider.DetermineColor(tabItem).Item2),
                                        Height = 10,
                                        Width = 5,
                                        Margin = new Thickness(0)
                                    },
                                    new TextBlock
                                    {
                                        Padding = new Thickness(0),
                                        Margin = new Thickness(5, 0, 0, 0),
                                        Foreground = new SolidColorBrush(
                                            uint.Parse(General.Instance.Foreground, NumberStyles.HexNumber).ToColorFromArgb()
                                            ),
                                        Text = GetTabTile(tabItem.Header)
                                    }
                                }
                            };

                            //tabItem.Header = "123";

                            //Style style = new Style(typeof(TabItem));//, tabItem.Style);
                            //style.Setters.Add(new Setter(TabItem.BackgroundProperty, Brushes.Blue));
                            ////style.Setters.Add(tabItem.Style.Setters[1]);
                            //style.Triggers.Add(
                            //    new Trigger
                            //    {
                            //        Property = TabItem.IsMouseDirectlyOverProperty,
                            //        Value = true,
                            //        Setters =
                            //        {
                            //            new Setter(TabItem.BackgroundProperty, Brushes.Green)
                            //        }
                            //    });
                            //tabItem.Style = style;

                            //var grid = tabItem.GetRecursiveByName("TitlePanel") as Grid;
                            //if(grid != null)
                            ////var border = tabItem.GetRecursiveByName("OuterBorder") as Border;
                            ////if (border != null)
                            //{
                            //    string mcontent = (tabItem.Content as dynamic)?.Name?.ToString() ?? string.Empty;
                            //    if (!string.IsNullOrEmpty(mcontent))
                            //    {
                            //        var parts = mcontent.Split('|');
                            //        if (parts.Length == 5)
                            //        {
                            //            var csprojPath = parts[1];

                            //            var hashcode = csprojPath.GetHashCode(); //tabItem.Content.ToString().GetHashCode();
                            //            var a = (byte)127;
                            //            var r = (byte)((hashcode & 0x00ff0000) >> 16);
                            //            var g = (byte)((hashcode & 0x0000ff00) >> 8);
                            //            var b = (byte)((hashcode & 0x00ff00ff) >> 0);
                            //            //var color = Color.FromArgb(a, r, g, b);
                            //            var color = 0x4fa24f.GetColor();
                            //            //if (!tabItem.IsFocused)
                            //            {
                            //                grid.Background = new SolidColorBrush(color);
                            //            }
                            //        }
                            //    }
                            //}
                        }

                        //restore timeout if case we're successful
                        waitTimeout = DefaultWaitTimeout;
                    }
                    catch (Exception excp)
                    {
                        Logging.LogVS(excp.Message);
                        Logging.LogVS(excp.StackTrace);

                        //increase timeout to prevent spam into the log
                        waitTimeout += IncreaseWaitTimeout;
                    }
                }
            }
            catch (Exception excp)
            {
                Logging.LogVS("STOP ERROR");
                Logging.LogVS(excp.Message);
                Logging.LogVS(excp.StackTrace);
            }
        }

        private static dynamic GetTabTile(dynamic tabItemHeader)
        {
            return tabItemHeader?.Title?.AnnotatedTitle?.ToString() ?? "<< unknown >>";
        }

    }

    public class ColorProvider
    {
        private static readonly int[] _colors = new[]
        {
            0x4fa24f,
            0x2fa1bb,
            0xcf7377,
            0xab8366,
            0x457daa,
            0xc36c3c,
            0x8867ac,
            0xc65199,
            0xa995c0,
            0xb66467,
            0x289296,
            0x886254,
            0x639229,
            0x3288ca,
            0x8867ac,
            0xbda230,
            0x2fa1bb,
        };

        private readonly ConcurrentDictionary<string, Color> _dict = new();

        public (Color, Color) DetermineColor(TabItem tabItem)
        {
            var grid = tabItem.GetRecursiveByName("TitlePanel") as Grid;
            if (grid != null)
            //var border = tabItem.GetRecursiveByName("OuterBorder") as Border;
            //if (border != null)
            {
                string mcontent = (tabItem.Content as dynamic)?.Name?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(mcontent))
                {
                    var parts = mcontent.Split('|');
                    if (parts.Length == 5)
                    {
                        var csprojPath = parts[1];
                        var filePath = parts[2];

                        Color folderColor = Colors.Transparent;
                        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                        {
                            var folderPath = new FileInfo(filePath).Directory?.FullName;
                            if (!string.IsNullOrEmpty(folderPath))
                            {
                                folderColor = GetRandomColor(folderPath);
                            }
                        }

                        if (_dict.TryGetValue(csprojPath, out var projectColor))
                        {
                            return (projectColor, folderColor);
                        }

                        if (_dict.Count < _colors.Length)
                        {
                            projectColor = _colors[_dict.Count].ToColorFromRgb();
                        }
                        else
                        {
                            //enable randomizer
                            projectColor = GetRandomColor(csprojPath);
                        }

                        _dict[csprojPath] = projectColor;
                        return (projectColor, folderColor);
                    }
                }
            }

            return (Colors.Transparent, Colors.Transparent);
        }

        private static Color GetRandomColor(
            string s
            )
        {
            using (var md5 = MD5.Create())
            {
                md5.Initialize();
                md5.ComputeHash(Encoding.UTF8.GetBytes(s));
                var hashcode = md5.Hash;

                var a = (byte)255;
                var r = hashcode[0];
                var g = hashcode[1];
                var b = hashcode[2];
                var color = Color.FromArgb(a, r, g, b);

                return color;
            }
        }

        public void Reset()
        {
            _dict.Clear();
        }
    }

    public static class ColorHelper
    {
        public static Color ToColorFromRgb(
            this int rgb
            )
        {
            var bytes = BitConverter.GetBytes(rgb);

            return Color.FromArgb(255, bytes[2], bytes[1], bytes[0]);
        }
    }

}
