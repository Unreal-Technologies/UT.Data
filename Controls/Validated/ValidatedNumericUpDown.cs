namespace UT.Data.Controls.Validated
{
    public class ValidatedNumericUpDown : Validated<NumericUpDown>
    {
        public ValidatedNumericUpDown() : base(delegate (NumericUpDown nud) 
        {
            return nud.Value.ToString();
        })
        {

        }
    }
}
