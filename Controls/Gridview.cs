using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace UT.Data.Controls
{
    public class Gridview<Tid> : Panel
        where Tid : struct
    {
        #region Constants
        private new const int Padding = 3;
        #endregion //Constants

        #region Enums
        public enum ControlLocations
        {
            Left, Right
        }

        public enum Alignment
        {
            Left, Right, Center
        }
        #endregion //Enums

        #region Delegates
        public delegate void ClickHandler(Tid? id);
        #endregion //Delegates

        #region Events
        public ClickHandler? OnAdd;
        public ClickHandler? OnEdit;
        public ClickHandler? OnRemove;
        #endregion //Events

        #region Members
        private readonly List<Column> columns;
        private readonly List<Tuple<Point, Control?, Alignment>> fields;
        private readonly List<Row> rows;
        private Dictionary<int, int> columnSizes;
        private Dictionary<int, int> rowSizes;
        private readonly Button add;
        #endregion //Members

        #region Properties
        public ControlLocations ControlLocation { get; set; }
        #endregion //Properties

        #region Constructors
        public Gridview(): base()
        {
            this.columns = [];
            this.fields = [];
            this.columnSizes = [];
            this.rowSizes = [];

            this.rows = [];
            this.ControlLocation = ControlLocations.Left;
            this.MinimumSize = new Size(20, 20);

            this.add = new Button
            {
                Image = Resources.Plus,
                Size = Resources.Plus.Size
            };
            this.add.Click += delegate (object? sender, EventArgs e) { this.OnAdd?.Invoke(null); };
        }
        #endregion //Constructors

        #region Public Methods
        public void AddRow(Row row)
        {
            this.rows.Add(row);
            this.ComposeRows();
        }

        public void SetRows(Row[] rows)
        {
            this.rows.Clear();
            this.rows.AddRange(rows);
            this.ComposeRows();
        }

        public void SetColumns(Column[] columns)
        {
            this.columns.Clear();
            this.columns.AddRange(columns);
            this.ComposeHeader();
        }

        public void AddColumn(Column column)
        {
            this.columns.Add(column);
            this.ComposeHeader();
        }
        #endregion //Public Methods

        #region Private Methods
        private void ComposeRows()
        {
            int i = 1;
            int offset = this.ControlLocation == ControlLocations.Left ? 2 : 0;

            for(int r = 0; r<this.rows.Count; r++)
            {
                Row row = this.rows[r];

                if (offset != 0)
                {
                    Button edit = new();
                    edit.Click += delegate (object? sender, EventArgs e) { this.OnEdit?.Invoke(row.ID); };
                    edit.Image = Resources.Pencil;
                    edit.Size = Resources.Pencil.Size;

                    Button remove = new();
                    remove.Click += delegate (object? sender, EventArgs e) { this.OnRemove?.Invoke(row.ID); };
                    remove.Image = Resources.Delete;
                    remove.Size = Resources.Delete.Size;

                    this.fields.Add(new Tuple<Point, Control?, Alignment>(new Point(0, r + i), edit, Alignment.Left));
                    this.Controls.Add(edit);

                    this.fields.Add(new Tuple<Point, Control?, Alignment>(new Point(1, r + i), remove, Alignment.Left));
                    this.Controls.Add(remove);
                }

                int length = row.Cells.Count;
                for (int c=0; c<length; c++)
                {
                    Cell cell = row.Cells[c];
                    int index = c + offset;

                    Label l = new()
                    {
                        Text = cell.Text
                    };
                    if (cell.FontStyle != null)
                    {
                        l.Font = new Font(this.Font, cell.FontStyle.Value);
                    }
                    if (cell.Color != null)
                    {
                        l.ForeColor = cell.Color.Value;
                    }
                    Alignment alignment = Alignment.Left;
                    if(cell.TextAlignment != null)
                    {
                        alignment = cell.TextAlignment.Value;
                    }

                    this.fields.Add(new Tuple<Point, Control?, Alignment>(new Point(index, r + i), l, alignment));
                    this.Controls.Add(l);
                }

                if (offset == 0)
                {
                    Button edit = new();
                    edit.Click += delegate (object? sender, EventArgs e) { this.OnEdit?.Invoke(row.ID); };

                    Button remove = new();
                    remove.Click += delegate (object? sender, EventArgs e) { this.OnRemove?.Invoke(row.ID); };

                    this.fields.Add(new Tuple<Point, Control?, Alignment>(new Point(length + 0, r + i), edit, Alignment.Left));
                    this.Controls.Add(edit);

                    this.fields.Add(new Tuple<Point, Control?, Alignment>(new Point(length + 1, r + i), remove, Alignment.Left));
                    this.Controls.Add(remove);
                }
            }

            this.ComposeSizes();
        }

        private void ComposeHeader()
        {
            this.fields.Clear();
            this.Controls.Clear();
            this.Controls.Add(this.add);
            int offset = this.ControlLocation == ControlLocations.Left ? 2 : 0;
            if (offset != 0)
            {
                this.fields.Add(new Tuple<Point, Control?, Alignment>(new Point(0, 0), this.add, Alignment.Left));
                this.fields.Add(new Tuple<Point, Control?, Alignment>(new Point(1, 0), null, Alignment.Left));
            }

            int i = 0;
            foreach (Column column in this.columns)
            {
                int index = i + offset;

                Label l = new()
                {
                    Text = column.Text,
                    Font = new Font(this.Font, FontStyle.Bold)
                };
                this.fields.Add(new Tuple<Point, Control?, Alignment>(new Point(index, 0), l, Alignment.Left));
                this.Controls.Add(l);

                i++;
            }

            if (offset == 0)
            {
                this.fields.Add(new Tuple<Point, Control?, Alignment>(new Point(i + 0, 0), null, Alignment.Left));
                this.fields.Add(new Tuple<Point, Control?, Alignment>(new Point(i + 1, 0), this.add, Alignment.Left));
            }

            this.ComposeSizes();
        }

        private void ComposeSizes()
        {
            int maxY = this.fields.Select(x => x.Item1.Y).Max();
            int maxX = this.fields.Select(x => x.Item1.X).Max();

            Dictionary<int, int> columns = [];
            Dictionary<int, int> rows = [];

            for (int y = 0; y <= maxY; y++)
            {
                for (int x = 0; x <= maxX; x++)
                {
                    Point location = new(x, y);
                    Control? control = this.fields.Where(x => x.Item1.X == location.X && x.Item1.Y == location.Y).Select(x => x.Item2).FirstOrDefault();

                    Size size = new(0, 0);
                    if(control != null)
                    {
                        control.Size = control.PreferredSize;
                        size = control.Size;
                    }

                    if(columns.TryGetValue(x, out int valueX))
                    {
                        columns[x] = Math.Max(valueX, size.Width);
                    }
                    else
                    {
                        columns.Add(x, size.Width);
                    }

                    if(rows.TryGetValue(y, out int valueY))
                    {
                        rows[y] = Math.Max(valueY, size.Height);
                    }
                    else
                    {
                        rows.Add(y, size.Height);
                    }
                }
            }

            this.columnSizes = columns;
            this.rowSizes = rows;

            this.PositionControls();
        }

        private void PositionControls()
        {
            int padding = Gridview<Tid>.Padding;
            Dictionary<int, int> rows = this.rowSizes;
            Dictionary<int, int> columns = this.columnSizes;

            int maxY = rows.Keys.Max();
            int maxX = columns.Keys.Max();

            int offsetY = padding;
            for (int y = 0; y <= maxY; y++)
            {
                int offsetX = padding;
                for (int x = 0; x <= maxX; x++)
                {
                    Point index = new(x, y);
                    Size size = new(columns[x], rows[y]);
                    Control? control = this.fields.Where(x => x.Item1.X == index.X && x.Item1.Y == index.Y).Select(x => x.Item2).FirstOrDefault();
                    Alignment alignment = this.fields.Where(x => x.Item1.X == index.X && x.Item1.Y == index.Y).Select(x => x.Item3).FirstOrDefault();

                    if (control != null)
                    {
                        control.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        control.Location = new Point(offsetX, offsetY + ((size.Height - control.Height) / 2));
                        if(alignment == Alignment.Right)
                        {
                            int baseX = control.Location.X;
                            int newX = baseX + (size.Width - control.Width);
                            control.Location = new Point(newX, control.Location.Y);
                        }
                        else if (alignment == Alignment.Center)
                        {
                            int baseX = control.Location.X;
                            int newX = baseX + ((size.Width - control.Width) / 2);
                            control.Location = new Point(newX, control.Location.Y);
                        }
                    }
                    offsetX += size.Width + padding;
                }
                offsetY += rows.Values.Max() + padding;
            }
        }

        private void DrawHorizontalLines(Graphics g)
        {
            int padding = Gridview<Tid>.Padding;
            Pen p = Pens.Black;
            int w = this.columnSizes.Values.Sum() + (this.columnSizes.Count * padding);

            for (int yI = 1; yI < this.rowSizes.Count; yI++)
            {
                int y = this.rowSizes.Values.Take(yI).Sum() + (yI * padding) + (padding / 2);

                g.DrawLine(p, new PointF(0, y), new PointF(w, y));
            }
        }

        private void DrawVerticalLines(Graphics g)
        {
            int padding = Gridview<Tid>.Padding;
            Pen p = Pens.Black;
            int h = this.rowSizes.Values.Sum() + (this.rowSizes.Count * padding);

            int offset = this.ControlLocation == ControlLocations.Left ? 2 : 0;
            int offsetInverse = 2 - offset;

            for (int xI = offset; xI < this.columnSizes.Count - offsetInverse; xI++)
            {
                int x = this.columnSizes.Values.Take(xI).Sum() + (xI * padding) + (padding / 2);

                g.DrawLine(p, new PointF(x, 0), new PointF(x, h));
            }
        }
        #endregion //Private Methods

        #region Overrides
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            this.DrawHorizontalLines(g);
            this.DrawVerticalLines(g);
        }
        #endregion //Overrides

        #region Classes
        public class Row
        {
            #region Properties
            public List<Cell> Cells { get; set; }
            public Tid? ID { get; set; }
            #endregion //Properties

            #region Constructors
            public Row()
            {
                this.Cells = [];
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

        public class Column
        {
            #region Properties
            public string? Text { get; set; }
            #endregion //Properties
        }
        #endregion //Classes
    }
}
