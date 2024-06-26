﻿namespace UT.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DefaultAttribute(string name, string value) : Attribute
    {
        #region Constructors
        public DefaultAttribute(string name, int value)
            : this(name, value.ToString())
        {
        }
        #endregion //Constructors

        #region Members
        private readonly string name = name;
        private readonly string value = value;
        #endregion //Members

        #region Properties
        public string Name
        {
            get { return name; }
        }

        public string Value
        {
            get { return value; }
        }

        #endregion //Properties
    }
}
