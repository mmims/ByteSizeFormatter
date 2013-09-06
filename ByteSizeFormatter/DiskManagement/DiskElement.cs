using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace DiskManagement
{
    public abstract class DiskElement
    {
        #region Private Members

        private readonly string _deviceID;
        private Dictionary<string, object> _properties;

        #endregion Private Members

        protected DiskElement(string deviceID)
        {
            _deviceID = deviceID;
            _properties = new Dictionary<string, object>();
            Size = 0;
        }

        #region Properties

        public string DeviceID
        {
            get { return _deviceID; }
        }

        public Dictionary<string, object> Properties
        {
            get { return _properties; }
        }

        public UInt64 Size
        {
            get;
            protected set;
        }
        #endregion Properties

        #region Instance Methods

        protected void AddProperty(PropertyData property)
        {
            _properties.Add(property.Name, property.Value);
        }

        public string GetSizeString(string format = "SF")
        {
            return String.Format(new ByteSizeFormatter(), "{0:" + format + "}", this.Size);
        }

        #endregion Instance Methods
    }
}
