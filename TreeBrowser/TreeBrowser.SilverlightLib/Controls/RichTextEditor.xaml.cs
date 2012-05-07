using System;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Liquid;
using Liquid.Components.Internal;

namespace TreeBrowser.SilverlightLib.Controls
{
    public partial class RichTextEditor : UserControl
    {
        #region Private Properties

        private readonly SolidColorBrush _buttonFillStyleApplied = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        private readonly SolidColorBrush _buttonFillStyleNotApplied = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        private bool _ignoreFormattingChanges;

        #endregion

        public RichTextEditor()
        {
            InitializeComponent();

            SetupPatternMatching();
            //SetupStyles();

            richTextBox.ID = "root";
            richTextBox.SelectionChanged += RichTextBox_SelectionChanged;
            richTextBox.ShowContextMenu += richTextBox_ShowContextMenu;
            richTextBox.TextPatternMatch += richTextBox_TextPatternMatch;
            //richTextBox.StyleCreated += richTextBox_StyleCreated;
            //richTextBox.StyleDeleted += richTextBox_StyleDeleted;
            richTextBox.ElementWrite += richTextBox_ElementWrite;
            richTextBox.Diagnostic += richTextBox_Diagnostic;
        }

        private void richTextBox_Diagnostic(object sender, RichTextBoxEventArgs e)
        {
            var tb = new TextBox
                         {
                             TextWrapping = TextWrapping.Wrap,
                             AcceptsReturn = true,
                             Text = e.Parameter.ToString()
                         };
            diagnosticList.Children.Add(tb);
        }

        public string RichText
        {
            get { return richTextBox.RichText; }
            set
            {
                richTextBox.RichText = value;
            }
        }

        public string HTML
        {
            get { return richTextBox.HTML; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    richTextBox.HTML = value;
                else
                    richTextBox.HTML = "<p></p>";
            }
        }


        #region Custom UIElement Handling

        private void InsertTreeView()
        {
            string richText;

            richTextBox.CustomNamespaces.Add("xmlns:liquidTreeView=\"clr-namespace:Liquid;assembly=Liquid.TreeView\"");
            richText = "<Xaml>" + CreateTreeViewXAML() + "</Xaml>";

            richTextBox.Insert(richText);
        }

        private string CreateTreeViewXAML()
        {
            return "<liquidTreeView:Tree Width=\"150\" Height=\"150\" EnableLines=\"True\">" +
                   "<liquidTreeView:Node Title=\"Root\">" +
                   "<liquidTreeView:Node Title=\"Part 1\" />" +
                   "<liquidTreeView:Node Title=\"Part 2\" />" +
                   "<liquidTreeView:Node Title=\"Part 3\" />" +
                   "<liquidTreeView:Node Title=\"Part 4\" />" +
                   "</liquidTreeView:Node></liquidTreeView:Tree>";
        }

        private void richTextBox_ElementWrite(object sender, RichTextBoxEventArgs e)
        {
            Liquid.Tree tree;
            BitmapImage image;

            switch (e.Format)
            {
                case Format.XML:
                    // Specify the Liquid treeviews as XAML
                    if (e.Source is Liquid.Tree)
                    {
                        tree = (Liquid.Tree) e.Source;
                        e.Parameter = CreateTreeViewXAML();
                    }
                    break;
                case Format.Text:
                    // When a non-text element is converted to plain text you can specify here what it's text
                    // representation is (here we look for Emoticon images and set the appropriate text)
                    if (e.Source is Image)
                    {
                        image = (BitmapImage) ((Image) e.Source).Source;
                        switch (image.UriSource.ToString())
                        {
                            case "images/happy.png":
                                e.Content = ":)";
                                break;
                            case "images/unhappy.png":
                                e.Content = "):";
                                break;
                            case "images/wink.png":
                                e.Content = ";)";
                                break;
                        }
                    }
                    else if (e.Source is CheckBox)
                    {
                        e.Content = ((CheckBox) e.Source).IsChecked.Value.ToString();
                    }
                    break;
            }
        }

        #endregion

        #region Pattern Matching Handling

        private void SetupPatternMatching()
        {
            richTextBox.TextPatterns.Add(":)");
            richTextBox.TextPatterns.Add(";)");
            richTextBox.TextPatterns.Add(":(");
            richTextBox.TextPatterns.Add("(c)");
            richTextBox.TextPatterns.Add("(C)");
        }

