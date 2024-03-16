using System.Diagnostics;
using System.Text;
using System.ComponentModel;
using System.Web;

namespace UT.Data.Controls.Custom
{
    public class CheckedComboBox : ComboBox
    {
        #region Members
        private readonly IContainer? components = null;
        private readonly Dropdown dropdown;
        private string valueSeparator;
        #endregion //Members

        #region Events
        public event ItemCheckEventHandler? ItemCheck;
        #endregion //Events

        #region Constructors
        public CheckedComboBox() : base()
        {
            DrawMode = DrawMode.OwnerDrawVariable;
            valueSeparator = ", ";
            DropDownHeight = 1;
            DropDownStyle = ComboBoxStyle.DropDown;
            dropdown = new Dropdown(this);
            CheckOnClick = true;
        }
        #endregion //Constructors

        #region Properties
        public string ValueSeparator
        {
            get { return valueSeparator; }
            set { valueSeparator = value; }
        }

        public bool CheckOnClick
        {
            get { return dropdown != null && dropdown.List != null && dropdown.List.CheckOnClick; }
            set { if (dropdown != null && dropdown.List != null) { dropdown.List.CheckOnClick = value; } }
        }

        public new string DisplayMember
        {
            get { return dropdown == null || dropdown.List == null ? string.Empty : dropdown.List.DisplayMember; }
            set { if (dropdown != null && dropdown.List != null) { dropdown.List.DisplayMember = value; } }
        }

        public new CheckedListBox.ObjectCollection? Items
        {
            get { return dropdown == null || dropdown.List == null ? null : dropdown.List.Items; }
        }

        public CheckedListBox.CheckedItemCollection? CheckedItems
        {
            get { return dropdown == null || dropdown.List == null ? null : dropdown.List.CheckedItems; }
        }

        public CheckedListBox.CheckedIndexCollection? CheckedIndices
        {
            get { return dropdown == null || dropdown.List == null ? null : dropdown.List.CheckedIndices; }
        }

        public bool ValueChanged
        {
            get { return dropdown != null && dropdown.ValueChanged; }
        }
        #endregion //Properties

        #region Public Methods
        public bool GetItemChecked(int index)
        {
            if (index < 0 || index > Items?.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "value out of range");
            }
            else
            {
                return dropdown.List?.GetItemChecked(index) ?? false;
            }
        }

