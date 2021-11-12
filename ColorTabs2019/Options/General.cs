using System.ComponentModel;
using Community.VisualStudio.Toolkit;

namespace ColorTabs2019.Options
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

        [Category("General")]
        [DisplayName("Enabled")]
        [Description("Disable this extension if it harms you IDE")]
        [DefaultValue(true)]
        public bool Enabled { get; set; } = true;

        [Category("General")]
        [DisplayName("Secondary color")]
        [Description("Control to show or not to show a secondary (folder) color rectangle")]
        [DefaultValue(true)]
        public bool SecondaryColorEnabled { get; set; } = true;

    }
}