        private void richTextBox_TextPatternMatch(object sender, RichTextBoxEventArgs e)
        {
            switch (e.Parameter.ToString())
            {
                case ":)":
                    e.Parameter = new Image {Source = new BitmapImage(new Uri("images/happy.png", UriKind.Relative))};
                    break;
                case ":(":
                    e.Parameter = new Image {Source = new BitmapImage(new Uri("images/unhappy.png", UriKind.Relative))};
                    break;
                case ";)":
                    e.Parameter = new Image {Source = new BitmapImage(new Uri("images/wink.png", UriKind.Relative))};
                    break;
                case "(c)":
                case "(C)":
                    e.Parameter = "©";
                    break;
            }
        }

        #endregion

        //#region Style Handling

        //private void SetupStyles()
        //{
        //    richTextBox.Styles.Add("H1",
        //                           new RichTextBoxStyle("H1", "Arial", 28, FontWeights.Bold)
        //                               {Margin = new Thickness(0, 12, 0, 3)});
        //    richTextBox.Styles.Add("H2",
        //                           new RichTextBoxStyle("H2", "Arial", 24, FontWeights.Bold)
        //                               {Margin = new Thickness(0, 12, 0, 3)});
        //    richTextBox.Styles.Add("H3",
        //                           new RichTextBoxStyle("H3", "Arial", 22, FontWeights.Bold)
        //                               {Margin = new Thickness(0, 12, 0, 3)});
        //    richTextBox.Styles.Add("Normal", new RichTextBoxStyle("Normal", "Arial", 14, FontWeights.Normal));

        //    foreach (string styleID in richTextBox.Styles.Keys)
        //    {
        //        AddStyle(styleID);
        //    }
        //}

        //private void AddStyle(string styleID)
        //{
        //    var textblock = new TextBlockPlus
        //                        {
        //                            Text = styleID
        //                        };
        //    textblock.Tag = styleID;
        //    //textblock.ApplyStyle(richTextBox.Styles[styleID]);
        //    selectStyle.Items.Add(textblock);
        //}

        //private void richTextBox_StyleCreated(object sender, RichTextBoxEventArgs e)
        //{
        //    AddStyle(((RichTextBoxStyle) e.Parameter).ID);
        //}

        //private void richTextBox_StyleDeleted(object sender, RichTextBoxEventArgs e)
        //{
        //    TextBlockPlus delete = GetStyleTextBlock(((RichTextBoxStyle) e.Parameter).ID);

        //    if (delete != null)
        //    {
        //        selectStyle.Items.Remove(delete);
        //    }
        //}

        //private TextBlockPlus GetStyleTextBlock(string styleID)
        //{
        //    TextBlockPlus result = null;

        //    foreach (TextBlockPlus textblock in selectStyle.Items)
        //    {
        //        if (textblock.Tag.ToString() == styleID)
        //        {
        //            result = textblock;
        //            break;
        //        }
        //    }

        //    return result;
        //}

        //#endregion

        #region Diagnostics

        private void RefreshSelectedStylesList()
        {
            TextBox tb;

            selectionStyleList.Children.Clear();

            foreach (StyleSelection style in richTextBox.SelectionStyles)
            {
                tb = new TextBox();

                tb.TextWrapping = TextWrapping.Wrap;
                tb.Text = "[" + style.Style.ID + "]-" + style.StartIndex + "/" + style.Length;
                selectionStyleList.Children.Add(tb);
            }
        }

        //private void RefreshHilightList()
        //{
        //    TextBox tb;

        //    hilightList.Children.Clear();

        //    foreach (RichTextHilight e in richTextBox.Hilights)
        //    {
        //        tb = new TextBox();

        //        tb.TextWrapping = TextWrapping.Wrap;
        //        tb.Text = "[" + e.Hilight.Start + "-" + e.Hilight.Length + "] " + e.Text + " [" + e.Element.Text + "]";
        //        hilightList.Children.Add(tb);
        //    }
        //}

        private void RefreshHistoryList()
        {
            TextBox tb;
            RichTextState before;
            RichTextState after;

            historyList.Children.Clear();

            foreach (HistoryEvent e in richTextBox.History.Events)
            {
                before = e.Parameter1 as RichTextState;
                after = e.Parameter2 as RichTextState;

                tb = new TextBox();

                tb.TextWrapping = TextWrapping.Wrap;
                tb.Text = e.Command + " (" + (before != null ? before.Text : "[No Before]") + " - " +
                          (after != null ? after.Text : "[No After]") + ")";
                historyList.Children.Add(tb);
            }
        }