        public void SetItemChecked(int index, bool isChecked)
        {
            if (index < 0 || index > Items?.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "value out of range");
            }
            else if (dropdown != null)
            {
                dropdown.List?.SetItemChecked(index, isChecked);
                Text = dropdown.GetCheckedItemsStringValue();
            }
        }

        public CheckState GetItemCheckState(int index)
        {
            if (index < 0 || index > Items?.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "value out of range");
            }
            else
            {
                return dropdown.List?.GetItemCheckState(index) ?? CheckState.Unchecked;
            }
        }

        public void SetItemCheckState(int index, CheckState state)
        {
            if (index < 0 || index > Items?.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "value out of range");
            }
            else
            {
                dropdown.List?.SetItemCheckState(index, state);
                Text = dropdown.GetCheckedItemsStringValue();
            }
        }
        #endregion //Public Methods

        #region Overrides
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnDropDown(EventArgs e)
        {
            base.OnDropDown(e);
            DoDropDown();
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            if (e is Dropdown.CCBoxEventArgs)
            {
                base.OnDropDownClosed(e);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                OnDropDown(EventArgs.Empty);
            }
            e.Handled = !e.Alt && !(e.KeyCode == Keys.Tab) && !(e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Home || e.KeyCode == Keys.End);

            base.OnKeyDown(e);
        }
        #endregion //Overrides

        #region Private Methods
        private void DoDropDown()
        {
            if (!dropdown.Visible)
            {
                Rectangle rect = RectangleToScreen(ClientRectangle);
                dropdown.Location = new Point(rect.X, rect.Y + Size.Height);
                int count = dropdown.List?.Items.Count ?? 0;
                if (count > MaxDropDownItems)
                {
                    count = MaxDropDownItems;
                }
                else if (count == 0)
                {
                    count = 1;
                }
                if (dropdown != null)
                {
                    dropdown.Size = new Size(Size.Width, (dropdown.List?.ItemHeight ?? 0) * count + 2);
                    dropdown.Show(this);
                }
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            e.Handled = true;
            base.OnKeyPress(e);
        }
        #endregion //Private Methods

        #region Classes
        internal class Dropdown : Form
        {
            #region Members
            private readonly CheckedComboBox ccbParent;
            private string oldStrValue = "";
            private bool[] checkedStateArr;
            private bool dropdownClosed = true;
            private CustomCheckedListBox? cclb;
            #endregion //Members

            #region Properties
            public bool ValueChanged
            {
                get
                {
                    string newStrValue = ccbParent.Text;
                    if (oldStrValue.Length > 0 && newStrValue.Length > 0)
                    {
                        return oldStrValue.CompareTo(newStrValue) != 0;
                    }
                    else
                    {
                        return oldStrValue.Length != newStrValue.Length;
                    }
                }
            }

            public CustomCheckedListBox? List
            {
                get { return cclb; }
                set { cclb = value; }
            }
            #endregion //Properties

            #region Constructors
            public Dropdown(CheckedComboBox ccbParent)
            {
                this.ccbParent = ccbParent;
                checkedStateArr = [];
                InitializeComponent();
                ShowInTaskbar = false;
                if (cclb != null)
                {
                    cclb.ItemCheck += new ItemCheckEventHandler(Cclb_ItemCheck);
                }
            }
            #endregion //Constructors

            #region Public Methods
            public string GetCheckedItemsStringValue()
            {
                StringBuilder sb = new("");
                if (cclb != null)
                {
                    for (int i = 0; i < cclb.CheckedItems.Count; i++)
                    {
                        sb.Append(cclb.GetItemText(cclb.CheckedItems[i])).Append(ccbParent.ValueSeparator);
                    }
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - ccbParent.ValueSeparator.Length, ccbParent.ValueSeparator.Length);
                }
                return sb.ToString();
            }

            public void CloseDropdown(bool enactChanges)
            {
                if (dropdownClosed)
                {
                    return;
                }
                if (enactChanges)
                {
                    ccbParent.SelectedIndex = -1;
                    ccbParent.Text = GetCheckedItemsStringValue();
                }
                else if (cclb != null)
                {
                    for (int i = 0; i < cclb.Items.Count; i++)
                    {
                        cclb.SetItemChecked(i, checkedStateArr[i]);
                    }
                }
                dropdownClosed = true;
                ccbParent.Focus();
                Hide();
                ccbParent.OnDropDownClosed(new CCBoxEventArgs(null, false));
            }
            #endregion //Public Methods

            #region Private Methods
            private void InitializeComponent()
            {
                cclb = new CustomCheckedListBox();
                SuspendLayout();
                // 
                // cclb
                // 
                cclb.BorderStyle = BorderStyle.None;
                cclb.Dock = DockStyle.Fill;
                cclb.FormattingEnabled = true;
                cclb.Location = new Point(0, 0);
                cclb.Name = "cclb";
                cclb.Size = new Size(47, 15);
                cclb.TabIndex = 0;
                // 
                // Dropdown
                // 
                AutoScaleDimensions = new SizeF(6F, 13F);
                AutoScaleMode = AutoScaleMode.Font;
                BackColor = SystemColors.Menu;
                ClientSize = new Size(47, 16);
                ControlBox = false;
                Controls.Add(cclb);
                ForeColor = SystemColors.ControlText;
                FormBorderStyle = FormBorderStyle.FixedToolWindow;
                MinimizeBox = false;
                Name = "ccbParent";
                StartPosition = FormStartPosition.Manual;
                ResumeLayout(false);
            }

            private void Cclb_ItemCheck(object? sender, ItemCheckEventArgs e)
            {
                if (ccbParent?.ItemCheck != null)
                {
                    ccbParent.ItemCheck(sender, e);
                }
            }
            #endregion //Private Methods

            #region Overrides
            protected override void OnActivated(EventArgs e)
            {
                base.OnActivated(e);
                dropdownClosed = false;
                oldStrValue = ccbParent.Text;
                if (cclb != null)
                {
                    checkedStateArr = new bool[cclb.Items.Count];
                    for (int i = 0; i < cclb.Items.Count; i++)
                    {
                        checkedStateArr[i] = cclb.GetItemChecked(i);
                    }
                }
            }

            protected override void OnDeactivate(EventArgs e)
            {
                base.OnDeactivate(e);
                if (e is CCBoxEventArgs ce)
                {
                    CloseDropdown(ce.AssignValues);

                }
                else
                {
                    CloseDropdown(true);
                }
            }
            #endregion //Overrides

            #region Classes
            internal class CCBoxEventArgs(EventArgs? e, bool assignValues) : EventArgs()
            {
                #region Members
                private bool assignValues = assignValues;
                private EventArgs? e = e;
                #endregion //Members

                #region Properties
                public bool AssignValues
                {
                    get { return assignValues; }
                    set { assignValues = value; }
                }

                public EventArgs? EventArgs
                {
                    get { return e; }
                    set { e = value; }
                }
                #endregion //Properties
            }

            internal class CustomCheckedListBox : CheckedListBox
            {
                #region Members
                private int curSelIndex = -1;
                #endregion //Members

                #region Constructors
                public CustomCheckedListBox() : base()
                {
                    SelectionMode = SelectionMode.One;
                    HorizontalScrollbar = true;
                }
                #endregion //Constructors

                #region Overrides
                protected override void OnKeyDown(KeyEventArgs e)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        ((Dropdown?)Parent)?.OnDeactivate(new CCBoxEventArgs(null, true));
                        e.Handled = true;

                    }
                    else if (e.KeyCode == Keys.Escape)
                    {
                        ((Dropdown?)Parent)?.OnDeactivate(new CCBoxEventArgs(null, false));
                        e.Handled = true;

                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        for (int i = 0; i < Items.Count; i++)
                        {
                            SetItemChecked(i, e.Shift);
                        }
                        e.Handled = true;
                    }
                    base.OnKeyDown(e);
                }

                protected override void OnMouseMove(MouseEventArgs e)
                {
                    base.OnMouseMove(e);
                    int index = IndexFromPoint(e.Location);
                    if (index >= 0 && index != curSelIndex)
                    {
                        curSelIndex = index;
                        SetSelected(index, true);
                    }
                }
                #endregion //Overrides

            }
            #endregion //Classes
        }
        #endregion //Classes
    }

}
