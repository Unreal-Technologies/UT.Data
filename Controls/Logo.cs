namespace UT.Data.Controls
{
    public class Logo : PictureBox
    {
        public Logo() : base()
        {
            Image = Resources.Logo;
            Size = Image.Size;
            SizeMode = PictureBoxSizeMode.StretchImage;
        }
    }
}