        private void RefreshElementList()
        {
            elementList.Children.Clear();

            foreach (UIElement el in richTextBox.Panel.ContentChildren)
            {
                var tb = new TextBox
                             {
                                 TextWrapping = TextWrapping.Wrap
                             };

                if (el is TextBlockPlus)
                {
                    tb.Text = "[TB] [" + ((TextBlockPlus) el).Text + "] [" + ((TextBlockPlus) el).Hilights.Count + "]";
                }
                else
                {
                    tb.Text = "[OT] " + el;
                }

                if (((FrameworkElement) el).Tag != null)
                {
                    tb.Text += " {" + ((FrameworkElement) el).Tag + "}";
                }

                elementList.Children.Add(tb);
            }
        }

        private void RefreshVisualList()
        {
            int s;
            int e;

            visualList.Children.Clear();

            foreach (RichTextPanelRow row in richTextBox.Panel.Rows)
            {
                s = richTextBox.Panel.ContentChildren.IndexOf(row.Start);
                e = richTextBox.Panel.ContentChildren.IndexOf(row.End);

                var tb = new TextBox
                             {
                                 TextWrapping = TextWrapping.Wrap
                             };

                tb.Text = s + "-" + e + " : ";

                if (s >= 0 && e >= 0)
                {
                    for (int i = s; i <= e; i++)
                    {
                        UIElement el = richTextBox.Panel.ContentChildren[i];

                        tb.Text += "\n";

                        if (el is TextBlockPlus)
                        {
                            tb.Text += "[TB] [" + ((TextBlockPlus) el).Text + "] [" +
                                       ((TextBlockPlus) el).Hilights.Count + "]";
                        }
                        else
                        {
                            tb.Text += "[OT] " + el;
                        }
                    }
                }

                visualList.Children.Add(tb);
            }
        }

        #endregion

        #region Formatting Handling

        private void UpdateFormattingControls()
        {
            makeBold.Background = _buttonFillStyleNotApplied;
            makeItalic.Background = _buttonFillStyleNotApplied;
            makeUnderline.Background = _buttonFillStyleNotApplied;
            bulletList.Background = _buttonFillStyleNotApplied;
            numberList.Background = _buttonFillStyleNotApplied;
            makeLeft.Background = _buttonFillStyleNotApplied;
            makeCenter.Background = _buttonFillStyleNotApplied;
            makeRight.Background = _buttonFillStyleNotApplied;
            makeLink.Background = _buttonFillStyleNotApplied;
            makeSuperscript.Background = _buttonFillStyleNotApplied;
            makeSubscript.Background = _buttonFillStyleNotApplied;
            makeStrike.Background = _buttonFillStyleNotApplied;

            if (richTextBox.SelectionStyle != null)
            {
                if (richTextBox.SelectionStyle.Weight == FontWeights.Bold)
                {
                    makeBold.Background = _buttonFillStyleApplied;
                }

                if (richTextBox.SelectionStyle.Style == FontStyles.Italic)
                {
                    makeItalic.Background = _buttonFillStyleApplied;
                }

                if (richTextBox.SelectionStyle.Decorations == TextDecorations.Underline)
                {
                    makeUnderline.Background = _buttonFillStyleApplied;
                }

                if (richTextBox.SelectionAlignment == HorizontalAlignment.Left)
                {
                    makeLeft.Background = _buttonFillStyleApplied;
                }
                else if (richTextBox.SelectionAlignment == HorizontalAlignment.Center)
                {
                    makeCenter.Background = _buttonFillStyleApplied;
                }
                else if (richTextBox.SelectionAlignment == HorizontalAlignment.Right)
                {
                    makeRight.Background = _buttonFillStyleApplied;
                }

                if (richTextBox.SelectionListType != null)
                {
                    if (richTextBox.SelectionListType.Type == BulletType.Bullet)
                    {
                        bulletList.Background = _buttonFillStyleApplied;
                    }
                    else if (richTextBox.SelectionListType.Type == BulletType.Number)
                    {
                        numberList.Background = _buttonFillStyleApplied;
                    }
                }

                if (richTextBox.SelectionMetadata != null)
                {
                    if (richTextBox.SelectionMetadata.IsLink)
                    {
                        makeLink.Background = _buttonFillStyleApplied;
                    }
                }

                if (richTextBox.SelectionStyle.Effect == TextBlockPlusEffect.Strike)
                {
                    makeStrike.Background = _buttonFillStyleApplied;
                }

                if (richTextBox.SelectionStyle.Special == RichTextSpecialFormatting.Superscript)
                {
                    makeSuperscript.Background = _buttonFillStyleApplied;
                }

                if (richTextBox.SelectionStyle.Special == RichTextSpecialFormatting.Subscript)
                {
                    makeSubscript.Background = _buttonFillStyleApplied;
                }

                //SetSelected(selectFontFamily, richTextBox.SelectionStyle.Family);
                SetSelected(selectFontSize, richTextBox.SelectionStyle.Size.ToString());
                //SetSelectedByTag(selectShadow, richTextBox.SelectionStyle.Shadow.ToString());
                //selectColor.Selected = ((SolidColorBrush) richTextBox.SelectionStyle.Foreground).Color;
                //selectBackground.Selected = ((SolidColorBrush) richTextBox.SelectionStyle.Background).Color;
                //selectStyle.SelectedItem = GetStyleTextBlock(richTextBox.SelectionStyle.ID);

                //url.Text = richTextBox.SelectionStyle.Link;

                bool inTable = (richTextBox.ActiveTable != null);

                //insertTable.IsEnabled = !inTable;
                //editTable.IsEnabled = inTable;
                mainMenu.SetEnabledStatus("insertRow", inTable);
                mainMenu.SetEnabledStatus("insertColumn", inTable);
                mainMenu.SetEnabledStatus("deleteRow", inTable);
                mainMenu.SetEnabledStatus("deleteColumn", inTable);
                mainMenu.SetEnabledStatus("formatTable", inTable);
            }
        }

