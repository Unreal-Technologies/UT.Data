namespace UT.Data.Controls.Validated
{
    public class ValidatedComboBox : Validated<ComboBox>
    {
        public ValidatedComboBox() : base(delegate(ComboBox cb)
        {
            return cb.SelectedItem?.ToString();
        })
        {

        }
    }
}
