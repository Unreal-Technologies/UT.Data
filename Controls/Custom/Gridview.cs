namespace UT.Data.Controls.Custom
{
    public class Gridview<Tdata> : Panel
        where Tdata : class
    {
        #region Constants
        private new const int Padding = 3;
        #endregion //Constants

        #region Enums
        public enum ControlLocations
        {
            Left, Right
        }
        #endregion //Enums

        #region Delegates
        public delegate void ClickHandler(Guid? id);
        #endregion //Delegates

        #region Events
        public ClickHandler? OnAdd { get; set; }
        public ClickHandler? OnEdit { get; set; }
        public ClickHandler? OnRemove { get; set; }
        #endregion //Events

        #region Members
        private readonly List<GvColumn> columns;
        private readonly List<Tuple<Point, Control?, Gridview.Alignment, int>> fields;
        private readonly List<Gridview.Row> rows;
        private Dictionary<int, int> columnSizes;
        private Dictionary<int, int> rowSizes;
        private readonly Button add;
        private IEnumerable<Tdata>? data;
        private readonly Func<Tdata, Guid> idValue;
        #endregion //Members

        #region Properties
        public ControlLocations ControlLocation { get; set; }
        #endregion //Properties

        #region Constructors
        public Gridview(Func<Tdata, Guid> idValue) : base()
        {
            this.idValue = idValue;

            columns = [];
            fields = [];
            columnSizes = [];
            rowSizes = [];

            rows = [];
            ControlLocation = ControlLocations.Left;
            MinimumSize = new Size(20, 20);

            add = new Button
            {
                Image = Resources.Plus,
                Size = Resources.Plus.Size,
                FlatStyle = FlatStyle.Flat
            };
            add.Click += delegate (object? sender, EventArgs e) { OnAdd?.Invoke(null); };
            add.FlatAppearance.BorderSize = 0;

            SetColumns([]);
        }
        #endregion //Constructors

        #region Public Methods
        public void Dataset(IEnumerable<Tdata> data)
        {
            this.data = data;
            Reload();
        }

        public GvColumn Column(string text, Func<Tdata, string> value)
        {
            return new GvColumn(text, value);
        }

        public GvColumn Column(string text, Func<Tdata, string> value, Gridview.Alignment alignment)
        {
            return new GvColumn(text, value, alignment);
        }

        public GvColumn Column(string text, Func<Tdata, string> value, Gridview.Alignment alignment, EventHandler eventHandler)
        {
            return new GvColumn(text, value, alignment, eventHandler);
        }

        public GvColumn Column(string text, Func<Tdata, string> value, EventHandler eventHandler)
        {
            return new GvColumn(text, value, eventHandler);
        }

        public void SetColumns(GvColumn[] columns)
        {
            this.columns.Clear();
            this.columns.AddRange(columns);
        }
        #endregion //Public Methods

        #region Private Methods
        private void Reload()
        {
            Clear();
            if (data == null)
            {
                return;
            }
            foreach (Tdata inputData in data)
            {
                Gridview.Row row = new()
                {
                    ID = idValue(inputData),
                    Edit = OnEdit != null,
                    Remove = OnRemove != null
                };

                foreach (GvColumn column in columns)
                {
                    Gridview.Cell cell = new()
                    {
                        Text = column.Value(inputData),
                        TextAlignment = column.Alignment
                    };

                    row.Cells.Add(cell);
                }

                AddRow(row);
            }
            Render();
        }

        private void AddRow(Gridview.Row row)
        {
            rows.Add(row);
        }

        private void Clear()
        {
            ClearControlsExceptHeader();
            rows.Clear();
        }

        private void Render()
        {
            ComposeHeader();
            ComposeRows();
        }

        private void ClearControlsExceptHeader()
        {
            foreach (Control? c in fields.Where(x => x.Item1.Y >= 1).Select(x => x.Item2))
            {
                Controls.Remove(c);
            }
            Tuple<Point, Control?, Gridview.Alignment, int>[] header = fields.Where(x => x.Item1.Y == 0).ToArray();
            fields.Clear();
            fields.AddRange(header);
        }

        private void ComposeRows()
        {
            int i = 1;
            int offset = ControlLocation == ControlLocations.Left ? 2 : 0;

            for (int r = 0; r < rows.Count; r++)
            {
                Gridview.Row row = rows[r];

                if (offset != 0)
                {
                    Button edit = new()
                    {
                        Image = Resources.Pencil,
                        Size = Resources.Pencil.Size,
                        Enabled = row.Edit == null || row.Edit.Value,
                        FlatStyle = FlatStyle.Flat
                    };
                    edit.Click += delegate (object? sender, EventArgs e) { OnEdit?.Invoke(row.ID); };
                    edit.FlatAppearance.BorderSize = 0;

                    Button remove = new()
                    {
                        Image = Resources.Delete,
                        Size = Resources.Delete.Size,
                        Enabled = row.Remove == null || row.Remove.Value,
                        FlatStyle = FlatStyle.Flat
                    };
                    remove.Click += delegate (object? sender, EventArgs e) { OnRemove?.Invoke(row.ID); };
                    remove.FlatAppearance.BorderSize = 0;

                    fields.Add(new Tuple<Point, Control?, Gridview.Alignment, int>(new Point(0, r + i), edit, Gridview.Alignment.Left, 1));
                    Controls.Add(edit);

                    fields.Add(new Tuple<Point, Control?, Gridview.Alignment, int>(new Point(1, r + i), remove, Gridview.Alignment.Left, 1));
                    Controls.Add(remove);
                }

                int length = row.Cells.Count;
                for (int c = 0; c < length; c++)
                {
                    Gridview.Cell cell = row.Cells[c];
                    int index = c + offset;

                    Label l = new()
                    {
                        Text = cell.Text
                    };
                    if (cell.FontStyle != null)
                    {
                        l.Font = new Font(Font, cell.FontStyle.Value);
                    }
                    if (cell.Color != null)
                    {
                        l.ForeColor = cell.Color.Value;
                    }
                    Gridview.Alignment alignment = Gridview.Alignment.Left;
                    if (cell.TextAlignment != null)
                    {
                        alignment = cell.TextAlignment.Value;
                    }

                    fields.Add(new Tuple<Point, Control?, Gridview.Alignment, int>(new Point(index, r + i), l, alignment, 1));
                    Controls.Add(l);
                }

                if (offset == 0)
                {
                    Button edit = new()
                    {
                        Image = Resources.Pencil,
                        Size = Resources.Pencil.Size,
                        Enabled = row.Edit == null || row.Edit.Value,
                        FlatStyle = FlatStyle.Flat
                    };
                    edit.Click += delegate (object? sender, EventArgs e) { OnEdit?.Invoke(row.ID); };
                    edit.FlatAppearance.BorderSize = 0;

                    Button remove = new()
                    {
                        Image = Resources.Delete,
                        Size = Resources.Delete.Size,
                        Enabled = row.Remove == null || row.Remove.Value,
                        FlatStyle = FlatStyle.Flat
                    };
                    remove.Click += delegate (object? sender, EventArgs e) { OnRemove?.Invoke(row.ID); };
                    remove.FlatAppearance.BorderSize = 0;

                    fields.Add(new Tuple<Point, Control?, Gridview.Alignment, int>(new Point(length + 0, r + i), edit, Gridview.Alignment.Left, 1));
                    Controls.Add(edit);

                    fields.Add(new Tuple<Point, Control?, Gridview.Alignment, int>(new Point(length + 1, r + i), remove, Gridview.Alignment.Left, 1));
                    Controls.Add(remove);
                }
            }

            ComposeSizes();
        }

        private void ComposeHeader()
        {
            add.Enabled = OnAdd != null;

            fields.Clear();
            Controls.Clear();
            Controls.Add(add);
            int offset = ControlLocation == ControlLocations.Left ? 2 : 0;
            if (offset != 0)
            {
                fields.Add(new Tuple<Point, Control?, Gridview.Alignment, int>(new Point(0, 0), add, Gridview.Alignment.Center, 2));
                fields.Add(new Tuple<Point, Control?, Gridview.Alignment, int>(new Point(1, 0), null, Gridview.Alignment.Left, 1));
            }

            int i = 0;
            foreach (GvColumn column in columns)
            {
                int index = i + offset;

                Label l = new()
                {
                    Text = column.Text,
                    Font = new Font(Font, FontStyle.Bold)
                };
                if (column.HasClickEvent)
                {
                    l.Click += (object? sender, EventArgs e) => { data = column.Invoke(data); Reload(); };
                    l.ForeColor = Color.Blue;
                }
                fields.Add(new Tuple<Point, Control?, Gridview.Alignment, int>(new Point(index, 0), l, Gridview.Alignment.Center, 1));
                Controls.Add(l);

                i++;
            }

            if (offset == 0)
            {
                fields.Add(new Tuple<Point, Control?, Gridview.Alignment, int>(new Point(i + 1, 0), add, Gridview.Alignment.Center, 2));
            }

            ComposeSizes();
        }

        private void ComposeSizes()
        {
            int maxY = fields.Select(x => x.Item1.Y).Max();
            int maxX = fields.Select(x => x.Item1.X).Max();

            Dictionary<int, int> columnsBuffer = [];
            Dictionary<int, int> rowsBuffer = [];

            for (int y = 0; y <= maxY; y++)
            {
                for (int x = 0; x <= maxX; x++)
                {
                    Point location = new(x, y);
                    Control? control = fields.Where(x => x.Item1.X == location.X && x.Item1.Y == location.Y).Select(x => x.Item2).FirstOrDefault();
                    int span = fields.Where(x => x.Item1.X == location.X && x.Item1.Y == location.Y).Select(x => x.Item4).FirstOrDefault();

                    Size size = new(0, 0);
                    if (control != null)
                    {
                        control.Size = control.PreferredSize;
                        size = control.Size;
                    }

                    for (int s = 0; s < span; s++)
                    {
                        if (s == 0)
                        {
                            if (columnsBuffer.TryGetValue(x + s, out int valueX))
                            {
                                columnsBuffer[x + s] = Math.Max(valueX, size.Width);
                            }
                            else
                            {
                                columnsBuffer.Add(x + s, size.Width);
                            }
                        }
                        else
                        {
                            if (columnsBuffer.TryGetValue(x, out int valueX))
                            {
                                columnsBuffer[x + s] = Math.Max(valueX, size.Width);
                            }
                            else
                            {
                                columnsBuffer.Add(x + s, size.Width);
                            }
                        }
                    }

                    if (rowsBuffer.TryGetValue(y, out int valueY))
                    {
                        rowsBuffer[y] = Math.Max(valueY, size.Height);
                    }
                    else
                    {
                        rowsBuffer.Add(y, size.Height);
                    }
                }
            }

            columnSizes = columnsBuffer;
            rowSizes = rowsBuffer;

            PositionControls();
        }

        private void PositionControls()
        {
            int padding = Padding;
            Dictionary<int, int> rowsBuffer = rowSizes;
            Dictionary<int, int> columnsBuffer = columnSizes;

            int maxY = rowsBuffer.Keys.Max();
            int maxX = columnsBuffer.Keys.Max();

            int offsetY = padding;
            for (int y = 0; y <= maxY; y++)
            {
                int offsetX = padding;
                for (int x = 0; x <= maxX; x++)
                {
                    Point index = new(x, y);
                    Size size = new(columnsBuffer[x], rowsBuffer[y]);

                    Tuple<Control?, Gridview.Alignment, int>? fieldData = fields.Where(x => x.Item1.X == index.X && x.Item1.Y == index.Y).Select(x => new Tuple<Control?, Gridview.Alignment, int>(x.Item2, x.Item3, x.Item4)).FirstOrDefault();
                    if (fieldData == null)
                    {
                        continue;
                    }

                    int span = fieldData.Item3;
                    if (span > 1)
                    {
                        for (int s = 1; s < span; s++)
                        {
                            int w = size.Width + columnsBuffer[x + s];
                            size = new Size(w, size.Height);
                        }
                    }

                    Control? control = fieldData.Item1;
                    Gridview.Alignment alignment = fieldData.Item2;

                    if (control != null)
                    {
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        control.Location = new Point(offsetX, offsetY + (size.Height - control.Height) / 2); //Default Left Align
                        if (alignment == Gridview.Alignment.Right)
                        {
                            int baseX = control.Location.X;
                            int newX = baseX + (size.Width - control.Width);
                            control.Location = new Point(newX, control.Location.Y);
                        }
                        else if (alignment == Gridview.Alignment.Center)
                        {
                            int baseX = control.Location.X;
                            int newX = baseX + (size.Width - control.Width) / 2;
                            control.Location = new Point(newX, control.Location.Y);
                        }
                    }
                    if (span > 1)
                    {
                        for (int s = 1; s < span; s++)
                        {
                            int w = size.Width - columnsBuffer[x + s];
                            size = new Size(w, size.Height);
                        }
                    }

                    offsetX += size.Width + padding;
                }
                offsetY += rowsBuffer.Values.Max() + padding;
            }
            Size = PreferredSize;
        }

        private void DrawHorizontalLines(Graphics g)
        {
            int padding = Padding;
            Pen p = Pens.Black;
            int w = columnSizes.Values.Sum() + columnSizes.Count * padding;

            for (int yI = 1; yI < rowSizes.Count; yI++)
            {
                int y = rowSizes.Values.Take(yI).Sum() + yI * padding + padding / 2;

                g.DrawLine(p, new PointF(0, y), new PointF(w, y));
            }
        }

        private void DrawVerticalLines(Graphics g)
        {
            int padding = Padding;
            Pen p = Pens.Black;
            int h = rowSizes.Values.Sum() + rowSizes.Count * padding;

            int offset = ControlLocation == ControlLocations.Left ? 2 : 0;
            int offsetInverse = 2 - offset;

            for (int xI = offset; xI < columnSizes.Count - offsetInverse; xI++)
            {
                int x = columnSizes.Values.Take(xI).Sum() + xI * padding + padding / 2;

                g.DrawLine(p, new PointF(x, 0), new PointF(x, h));
            }
        }
        #endregion //Private Methods

        #region Overrides
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            DrawHorizontalLines(g);
            DrawVerticalLines(g);
        }
        #endregion //Overrides

        public class GvEventArgs(IEnumerable<Tdata>? data) : EventArgs
        {
            #region Properties
            public IEnumerable<Tdata>? Data { get; set; } = data;
            #endregion //Properties
        }

        public class GvSorting
        {
            #region Constructors
            protected GvSorting() { }
            #endregion //Constructors

            #region Members
            private static string? last = null;
            private static int count = 0;
            #endregion //Members

            #region Protected Methods
            protected static (Gridview<Tdata>.GvEventArgs, Gridview<Tdata>.GvColumn, bool, int) Data(object? sender, EventArgs e)
            {
                if (e is Gridview<Tdata>.GvEventArgs ea && sender is Gridview<Tdata>.GvColumn column)
                {
                    bool isChanged = last != column.Text;
                    if (isChanged)
                    {
                        count = 0;
                    }
                    int direction = count % 2 == 0 ? 1 : -1;

                    count++;
                    last = column.Text;

                    return (ea, column, isChanged, direction);
                }
                return default;
            }
            #endregion //Protected Methods
        }

        public class GvColumn(string text, Func<Tdata, string> value)
        {
            #region Constructors
            public GvColumn(string text, Func<Tdata, string> value, Gridview.Alignment alignment) 
                : this(text, value)
            {
                Alignment = alignment;
            }

            public GvColumn(string text, Func<Tdata, string> value, Gridview.Alignment alignment, EventHandler eventHandler) 
                : this(text, value, eventHandler)
            {
                Alignment = alignment;
            }

            public GvColumn(string text, Func<Tdata, string> value, EventHandler eventHandler) 
                : this(text, value)
            {
                Click += eventHandler;
            }
            #endregion //Constructors

            #region Events
            public event EventHandler? Click;
            #endregion //Events

            #region Properties
            internal Func<Tdata, string> Value { get; set; } = value;
            public Gridview.Alignment Alignment { get; set; } = Gridview.Alignment.Left;
            public string? Text { get; set; } = text;
            public bool HasClickEvent { get { return Click != null; } }
            #endregion //Properties

            internal IEnumerable<Tdata>? Invoke(IEnumerable<Tdata>? data)
            {
                GvEventArgs gvEventArgs = new(data);

                Click?.Invoke(this, gvEventArgs);
                return gvEventArgs.Data;
            }
        }
    }

    public class Gridview
    {
        #region Enums
        public enum Alignment
        {
            Left, Right, Center
        }
        #endregion //Enums

        #region Classes
        public class Row
        {
            #region Properties
            public List<Cell> Cells { get; set; }
            public Guid? ID { get; set; }
            public bool? Remove { get; set; }
            public bool? Edit { get; set; }
            #endregion //Properties

            #region Constructors
            public Row()
            {
                Cells = [];
            }
            #endregion //Constructors
        }

        public class Cell
        {
            public Alignment? TextAlignment { get; set; }
            public string? Text { get; set; }
            public Color? Color { get; set; }
            public FontStyle? FontStyle { get; set; }
        }
        #endregion //Classes
    }
}
