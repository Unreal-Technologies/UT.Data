using System.Diagnostics;
using System.Text;
using System.ComponentModel;
using System.Web;

namespace UT.Data.Controls
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
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.valueSeparator = ", ";
            this.DropDownHeight = 1;
            this.DropDownStyle = ComboBoxStyle.DropDown;
            this.dropdown = new Dropdown(this);
            this.CheckOnClick = true;
        }
        #endregion //Constructors

        #region Properties
        public string ValueSeparator
        {
            get { return this.valueSeparator; }
            set { this.valueSeparator = value; }
        }

        public bool CheckOnClick
        {
            get { return this.dropdown != null && this.dropdown.List != null && this.dropdown.List.CheckOnClick; }
            set { if (this.dropdown != null && this.dropdown.List != null) { this.dropdown.List.CheckOnClick = value; } }
        }

        public new string DisplayMember
        {
            get { return this.dropdown == null || this.dropdown.List == null ? string.Empty : this.dropdown.List.DisplayMember; }
            set { if (this.dropdown != null && this.dropdown.List != null) { this.dropdown.List.DisplayMember = value; } }
        }

        public new CheckedListBox.ObjectCollection? Items
        {
            get { return this.dropdown == null || this.dropdown.List == null ? null : dropdown.List.Items; }
        }

        public CheckedListBox.CheckedItemCollection? CheckedItems
        {
            get { return this.dropdown == null || this.dropdown.List == null ? null : dropdown.List.CheckedItems; }
        }

        public CheckedListBox.CheckedIndexCollection? CheckedIndices
        {
            get { return this.dropdown == null || this.dropdown.List == null ? null : dropdown.List.CheckedIndices; }
        }

        public bool ValueChanged
        {
            get { return this.dropdown != null && dropdown.ValueChanged; }
        }
        #endregion //Properties

        #region Public Methods
        public bool GetItemChecked(int index)
        {
            if (index < 0 || index > this.Items?.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "value out of range");
            }
            else
            {
                return this.dropdown.List?.GetItemChecked(index) ?? false;
            }
        }

        public void SetItemChecked(int index, bool isChecked)
        {
            if (index < 0 || index > this.Items?.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "value out of range");
            }
            else if (this.dropdown != null)
            {
                dropdown.List?.SetItemChecked(index, isChecked);
                this.Text = this.dropdown.GetCheckedItemsStringValue();
            }
        }

        public CheckState GetItemCheckState(int index)
        {
            if (index < 0 || index > this.Items?.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "value out of range");
            }
            else
            {
                return this.dropdown.List?.GetItemCheckState(index) ?? CheckState.Unchecked ;
            }
        }

        public void SetItemCheckState(int index, CheckState state)
        {
            if (index < 0 || index > this.Items?.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "value out of range");
            }
            else
            {
                this.dropdown.List?.SetItemCheckState(index, state);
                this.Text = this.dropdown.GetCheckedItemsStringValue();
            }
        }
        #endregion //Public Methods

        #region Overrides
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnDropDown(EventArgs e)
        {
            base.OnDropDown(e);
            this.DoDropDown();
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
                this.OnDropDown(EventArgs.Empty);
            }
            e.Handled = !e.Alt && !(e.KeyCode == Keys.Tab) && !((e.KeyCode == Keys.Left) || (e.KeyCode == Keys.Right) || (e.KeyCode == Keys.Home) || (e.KeyCode == Keys.End));

            base.OnKeyDown(e);
        }
        #endregion //Overrides

        #region Private Methods
        private void DoDropDown()
        {
            if (!this.dropdown.Visible)
            {
                Rectangle rect = RectangleToScreen(this.ClientRectangle);
                this.dropdown.Location = new Point(rect.X, rect.Y + this.Size.Height);
                int count = this.dropdown.List?.Items.Count ?? 0;
                if (count > this.MaxDropDownItems)
                {
                    count = this.MaxDropDownItems;
                }
                else if (count == 0)
                {
                    count = 1;
                }
                if (this.dropdown != null)
                {
                    this.dropdown.Size = new Size(this.Size.Width, (this.dropdown.List?.ItemHeight ?? 0) * count + 2);
                    this.dropdown.Show(this);
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
                    if ((oldStrValue.Length > 0) && (newStrValue.Length > 0))
                    {
                        return (oldStrValue.CompareTo(newStrValue) != 0);
                    }
                    else
                    {
                        return (oldStrValue.Length != newStrValue.Length);
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
                this.checkedStateArr = [];
                this.InitializeComponent();
                this.ShowInTaskbar = false;
                if (this.cclb != null)
                {
                    this.cclb.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.Cclb_ItemCheck);
                }
            }
            #endregion //Constructors

            #region Public Methods
            public string GetCheckedItemsStringValue()
            {
                StringBuilder sb = new("");
                if (this.cclb != null)
                {
                    for (int i = 0; i < this.cclb.CheckedItems.Count; i++)
                    {
                        sb.Append(this.cclb.GetItemText(this.cclb.CheckedItems[i])).Append(this.ccbParent.ValueSeparator);
                    }
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - this.ccbParent.ValueSeparator.Length, this.ccbParent.ValueSeparator.Length);
                }
                return sb.ToString();
            }

            public void CloseDropdown(bool enactChanges)
            {
                if (this.dropdownClosed)
                {
                    return;
                }
                if (enactChanges)
                {
                    this.ccbParent.SelectedIndex = -1;
                    this.ccbParent.Text = GetCheckedItemsStringValue();
                }
                else if(this.cclb != null)
                {
                    for (int i = 0; i < this.cclb.Items.Count; i++)
                    {
                        this.cclb.SetItemChecked(i, this.checkedStateArr[i]);
                    }
                }
                this.dropdownClosed = true;
                this.ccbParent.Focus();
                this.Hide();
                this.ccbParent.OnDropDownClosed(new CCBoxEventArgs(null, false));
            }
            #endregion //Public Methods

            #region Private Methods
            private void InitializeComponent()
            {
                this.cclb = new CustomCheckedListBox();
                this.SuspendLayout();
                // 
                // cclb
                // 
                this.cclb.BorderStyle = BorderStyle.None;
                this.cclb.Dock = DockStyle.Fill;
                this.cclb.FormattingEnabled = true;
                this.cclb.Location = new Point(0, 0);
                this.cclb.Name = "cclb";
                this.cclb.Size = new Size(47, 15);
                this.cclb.TabIndex = 0;
                // 
                // Dropdown
                // 
                this.AutoScaleDimensions = new SizeF(6F, 13F);
                this.AutoScaleMode = AutoScaleMode.Font;
                this.BackColor = SystemColors.Menu;
                this.ClientSize = new Size(47, 16);
                this.ControlBox = false;
                this.Controls.Add(this.cclb);
                this.ForeColor = SystemColors.ControlText;
                this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                this.MinimizeBox = false;
                this.Name = "ccbParent";
                this.StartPosition = FormStartPosition.Manual;
                this.ResumeLayout(false);
            }

            private void Cclb_ItemCheck(object? sender, ItemCheckEventArgs e)
            {
                if (this.ccbParent?.ItemCheck != null)
                {
                    this.ccbParent.ItemCheck(sender, e);
                }
            }
            #endregion //Private Methods

            #region Overrides
            protected override void OnActivated(EventArgs e)
            {
                base.OnActivated(e);
                this.dropdownClosed = false;
                this.oldStrValue = this.ccbParent.Text;
                if (this.cclb != null)
                {
                    this.checkedStateArr = new bool[this.cclb.Items.Count];
                    for (int i = 0; i < this.cclb.Items.Count; i++)
                    {
                        this.checkedStateArr[i] = this.cclb.GetItemChecked(i);
                    }
                }
            }

            protected override void OnDeactivate(EventArgs e)
            {
                base.OnDeactivate(e);
                if (e is CCBoxEventArgs ce)
                {
                    this.CloseDropdown(ce.AssignValues);

                }
                else
                {
                    this.CloseDropdown(true);
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
                    get { return this.assignValues; }
                    set { this.assignValues = value; }
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
                    this.SelectionMode = SelectionMode.One;
                    this.HorizontalScrollbar = true;                    
                }
                #endregion //Constructors

                #region Overrides
                protected override void OnKeyDown(KeyEventArgs e) 
                {
                    if (e.KeyCode == Keys.Enter) 
                    {
                        ((CheckedComboBox.Dropdown?)this.Parent)?.OnDeactivate(new CCBoxEventArgs(null, true));
                        e.Handled = true;

                    } else if (e.KeyCode == Keys.Escape) 
                    {
                        ((CheckedComboBox.Dropdown?)this.Parent)?.OnDeactivate(new CCBoxEventArgs(null, false));
                        e.Handled = true;

                    } else if (e.KeyCode == Keys.Delete)
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
                    if ((index >= 0) && (index != curSelIndex)) 
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
