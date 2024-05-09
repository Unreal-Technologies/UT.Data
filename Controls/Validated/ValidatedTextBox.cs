namespace UT.Data.Controls.Validated
{
    public class ValidatedTextBox : Validated<TextBox>
    {
        public ValidatedTextBox() : base(delegate (TextBox textBox)
        {
            return textBox.Text;
        })
        {
        }
    }
}