        private void SetSelected(System.Windows.Controls.ComboBox combo, string value)
        {
            if (value != null)
            {
                _ignoreFormattingChanges = true;

                foreach (ComboBoxItem item in combo.Items)
                {
                    if (item.Content.ToString().ToLower() == value.ToLower())
                    {
                        combo.SelectedItem = item;
                        break;
                    }
                }

                _ignoreFormattingChanges = false;
            }
        }

        //private void SetSelectedByTag(System.Windows.Controls.ComboBox combo, string value)
        //{
        //    if (value != null)
        //    {
        //        _ignoreFormattingChanges = true;

        //        foreach (ComboBoxItem item in combo.Items)
        //        {
        //            if (item.Tag.ToString().ToLower() == value.ToLower())
        //            {
        //                combo.SelectedItem = item;
        //                break;
        //            }
        //        }

        //        _ignoreFormattingChanges = false;
        //    }
        //}

        #endregion

        #region Event Handling

        private void richTextBox_ShowContextMenu(object sender, RichTextBoxEventArgs e)
        {
            //Visibility state = Visibility.Collapsed;

            //if (e.Source is Table && richTextBox.ActiveTable != null)
            //    state = Visibility.Visible;

            //contextMenu.Get("tableDivider").Visibility = state;
            //contextMenu.Get("tableDivider2").Visibility = state;
            //contextMenu.Get("formatTable").Visibility = state;
            //contextMenu.Get("insertRow").Visibility = state;
            //contextMenu.Get("insertColumn").Visibility = state;
            //contextMenu.Get("deleteRow").Visibility = state;
            //contextMenu.Get("deleteColumn").Visibility = state;

            //e.Cancel = false;
        }

        private void RichTextBox_SelectionChanged(object sender, RichTextBoxEventArgs e)
        {
            UpdateFormattingControls();
        }

        private void MakeBold_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (makeBold.Background == _buttonFillStyleNotApplied
                                     ? Formatting.Bold
                                     : Formatting.RemoveBold);

            ExecuteFormatting(format, null);
        }

        private void MakeItalic_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (makeItalic.Background == _buttonFillStyleNotApplied
                                     ? Formatting.Italic
                                     : Formatting.RemoveItalic);

