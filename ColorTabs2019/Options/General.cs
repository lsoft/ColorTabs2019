using Community.VisualStudio.Toolkit;
using System.ComponentModel;

namespace ColorTabs2019
{
    internal partial class OptionsProvider
    {
        // Register the options with these attributes on your package class:
        public class GeneralOptions : BaseOptionPage<General> { }
    }

    public class General : BaseOptionModel<General>
    {
        [Category("General")]
        [DisplayName("Foreground")]
        [Description("Foreground color for tab name (ARGB in hex).")]
        [DefaultValue("FFFFFFFF")]
        public string Foreground { get; set; } = "FFFFFFFF";
    }
}
