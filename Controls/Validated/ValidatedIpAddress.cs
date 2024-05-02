using System.Net;
using System.Text.RegularExpressions;

namespace UT.Data.Controls.Validated
{
    public partial class ValidatedIpAddress : Validated<TextBox>
    {
        #region Constructors
        public ValidatedIpAddress(): base(delegate(TextBox tb)
        {
            return tb.Text;
        })
        {
            AddValidation(delegate (TextBox tb)
            {
                if (!IpRegex().Match(tb.Text).Success)
                {
                    return new Tuple<bool, string>(false, "Is not a valid IP format.");
                }
                if (!IPAddress.TryParse(tb.Text, out IPAddress? outIp))
                {
                    return new Tuple<bool, string>(false, "Is not a valid IP address.");
                }

                return new Tuple<bool, string>(true, string.Empty);
            });
        }
        #endregion //Constructors

        #region Private Methods
        [GeneratedRegex(@"([0-9]{1,3}(\.)){3}[0-9]{1,3}")]
        private static partial Regex IpRegex();
        #endregion //Private Methods
    }
}