            ExecuteFormatting(format, null);
        }

        private void MakeUnderline_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (makeUnderline.Background == _buttonFillStyleNotApplied
                                     ? Formatting.Underline
                                     : Formatting.RemoveUnderline);

            ExecuteFormatting(format, null);
        }

        private void MakeLeft_Click(object sender, RoutedEventArgs e)
        {
            ExecuteFormatting(Formatting.AlignLeft, null);
        }

        private void MakeCenter_Click(object sender, RoutedEventArgs e)
        {
            ExecuteFormatting(Formatting.AlignCenter, null);
        }

        private void MakeRight_Click(object sender, RoutedEventArgs e)
        {
            ExecuteFormatting(Formatting.AlignRight, null);
        }

        //private void SelectFontFamily_ItemSelected(object sender, System.EventArgs e)
        //{
        //    if (selectFontFamily != null)
        //    {
        //        ExecuteFormatting(Formatting.FontFamily,
        //                          ((ComboBoxItem) selectFontFamily.SelectedItem).FontFamily.Source);
        //    }
        //}

        private void SelectFontSize_ItemSelected(object sender, System.EventArgs e)
        {
            if (selectFontSize != null)
            {
                ExecuteFormatting(Formatting.FontSize,
                                  double.Parse(((ComboBoxItem) selectFontSize.SelectedItem).Content.ToString()));
            }
        }

        //private void SelectShadow_ItemSelected(object sender, System.EventArgs e)
        //{
        //    if (selectShadow != null)
        //    {
        //        switch (((ComboBoxItem) selectShadow.SelectedItem).Tag.ToString())
        //        {
        //            case "Slight":
        //                ExecuteFormatting(Formatting.ShadowSlight, "#44888888");
        //                break;
        //            case "Normal":
        //                ExecuteFormatting(Formatting.ShadowNormal, "#44888888");
        //                break;
        //            default:
        //                ExecuteFormatting(Formatting.RemoveShadow, null);
        //                break;
        //        }
        //    }
        //}

        //private void SelectColor_ItemSelected(object sender, System.EventArgs e)
        //{
        //    if (selectColor != null)
        //    {
        //        ExecuteFormatting(Formatting.Foreground, selectColor.Selected.ToString());
        //    }
        //}

        //private void SelectBackground_ItemSelected(object sender, System.EventArgs e)
        //{
        //    if (selectBackground != null)
        //    {
        //        ExecuteFormatting(Formatting.Background, selectBackground.Selected.ToString());
        //    }
        //}

        private void Indent_Click(object sender, RoutedEventArgs e)
        {
            ExecuteFormatting(Formatting.Indent, null);
        }

        private void Outdent_Click(object sender, RoutedEventArgs e)
        {
            ExecuteFormatting(Formatting.Outdent, null);
        }

        private void BulletList_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (bulletList.Background == _buttonFillStyleNotApplied
                                     ? Formatting.BulletList
                                     : Formatting.RemoveBullet);

            ExecuteFormatting(format, null);
        }

        private void NumberList_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (numberList.Background == _buttonFillStyleNotApplied
                                     ? Formatting.NumberList
                                     : Formatting.RemoveNumber);

            ExecuteFormatting(format, null);
        }

        //private void InsertImage_ItemSelected(object sender, System.EventArgs e)
        //{
        //    var item = (ComboBoxItem) insertImage.SelectedItem;
        //    var image = (Image) item.Content;
        //    richTextBox.Insert("<Xaml><Image Source=\"" + ((BitmapImage) image.Source).UriSource + "\" /></Xaml>");
        //    richTextBox.ReturnFocus();
        //}

        //private void Zoom_ItemSelected(object sender, System.EventArgs e)
        //{
        //    if (zoom != null)
        //    {
        //        var item = (ComboBoxItem) zoom.SelectedItem;

        //        richTextBox.Zoom = double.Parse(item.Content.ToString().Trim('%')) * 0.01;
        //        richTextBox.ReturnFocus();
        //    }
        //}

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            richTextBox.Cut();
            richTextBox.ReturnFocus();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            richTextBox.Copy();
            richTextBox.ReturnFocus();
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            richTextBox.Paste();
            richTextBox.ReturnFocus();
        }

        private void Painter_Click(object sender, RoutedEventArgs e)
        {
            richTextBox.Painter();
            richTextBox.ReturnFocus();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            richTextBox.Undo();
            richTextBox.ReturnFocus();
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            richTextBox.Redo();
            richTextBox.ReturnFocus();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            bool result = richTextBox.Find(searchTerms.Text);

            if (!result)
            {
                infoDialog.Show("The search term '" + searchTerms.Text + "' could not be found.", "Rich TextBox Search");
            }

            richTextBox.ReturnFocus();
        }

        private void Strike_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (makeStrike.Background == _buttonFillStyleNotApplied
                                     ? Formatting.Strike
                                     : Formatting.RemoveStrike);

            ExecuteFormatting(format, null);
        }

        private void Link_Click(object sender, RoutedEventArgs e)
        {
            enterURL.Show();
        }

        private void Superscript_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (makeSuperscript.Background == _buttonFillStyleNotApplied
                                     ? Formatting.SuperScript
                                     : Formatting.RemoveSpecial);

            ExecuteFormatting(format, null);
        }

        private void Subscript_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (makeSubscript.Background == _buttonFillStyleNotApplied
                                     ? Formatting.SubScript
                                     : Formatting.RemoveSpecial);

            ExecuteFormatting(format, null);
        }

        //private void InsertTable_Click(object sender, RoutedEventArgs e)
        //{
        //    tableRows.IsEnabled = true;
        //    tableColumns.IsEnabled = true;

        //    insertEditTableDialog.Title = "Insert Table";
        //    insertEditTableDialog.Buttons = DialogButtons.Close;
        //    insertEditTableDialog.CreateButton(DialogButtons.Custom, "Insert", "insert");
        //    insertEditTableDialog.ShowAsModal();
        //}

        //private void EditTable_Click(object sender, RoutedEventArgs e)
        //{
        //    foreach (ComboBoxItem item in tableStyle.Items)
        //    {
        //        if (item.Tag.ToString() == richTextBox.ActiveTable.StyleID)
        //        {
        //            tableStyle.SelectedItem = item;
        //            break;
        //        }
        //    }

        //    tableRows.Text = richTextBox.ActiveTable.RowDefinitions.Count.ToString();
        //    tableColumns.Text = richTextBox.ActiveTable.ColumnDefinitions.Count.ToString();
        //    tableRows.IsEnabled = false;
        //    tableColumns.IsEnabled = false;

        //    insertEditTableDialog.Title = "Edit Table";
        //    insertEditTableDialog.Buttons = DialogButtons.Apply | DialogButtons.Close;
        //    insertEditTableDialog.ShowAsModal();
        //}

        //private void insertEditTable_Closed(object sender, DialogEventArgs e)
        //{
        //    Table selectedTable = richTextBox.ActiveTable;
        //    int rows = 1;
        //    int columns = 1;
        //    string styleID;

        //    if (insertEditTableDialog.Result != DialogButtons.Close &&
        //        insertEditTableDialog.Result != DialogButtons.None)
        //    {
        //        styleID = tablePreview.Tag.ToString();
        //        int.TryParse(tableRows.Text, out rows);
        //        int.TryParse(tableColumns.Text, out columns);

        //        if (!richTextBox.TableStyles.ContainsKey(styleID))
        //        {
        //            // Create a new table style if not present
        //            richTextBox.TableStyles.Add(styleID, new RichTextBoxTableStyle(styleID, tablePreview.Background,
        //                                                                           tablePreview.CellFill,
        //                                                                           tablePreview.HeaderFill,
        //                                                                           tablePreview.BorderBrush,
        //                                                                           tablePreview.BorderThickness,
        //                                                                           tablePreview.CellBorderBrush,
        //                                                                           tablePreview.CellBorderThickness, 2));
        //        }

        //        switch (e.Tag.ToString())
        //        {
        //            case "insert":
        //                richTextBox.InsertTable(rows, columns, tablePreview.HeaderRows, tablePreview.HeaderColumns,
        //                                        styleID);
        //                break;
        //            case "apply":
        //                selectedTable.HeaderColumns = tablePreview.HeaderColumns;
        //                selectedTable.HeaderRows = tablePreview.HeaderRows;
        //                richTextBox.TableStyles[styleID].ApplyToTable(selectedTable);
        //                break;
        //        }
        //    }
        //}

        //private void ShowStyle_Click(object sender, RoutedEventArgs e)
        //{
        //    stylePopup.Show();
        //}

        //private void stylePopup_Closed(object sender, DialogEventArgs e)
        //{
        //    if (e.Tag.ToString() == "ok")
        //    {
        //        string styleID = ((TextBlockPlus) selectStyle.SelectedItem).Tag.ToString();

        //        if (richTextBox.SelectionStyle.ID != styleID)
        //        {
        //            ExecuteFormatting(Formatting.Style, styleID);
        //        }
        //    }
        //}

        //private void tableStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (tableStyle != null)
        //    {
        //        if (tableStyle.SelectedItem != null)
        //        {
        //            var borderItem = (Table) ((ComboBoxItem) tableStyle.SelectedItem).Content;

        //            if (borderItem != null)
        //            {
        //                tablePreview.Tag = ((ComboBoxItem) tableStyle.SelectedItem).Tag;
        //                tablePreview.Background = borderItem.Background;
        //                tablePreview.HeaderColumns = borderItem.HeaderColumns;
        //                tablePreview.HeaderRows = borderItem.HeaderRows;
        //                tablePreview.BorderBrush = borderItem.BorderBrush;
        //                tablePreview.BorderThickness = borderItem.BorderThickness;
        //                tablePreview.CellBorderBrush = borderItem.CellBorderBrush;
        //                tablePreview.CellBorderThickness = borderItem.CellBorderThickness;
        //                tablePreview.HeaderFill = borderItem.HeaderFill;
        //                tablePreview.CellFill = borderItem.CellFill;
        //            }
        //        }
        //    }
        //}

        private void RichTextBox_LinkClicked(object sender, RichTextBoxEventArgs e)
        {
            HtmlPage.Window.Navigate(new Uri(e.Parameter.ToString()), "_blank");
        }

        private void EnterURL_Closed(object sender, System.EventArgs e)
        {
            Formatting format = (makeLink.Background == _buttonFillStyleNotApplied
                                     ? Formatting.Link
                                     : Formatting.RemoveLink);
            string link = (format == Formatting.Link ? url.Text : "");

            if (!link.StartsWith("http://"))
            {
                link = "http://" + link;
            }

            if (enterURL.Result == DialogButtons.OK)
            {
                ExecuteFormatting(format, link);
            }
        }

        private void ExecuteFormatting(Formatting format, object param)
        {
            if (!_ignoreFormattingChanges)
            {
                richTextBox.ApplyFormatting(format, param);
                richTextBox.ReturnFocus();
            }
        }

        private void preview_Click(object sender, RoutedEventArgs e)
        {
            previewRichText.RichText = richTextBox.RichText;
            previewPopup.ShowAsModal();
        }

        #endregion

        #region Import/Export

        private void UpdateExportedText_Click(object sender, RoutedEventArgs e)
        {
            string conversion = string.Empty;

            switch (((ComboBoxItem) exportFormat.SelectedItem).Content.ToString().ToLower())
            {
                case "html":
                    conversion = richTextBox.HTML;
                    break;
                case "xml":
                    conversion = richTextBox.RichText;
                    break;
                case "xaml":
                    conversion = richTextBox.XAML;
                    break;
                case "plain text":
                    conversion = richTextBox.Save(Format.Text, RichTextSaveOptions.None);
                    break;
            }

            exportedText.Text = conversion;
        }

        private void UpdateImportedText_Click(object sender, RoutedEventArgs e)
        {
            string conversion = importText.Text;

            switch (((ComboBoxItem) importFormat.SelectedItem).Content.ToString().ToLower())
            {
                case "html":
                    richTextBox.HTML = conversion;
                    break;
                case "xml":
                    richTextBox.RichText = conversion;
                    break;
                case "plain text":
                    richTextBox.Load(Format.Text, conversion);
                    break;
            }
        }

        #endregion

        #region Menu Selection

        private void mainMenu_ItemSelected(object sender, MenuEventArgs e)
        {
            var item = (MenuItem) e.Parameter;

            if (e.Tag == null)
            {
                return;
            }

            switch (e.Tag.ToString())
            {
                case "new":
                    richTextBox.RichText = "";
                    break;
                case "save":
                    // TODO: Save functionality
                    break;
                case "preview":
                    previewRichText.RichText = richTextBox.RichText;
                    previewPopup.ShowAsModal();
                    break;
                case "export":
                    exportedText.Text = string.Empty;
                    exportPopup.ShowAsModal();
                    break;
                case "import":
                    importText.Text = string.Empty;
                    importPopup.ShowAsModal();
                    break;
                case "exit":
                    // TODO: Close functionality
                    break;
                case "cut":
                    richTextBox.Cut();
                    break;
                case "copy":
                    richTextBox.Copy();
                    break;
                case "paste":
                    richTextBox.Paste();
                    break;
                case "undo":
                    richTextBox.Undo();
                    break;
                case "redo":
                    richTextBox.Redo();
                    break;
                case "delete":
                    richTextBox.Delete(false);
                    break;
                case "selectAll":
                    richTextBox.SelectAll();
                    break;
                case "find":
                    findPopup.ShowAsModal();
                    break;
                case "h1":
                    richTextBox.ApplyFormatting(Formatting.Style, "H1");
                    break;
                case "h2":
                    richTextBox.ApplyFormatting(Formatting.Style, "H2");
                    break;
                case "h3":
                    richTextBox.ApplyFormatting(Formatting.Style, "H3");
                    break;
                case "normal":
                    richTextBox.ApplyFormatting(Formatting.Style, "Normal");
                    break;
                case "left":
                    richTextBox.ApplyFormatting(Formatting.AlignLeft, null);
                    break;
                case "center":
                    richTextBox.ApplyFormatting(Formatting.AlignCenter, null);
                    break;
                case "right":
                    richTextBox.ApplyFormatting(Formatting.AlignRight, null);
                    break;
                case "top":
                    richTextBox.ApplyFormatting(Formatting.AlignTop, null);
                    break;
                case "middle":
                    richTextBox.ApplyFormatting(Formatting.AlignMiddle, null);
                    break;
                case "bottom":
                    richTextBox.ApplyFormatting(Formatting.AlignBottom, null);
                    break;
                case "bold":
                    richTextBox.ApplyFormatting(Formatting.Bold, null);
                    break;
                case "italic":
                    richTextBox.ApplyFormatting(Formatting.Italic, null);
                    break;
                case "underline":
                    richTextBox.ApplyFormatting(Formatting.Underline, null);
                    break;
                case "subscript":
                    richTextBox.ApplyFormatting(Formatting.SubScript, null);
                    break;
                case "superscript":
                    richTextBox.ApplyFormatting(Formatting.SuperScript, null);
                    break;
                case "numberedList":
                    richTextBox.ApplyFormatting(Formatting.NumberList, null);
                    break;
                case "bulletedList":
                    richTextBox.ApplyFormatting(Formatting.BulletList, null);
                    break;
                case "bulletedImageList":
                    richTextBox.ApplyFormatting(Formatting.BulletImageList,
                                                new Uri("images/cloud.png", UriKind.Relative));
                    break;
                case "link":
                    enterURL.ShowAsModal();
                    break;
                case "insertTree":
                    InsertTreeView();
                    break;
                //case "insertTable":
                //    insertEditTableDialog.ShowAsModal();
                //    break;
                //case "editTable":
                //    insertEditTableDialog.ShowAsModal();
                //    break;
                case "insertRow":
                    richTextBox.InsertTableRow(true);
                    break;
                case "insertColumn":
                    richTextBox.InsertTableColumn(true);
                    break;
                case "deleteRow":
                    richTextBox.DeleteTableRow();
                    break;
                case "deleteColumn":
                    richTextBox.DeleteTableColumn();
                    break;
                //case "formatTable":
                //    EditTable_Click(this, null);
                //    break;
                case "readonly":
                    richTextBox.IsReadOnly ^= true;
                    item.Text = (richTextBox.IsReadOnly ? "Editable" : "Read-only");
                    break;
                case "elements":
                    RefreshElementList();
                    elementsPopup.ShowAsModal();
                    break;
                case "lines":
                    RefreshVisualList();
                    visualPopup.ShowAsModal();
                    break;
                case "history":
                    RefreshHistoryList();
                    historyPopup.ShowAsModal();
                    break;
                //case "hilights":
                //    RefreshHilightList();
                //    hilightPopup.ShowAsModal();
                //    break;
                case "selectedStyles":
                    RefreshSelectedStylesList();
                    selectionStylePopup.ShowAsModal();
                    break;
                case "diagnostic":
                    diagnosticPopup.Show();
                    break;
                default:
                    richTextBox.Zoom = double.Parse(e.Tag.ToString().Trim('%')) * 0.01;
                    break;
            }

            richTextBox.ReturnFocus();
        }

        private void contextMenu_ItemSelected(object sender, MenuEventArgs e)
        {
            var item = (MenuItem) e.Parameter;

            if (e.Tag == null)
            {
                return;
            }

            switch (e.Tag.ToString())
            {
                case "cut":
                    richTextBox.Cut();
                    break;
                case "copy":
                    richTextBox.Copy();
                    break;
                case "paste":
                    richTextBox.Paste();
                    break;
                //case "insertRow":
                //    richTextBox.InsertTableRow(true);
                //    break;
                //case "insertColumn":
                //    richTextBox.InsertTableColumn(true);
                //    break;
                //case "deleteRow":
                //    richTextBox.DeleteTableRow();
                //    break;
                //case "deleteColumn":
                //    richTextBox.DeleteTableColumn();
                //    break;
                //case "formatTable":
                //    EditTable_Click(this, null);
                //    break;
                default:
                    break;
            }

            richTextBox.ReturnFocus();
        }

        #endregion
    }
}