using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


namespace SmartStore.Web
{
    /// <summary>
    /// Summary description for LdapConfiguration
    /// </summary>
    public sealed class LdapConnectionConfigurationSection : ConfigurationSection
    {
        private static string ConfigSectionName = "LdapConnection";

        public static LdapConnectionConfigurationSection Current
        {
            get
            {
                return HttpContext.Current.GetSection(ConfigSectionName) as LdapConnectionConfigurationSection;
            }
        }

        private static ConfigurationPropertyCollection _properties;

        private static readonly ConfigurationProperty _server =
            new ConfigurationProperty("server",
            typeof(string), "localhost",
            ConfigurationPropertyOptions.IsKey);

        private static readonly ConfigurationProperty _port =
            new ConfigurationProperty("port",
            typeof(int), 389,
            ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty _baseDn =
            new ConfigurationProperty("baseDn",
            typeof(string), String.Empty,
            ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty _user =
            new ConfigurationProperty("user",
            typeof(string), String.Empty,
            ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty _password =
            new ConfigurationProperty("password",
            typeof(string), String.Empty,
            ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty _newUsersContainerDn =
            new ConfigurationProperty("newUsersContainerDn",
            typeof(string), String.Empty,
            ConfigurationPropertyOptions.IsRequired);


        public LdapConnectionConfigurationSection()
        {
            _properties =
                new ConfigurationPropertyCollection();

            _properties.Add(_server);
            _properties.Add(_port);
            _properties.Add(_baseDn);
            _properties.Add(_user);
            _properties.Add(_password);
            _properties.Add(_newUsersContainerDn);
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return _properties;
            }
        }

        public string Server
        {
            get
            {
                return (string)this["server"];
            }
        }

        public int Port
        {
            get
            {
                return (int)this["port"];
            }
        }

        public string BaseDn
        {
            get
            {
                return (string)this["baseDn"];
            }
        }

        public string User
        {
            get
            {
                return (string)this["user"];
            }
        }

        public string Password
        {
            get
            {
                return (string)this["password"];
            }
        }

        public string NewUsersContainerDn
        {
            get
            {
                return (string)this["newUsersContainerDn"];
            }
        }
    }
}