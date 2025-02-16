﻿namespace MaterialSkin
{
    using MaterialSkin.Controls;
    using MaterialSkin.Properties;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Text;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class MaterialSkinManager
    {
        private static MaterialSkinManager _instance;

        private readonly List<MaterialForm> _formsToManage = new List<MaterialForm>();

        public delegate void SkinManagerEventHandler(object sender);
        public event SkinManagerEventHandler ColorSchemeChanged;
        public event SkinManagerEventHandler ThemeChanged;

        private Themes _theme;

        public Themes Theme
        {
            get { return _theme; }
            set
            {
                _theme = value;
                UpdateBackgrounds();
                ThemeChanged?.Invoke(this);
            }
        }

        private ColorScheme _colorScheme;

        public ColorScheme ColorScheme
        {
            get { return _colorScheme; }
            set
            {
                _colorScheme = value;
                UpdateBackgrounds();
                ColorSchemeChanged?.Invoke(this);
            }
        }

        public enum Themes : byte
        {
            LIGHT,
            DARK
        }

        private static readonly Color PRIMARY_TEXT_LIGHT = Color.FromArgb(220, 255, 255, 255);
        private static readonly Brush PRIMARY_TEXT_LIGHT_BRUSH = new SolidBrush(PRIMARY_TEXT_LIGHT);
        private static readonly Color PRIMARY_TEXT_DARK = Color.FromArgb(180, 0, 0, 0);
        private static readonly Brush PRIMARY_TEXT_DARK_BRUSH = new SolidBrush(PRIMARY_TEXT_DARK);
        private static readonly Color SECONDARY_TEXT_LIGHT = Color.FromArgb(179, 255, 255, 255);
        private static readonly Brush SECONDARY_TEXT_LIGHT_BRUSH = new SolidBrush(SECONDARY_TEXT_LIGHT);
        private static readonly Color SECONDARY_TEXT_DARK = Color.FromArgb(138, 0, 0, 0);
        private static readonly Brush SECONDARY_TEXT_DARK_BRUSH = new SolidBrush(SECONDARY_TEXT_DARK);
        private static readonly Color DISABLED_OR_HINT_TEXT_LIGHT = Color.FromArgb(30, 255, 255, 255);
        private static readonly Brush DISABLED_OR_HINT_TEXT_LIGHT_BRUSH = new SolidBrush(DISABLED_OR_HINT_TEXT_LIGHT);
        private static readonly Color DISABLED_OR_HINT_TEXT_DARK = Color.FromArgb(60, 0, 0, 0);
        private static readonly Brush DISABLED_OR_HINT_TEXT_DARK_BRUSH = new SolidBrush(DISABLED_OR_HINT_TEXT_DARK);

        private static readonly Color DIVIDERS_LIGHT = Color.FromArgb(31, 255, 255, 255);
        private static readonly Brush DIVIDERS_LIGHT_BRUSH = new SolidBrush(DIVIDERS_LIGHT);
        private static readonly Color DIVIDERS_DARK = Color.FromArgb(31, 0, 0, 0);
        private static readonly Brush DIVIDERS_DARK_BRUSH = new SolidBrush(DIVIDERS_DARK);

        private static readonly Color CHECKBOX_OFF_LIGHT = Color.FromArgb(138, 0, 0, 0);
        private static readonly Brush CHECKBOX_OFF_LIGHT_BRUSH = new SolidBrush(CHECKBOX_OFF_LIGHT);
        private static readonly Color CHECKBOX_OFF_DARK = Color.FromArgb(179, 255, 255, 255);
        private static readonly Brush CHECKBOX_OFF_DARK_BRUSH = new SolidBrush(CHECKBOX_OFF_DARK);
        private static readonly Color CHECKBOX_OFF_DISABLED_LIGHT = Color.FromArgb(66, 0, 0, 0);
        private static readonly Brush CHECKBOX_OFF_DISABLED_LIGHT_BRUSH = new SolidBrush(CHECKBOX_OFF_DISABLED_LIGHT);
        private static readonly Color CHECKBOX_OFF_DISABLED_DARK = Color.FromArgb(77, 255, 255, 255);
        private static readonly Brush CHECKBOX_OFF_DISABLED_DARK_BRUSH = new SolidBrush(CHECKBOX_OFF_DISABLED_DARK);

        private static readonly Color SWITCH_OFF_THUMB_LIGHT = Color.FromArgb(255, 255, 255, 255);
        private static readonly Color SWITCH_OFF_THUMB_DARK = Color.FromArgb(255, 190, 190, 190);
        private static readonly Color SWITCH_OFF_TRACK_LIGHT = Color.FromArgb(100, 0, 0, 0);
        private static readonly Color SWITCH_OFF_TRACK_DARK = Color.FromArgb(100, 255, 255, 255);
        private static readonly Color SWITCH_OFF_DISABLED_THUMB_LIGHT = Color.FromArgb(255, 230, 230, 230);
        private static readonly Color SWITCH_OFF_DISABLED_THUMB_DARK = Color.FromArgb(255, 150, 150, 150);
        private static readonly Color SWITCH_OFF_DISABLED_TRACK_LIGHT = Color.FromArgb(50, 0, 0, 0);
        private static readonly Color SWITCH_OFF_DISABLED_TRACK_DARK = Color.FromArgb(50, 255, 255, 255);

        private static readonly Color BUTTON_OUTLINE_LIGHT = Color.FromArgb(255, 150, 150, 150);
        private static readonly Color BUTTON_OUTLINE_DARK = Color.FromArgb(255, 200, 200, 200);
        private static readonly Color BUTTON_BACKGROUND_LIGHT = Color.FromArgb(255, 255, 255, 255);
        private static readonly Brush BUTTON_BACKGROUND_LIGHT_BRUSH = new SolidBrush(BUTTON_BACKGROUND_LIGHT);
        private static readonly Color BUTTON_BACKGROUND_DARK = Color.FromArgb(255, 85, 85, 85);
        private static readonly Brush BUTTON_BACKGROUND_DARK_BRUSH = new SolidBrush(BUTTON_BACKGROUND_DARK);
        private static readonly Color BUTTON_BACKGROUND_HOVER_LIGHT = Color.FromArgb(20.PercentageToColorComponent(), 0x999999.ToColor());
        private static readonly Brush BUTTON_BACKGROUND_HOVER_LIGHT_BRUSH = new SolidBrush(BUTTON_BACKGROUND_HOVER_LIGHT);
        private static readonly Color BUTTON_BACKGROUND_HOVER_DARK = Color.FromArgb(15.PercentageToColorComponent(), 0xCCCCCC.ToColor());
        private static readonly Brush BUTTON_BACKGROUND_HOVER_DARK_BRUSH = new SolidBrush(BUTTON_BACKGROUND_HOVER_DARK);
        private static readonly Color BUTTON_BACKGROUND_PRESSED_LIGHT = Color.FromArgb(40.PercentageToColorComponent(), 0x999999.ToColor());
        private static readonly Brush BUTTON_BACKGROUND_PRESSED_LIGHT_BRUSH = new SolidBrush(BUTTON_BACKGROUND_PRESSED_LIGHT);
        private static readonly Color BUTTON_BACKGROUND_PRESSED_DARK = Color.FromArgb(25.PercentageToColorComponent(), 0xCCCCCC.ToColor());
        private static readonly Brush BUTTON_BACKGROUND_PRESSED_DARK_BRUSH = new SolidBrush(BUTTON_BACKGROUND_PRESSED_DARK);
        private static readonly Color BUTTON_TEXT_LIGHT = PRIMARY_TEXT_LIGHT;
        private static readonly Brush BUTTON_TEXT_LIGHT_BRUSH = new SolidBrush(BUTTON_TEXT_LIGHT);
        private static readonly Color BUTTON_TEXT_DARK = PRIMARY_TEXT_DARK;
        private static readonly Brush BUTTON_TEXT_DARK_BRUSH = new SolidBrush(BUTTON_TEXT_DARK);
        private static readonly Color BUTTON_DISABLEDTEXT_LIGHT = Color.FromArgb(26.PercentageToColorComponent(), 0x000000.ToColor());
        private static readonly Brush BUTTON_DISABLEDTEXT_LIGHT_BRUSH = new SolidBrush(BUTTON_DISABLEDTEXT_LIGHT);
        private static readonly Color BUTTON_DISABLEDTEXT_DARK = Color.FromArgb(30.PercentageToColorComponent(), 0xFFFFFF.ToColor());
        private static readonly Brush BUTTON_DISABLEDTEXT_DARK_BRUSH = new SolidBrush(BUTTON_DISABLEDTEXT_DARK);

        private static readonly Color CMS_BACKGROUND_LIGHT_HOVER = Color.FromArgb(255, 238, 238, 238);
        private static readonly Brush CMS_BACKGROUND_HOVER_LIGHT_BRUSH = new SolidBrush(CMS_BACKGROUND_LIGHT_HOVER);
        private static readonly Color CMS_BACKGROUND_DARK_HOVER = Color.FromArgb(38, 204, 204, 204);
        private static readonly Brush CMS_BACKGROUND_HOVER_DARK_BRUSH = new SolidBrush(CMS_BACKGROUND_DARK_HOVER);

        private static readonly Color BACKGROUND_LIGHT = Color.FromArgb(255, 255, 255, 255);
        private static readonly Brush BACKGROUND_LIGHT_BRUSH = new SolidBrush(BACKGROUND_LIGHT);
        private static readonly Color BACKGROUND_DARK = Color.FromArgb(255, 51, 51, 51);
        private static readonly Brush BACKGROUND_DARK_BRUSH = new SolidBrush(BACKGROUND_DARK);

        public readonly Color ACTION_BAR_TEXT = Color.FromArgb(255, 255, 255, 255);
        public readonly Brush ACTION_BAR_TEXT_BRUSH = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
        public readonly Color ACTION_BAR_TEXT_SECONDARY = Color.FromArgb(153, 255, 255, 255);
        public readonly Brush ACTION_BAR_TEXT_SECONDARY_BRUSH = new SolidBrush(Color.FromArgb(153, 255, 255, 255));

        public Color GetSwitchOffThumbColor()
        {
            return Theme == Themes.LIGHT ? SWITCH_OFF_THUMB_LIGHT : SWITCH_OFF_THUMB_DARK;
        }
        public Color GetSwitchOffTrackColor()
        {
            return Theme == Themes.LIGHT ? SWITCH_OFF_TRACK_LIGHT : SWITCH_OFF_TRACK_DARK;
        }
        public Color GetSwitchOffDisabledThumbColor()
        {
            return Theme == Themes.LIGHT ? SWITCH_OFF_DISABLED_THUMB_LIGHT : SWITCH_OFF_DISABLED_THUMB_DARK;
        }
        public Color GetSwitchOffDisabledTrackColor()
        {
            return Theme == Themes.LIGHT ? SWITCH_OFF_DISABLED_TRACK_LIGHT : SWITCH_OFF_DISABLED_TRACK_DARK;
        }

        public Color GetPrimaryTextColor()
        {
            return Theme == Themes.LIGHT ? PRIMARY_TEXT_DARK : PRIMARY_TEXT_LIGHT;
        }

        public Brush GetPrimaryTextBrush()
        {
            return Theme == Themes.LIGHT ? PRIMARY_TEXT_DARK_BRUSH : PRIMARY_TEXT_LIGHT_BRUSH;
        }

        public Color GetSecondaryTextColor()
        {
            return Theme == Themes.LIGHT ? SECONDARY_TEXT_DARK : SECONDARY_TEXT_LIGHT;
        }

        public Brush GetSecondaryTextBrush()
        {
            return Theme == Themes.LIGHT ? SECONDARY_TEXT_DARK_BRUSH : SECONDARY_TEXT_LIGHT_BRUSH;
        }

        public Color GetDisabledOrHintColor()
        {
            return Theme == Themes.LIGHT ? DISABLED_OR_HINT_TEXT_DARK : DISABLED_OR_HINT_TEXT_LIGHT;
        }

        public Brush GetDisabledOrHintBrush()
        {
            return Theme == Themes.LIGHT ? DISABLED_OR_HINT_TEXT_DARK_BRUSH : DISABLED_OR_HINT_TEXT_LIGHT_BRUSH;
        }

        public Color GetDividersColor()
        {
            return Theme == Themes.LIGHT ? DIVIDERS_DARK : DIVIDERS_LIGHT;
        }

        public Brush GetDividersBrush()
        {
            return Theme == Themes.LIGHT ? DIVIDERS_DARK_BRUSH : DIVIDERS_LIGHT_BRUSH;
        }

        public Color GetCheckboxOffColor()
        {
            return Theme == Themes.LIGHT ? CHECKBOX_OFF_LIGHT : CHECKBOX_OFF_DARK;
        }

        public Color GetSwitchOffColor()
        {
            return Theme == Themes.LIGHT ? CHECKBOX_OFF_DARK : CHECKBOX_OFF_LIGHT;
        }

        public Brush GetCheckboxOffBrush()
        {
            return Theme == Themes.LIGHT ? CHECKBOX_OFF_LIGHT_BRUSH : CHECKBOX_OFF_DARK_BRUSH;
        }

        public Color GetCheckBoxOffDisabledColor()
        {
            return Theme == Themes.LIGHT ? CHECKBOX_OFF_DISABLED_LIGHT : CHECKBOX_OFF_DISABLED_DARK;
        }

        public Brush GetCheckBoxOffDisabledBrush()
        {
            return Theme == Themes.LIGHT ? CHECKBOX_OFF_DISABLED_LIGHT_BRUSH : CHECKBOX_OFF_DISABLED_DARK_BRUSH;
        }

        public Color GetButtonBackgroundColor()
        {
            return Theme == Themes.LIGHT ? BUTTON_BACKGROUND_LIGHT : BUTTON_BACKGROUND_DARK;
        }

        public Brush GetButtonBackgroundBrush()
        {
            return Theme == Themes.LIGHT ? BUTTON_BACKGROUND_LIGHT_BRUSH : BUTTON_BACKGROUND_DARK_BRUSH;
        }

        public Color GetButtonOutlineColor()
        {
            return Theme == Themes.LIGHT ? BUTTON_OUTLINE_LIGHT : BUTTON_OUTLINE_DARK;
        }

        public Color GetButtonDisabledOutlineColor()
        {
            return Theme == Themes.LIGHT ? BUTTON_OUTLINE_DARK : BUTTON_OUTLINE_LIGHT;
        }

        public Brush GetButtonTextBrush(bool primary)
        {
            return primary ? BUTTON_TEXT_LIGHT_BRUSH : BUTTON_TEXT_DARK_BRUSH;
        }

        public Color GetButtonHoverBackgroundColor()
        {
            return Theme == Themes.LIGHT ? BUTTON_BACKGROUND_HOVER_LIGHT : BUTTON_BACKGROUND_HOVER_DARK;
        }

        public Brush GetButtonHoverBackgroundBrush()
        {
            return Theme == Themes.LIGHT ? BUTTON_BACKGROUND_HOVER_LIGHT_BRUSH : BUTTON_BACKGROUND_HOVER_DARK_BRUSH;
        }

        public Color GetButtonPressedBackgroundColor()
        {
            return Theme == Themes.LIGHT ? BUTTON_BACKGROUND_PRESSED_LIGHT : BUTTON_BACKGROUND_PRESSED_DARK;
        }

        public Brush GetButtonPressedBackgroundBrush()
        {
            return Theme == Themes.LIGHT ? BUTTON_BACKGROUND_PRESSED_LIGHT_BRUSH : BUTTON_BACKGROUND_PRESSED_DARK_BRUSH;
        }

        public Brush GetButtonDisabledTextBrush()
        {
            return Theme == Themes.LIGHT ? BUTTON_DISABLEDTEXT_LIGHT_BRUSH : BUTTON_DISABLEDTEXT_DARK_BRUSH;
        }

        public Color GetButtonDisabledTextColor()
        {
            return Theme == Themes.LIGHT ? BUTTON_DISABLEDTEXT_LIGHT : BUTTON_DISABLEDTEXT_DARK;
        }

        public Brush GetCmsSelectedItemBrush()
        {
            return Theme == Themes.LIGHT ? CMS_BACKGROUND_HOVER_LIGHT_BRUSH : CMS_BACKGROUND_HOVER_DARK_BRUSH;
        }

        public Color GetApplicationBackgroundColor()
        {
            return Theme == Themes.LIGHT ? BACKGROUND_LIGHT.Darken((float)0.03) : BACKGROUND_DARK.Lighten((float)0.05);
        }

        public Color GetControlBackgroundColor()
        {
            return Theme == Themes.LIGHT ? BACKGROUND_LIGHT.Darken((float)0.07) : BACKGROUND_DARK.Lighten((float)0.15);
        }

        public int FORM_PADDING = 14;
        public static MaterialSkinManager Instance => _instance ?? (_instance = new MaterialSkinManager());

        public enum fontType
        {
            H1,
            H2,
            H3,
            H4,
            H5,
            H6,
            Subtitle1,
            Subtitle2,
            Body1,
            Body2,
            Button,
            Caption,
            Overline
        }

        public Font getFontByType(fontType type)
        {
            switch (type)
            {
                case fontType.H1:
                    return new Font(RobotoFontFamilies["Roboto_Light"], 96f, FontStyle.Regular, GraphicsUnit.Pixel);
                case fontType.H2:
                    return new Font(RobotoFontFamilies["Roboto_Light"], 60f, FontStyle.Regular, GraphicsUnit.Pixel);
                case fontType.H3:
                    return new Font(RobotoFontFamilies["Roboto"], 48f, FontStyle.Bold, GraphicsUnit.Pixel);
                case fontType.H4:
                    return new Font(RobotoFontFamilies["Roboto"], 34f, FontStyle.Bold, GraphicsUnit.Pixel);
                case fontType.H5:
                    return new Font(RobotoFontFamilies["Roboto"], 24f, FontStyle.Bold, GraphicsUnit.Pixel);
                case fontType.H6:
                    return new Font(RobotoFontFamilies["Roboto_Medium"], 20f, FontStyle.Bold, GraphicsUnit.Pixel);
                case fontType.Subtitle1:
                    return new Font(RobotoFontFamilies["Roboto"], 16f, FontStyle.Regular, GraphicsUnit.Pixel);
                case fontType.Subtitle2:
                    return new Font(RobotoFontFamilies["Roboto_Medium"], 14f, FontStyle.Bold, GraphicsUnit.Pixel);
                case fontType.Body1:
                    return new Font(RobotoFontFamilies["Roboto"], 14f, FontStyle.Regular, GraphicsUnit.Pixel);
                case fontType.Body2:
                    return new Font(RobotoFontFamilies["Roboto"], 12f, FontStyle.Regular, GraphicsUnit.Pixel);
                case fontType.Button:
                    return new Font(RobotoFontFamilies["Roboto"], 14f, FontStyle.Bold, GraphicsUnit.Pixel);
                case fontType.Caption:
                    return new Font(RobotoFontFamilies["Roboto"], 12f, FontStyle.Regular, GraphicsUnit.Pixel);
                case fontType.Overline:
                    return new Font(RobotoFontFamilies["Roboto"], 10f, FontStyle.Regular, GraphicsUnit.Pixel);
            }
            return new Font(RobotoFontFamilies["Roboto"], 14f, FontStyle.Regular, GraphicsUnit.Pixel);
        }


        /// <summary>
        /// Get the font by size - used for textbox label animation, try to not use this for anything else
        /// </summary>
        /// <param name="size">font size, ranges from 12 up to 16</param>
        /// <returns></returns>
        public IntPtr getTextBoxFontBySize(int size)
        {
            string name = "textBox" + Math.Min(16, Math.Max(12, size)).ToString();
            return logicalFonts[name];
        }

        /// <summary>
        /// Gets a Material Skin Logical Roboto Font given a standard material font type
        /// </summary>
        /// <param name="type">material design font type</param>
        /// <returns></returns>
        public IntPtr getLogFontByType(fontType type)
        {
            return logicalFonts[Enum.GetName(typeof(fontType), type)];
        }

        // Font stuff
        private Dictionary<string, IntPtr> logicalFonts;
        private Dictionary<string, FontFamily> RobotoFontFamilies;

        private MaterialSkinManager()
        {
            Theme = Themes.LIGHT;
            ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);

            // Create and cache Roboto fonts
            // Thanks https://www.codeproject.com/Articles/42041/How-to-Use-a-Font-Without-Installing-it
            // And https://www.codeproject.com/Articles/107376/Embedding-Font-To-Resources

            // Add font to system table in memory and save the font family
            addFont(Resources.Roboto_Thin);
            addFont(Resources.Roboto_Light);
            addFont(Resources.Roboto_Regular);
            addFont(Resources.Roboto_Medium);
            addFont(Resources.Roboto_Bold);
            addFont(Resources.Roboto_Black);

            RobotoFontFamilies = new Dictionary<string, FontFamily>();
            foreach (FontFamily ff in privateFontCollection.Families.ToArray())
            {
                RobotoFontFamilies.Add(ff.Name.Replace(' ', '_'), ff);
            }

            // create and save font handles for GDI
            logicalFonts = new Dictionary<string, IntPtr>(18);
            logicalFonts.Add("H1", createLogicalFont("Roboto Light", 96, NativeTextRenderer.logFontWeight.FW_LIGHT));
            logicalFonts.Add("H2", createLogicalFont("Roboto Light", 60, NativeTextRenderer.logFontWeight.FW_LIGHT));
            logicalFonts.Add("H3", createLogicalFont("Roboto", 48, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("H4", createLogicalFont("Roboto", 34, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("H5", createLogicalFont("Roboto", 24, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("H6", createLogicalFont("Roboto Medium", 20, NativeTextRenderer.logFontWeight.FW_MEDIUM));
            logicalFonts.Add("Subtitle1", createLogicalFont("Roboto", 16, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("Subtitle2", createLogicalFont("Roboto Medium", 14, NativeTextRenderer.logFontWeight.FW_MEDIUM));
            logicalFonts.Add("Body1", createLogicalFont("Roboto", 16, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("Body2", createLogicalFont("Roboto", 14, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("Button", createLogicalFont("Roboto Medium", 14, NativeTextRenderer.logFontWeight.FW_MEDIUM));
            logicalFonts.Add("Caption", createLogicalFont("Roboto", 12, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("Overline", createLogicalFont("Roboto", 10, NativeTextRenderer.logFontWeight.FW_REGULAR));
            // Logical fonts for textbox animation
            logicalFonts.Add("textBox16", createLogicalFont("Roboto", 16, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("textBox15", createLogicalFont("Roboto", 15, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("textBox14", createLogicalFont("Roboto", 14, NativeTextRenderer.logFontWeight.FW_REGULAR));
            logicalFonts.Add("textBox13", createLogicalFont("Roboto Medium", 13, NativeTextRenderer.logFontWeight.FW_MEDIUM));
            logicalFonts.Add("textBox12", createLogicalFont("Roboto Medium", 12, NativeTextRenderer.logFontWeight.FW_MEDIUM));
        }

        // Destructor
        ~MaterialSkinManager()
        {
            // RemoveFontMemResourceEx
            foreach (IntPtr handle in logicalFonts.Values)
            {
                NativeTextRenderer.DeleteObject(handle);
            }
        }

        private PrivateFontCollection privateFontCollection = new PrivateFontCollection();

        private void addFont(byte[] fontdata)
        {
            // Add font to system table in memory
            int dataLength = fontdata.Length;

            IntPtr ptrFont = Marshal.AllocCoTaskMem(dataLength);
            Marshal.Copy(fontdata, 0, ptrFont, dataLength);

            NativeTextRenderer.AddFontMemResourceEx(fontdata, dataLength, IntPtr.Zero, out _);

            privateFontCollection.AddMemoryFont(ptrFont, dataLength);
        }

        private IntPtr createLogicalFont(string fontName, int size, NativeTextRenderer.logFontWeight weight)
        {
            // Logical font:
            NativeTextRenderer.LogFont lfont = new NativeTextRenderer.LogFont();
            lfont.lfFaceName = fontName;
            lfont.lfHeight = -size;
            lfont.lfWeight = (int)weight;
            return NativeTextRenderer.CreateFontIndirect(lfont);
        }

        public void AddFormToManage(MaterialForm materialForm)
        {
            _formsToManage.Add(materialForm);
            UpdateBackgrounds();
        }

        public void RemoveFormToManage(MaterialForm materialForm)
        {
            _formsToManage.Remove(materialForm);
        }

        private void UpdateBackgrounds()
        {
            var newBackColor = GetApplicationBackgroundColor();
            foreach (var materialForm in _formsToManage)
            {
                materialForm.BackColor = newBackColor;
                UpdateControlBackColor(materialForm, newBackColor);
            }
        }

        private void UpdateToolStrip(ToolStrip toolStrip, Color newBackColor)
        {
            if (toolStrip == null)
            {
                return;
            }

            toolStrip.BackColor = newBackColor;
            foreach (ToolStripItem control in toolStrip.Items)
            {
                control.BackColor = newBackColor;
                if (control is MaterialToolStripMenuItem && (control as MaterialToolStripMenuItem).HasDropDown)
                {

                    //recursive call
                    UpdateToolStrip((control as MaterialToolStripMenuItem).DropDown, newBackColor);
                }
            }
        }

        private void UpdateControlBackColor(Control controlToUpdate, Color newBackColor)
        {
            // No control
            if (controlToUpdate == null) return;

            // Control's Context menu
            if (controlToUpdate.ContextMenuStrip != null) UpdateToolStrip(controlToUpdate.ContextMenuStrip, newBackColor);

            // Material Tabcontrol pages
            if (controlToUpdate is TabPage)
            {
                ((TabPage)controlToUpdate).BackColor = newBackColor;
            }

            // Material Divider
            else if (controlToUpdate is MaterialDivider)
            {
                controlToUpdate.BackColor = GetDividersColor();
            }

            // Other Material Skin control
            else if (controlToUpdate.IsMaterialControl())
            {
                controlToUpdate.BackColor = newBackColor;
                controlToUpdate.ForeColor = GetPrimaryTextColor();
            }

            // Other Generic control not part of material skin
            else if (controlToUpdate.HasProperty("BackColor") && !controlToUpdate.IsMaterialControl() && controlToUpdate.Parent != null)
            {
                controlToUpdate.BackColor = controlToUpdate.Parent.BackColor;
                if (controlToUpdate.HasProperty("ForeColor"))
                {
                    controlToUpdate.ForeColor = GetPrimaryTextColor();
                    controlToUpdate.Font = getFontByType(MaterialSkinManager.fontType.Body1);
                }
            }

            // Recursive call to control's children
            foreach (Control control in controlToUpdate.Controls)
            {
                UpdateControlBackColor(control, newBackColor);
            }
        }
    }


    public static class StringExtension
    {
        public static string ToSecureString(this string plainString)
        {
            if (plainString == null)
                return null;

            string secureString = "";
            foreach (char c in plainString.ToCharArray())
            {
                secureString += '\u25CF';
            }
            return secureString;
        }
    }
}
