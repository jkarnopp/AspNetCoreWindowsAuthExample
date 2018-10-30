using System;
using System.Security.Principal;

namespace AspNetCoreWindowsAuthExample.Security
{
    /// <summary>
    ///
    /// </summary>
    public class AdInfo //: IIdentity
    {
        #region Public User Properties

        //
        // User Properties from Active Directory
        //

        /// <summary>
        /// Authentication Type of the User
        /// </summary>
        public string AuthenticationType
        {
            get { return (authenticationType); }
        }

        private string authenticationType;

        /// <summary>
        /// Whether the User is Authenticated
        /// </summary>
        public bool IsAuthenticated
        {
            get { return (isAuthenticated); }
        }

        private bool isAuthenticated;

        /// <summary>
        /// Unique Name for the User
        /// </summary>
        public string Name
        {
            get { return (name); }
        }

        private string name;

        /// <summary>
        /// User's ID
        /// </summary>
        public string UserId
        {
            get { return (userId); }
        }

        private string userId;

        /// <summary>
        /// User's Domain
        /// </summary>
        public string Domain
        {
            get { return (domain); }
        }

        private string domain;

        /// <summary>
        /// User's UPN Extension
        /// </summary>
        public string UpnExtension
        {
            get
            {
                if (string.IsNullOrEmpty(upnExtension))
                    if (!ADQueried) QueryActiveDirectory();
                return (upnExtension);
            }
        }

        private string upnExtension;

        /// <summary>
        /// User's UPN from Active Directory
        /// </summary>
        public string UserPrincipalName
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (userPrincipalName);
            }
        }

        private string userPrincipalName;

        /// <summary>
        /// User's First Name from Active Directory
        /// </summary>
        public string FirstName
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (firstName);
            }
        }

        private string firstName;

        /// <summary>
        /// User's Last Name from Active Directory
        /// </summary>
        public string LastName
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (lastName);
            }
        }

        private string lastName;

        /// <summary>
        /// User's Full Name from Active Directory
        /// </summary>
        public string FullName
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (string.Format("{0} {1}", firstName, lastName));
            }
        }

        /// <summary>
        /// User's Display Name from Active Directory
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (displayName);
            }
        }

        private string displayName;

        /// <summary>
        /// User's Phone Number from Active Directory
        /// </summary>
        public string Phone
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (phone);
            }
        }

        private string phone;

        /// <summary>
        /// User's Fax Number from Active Directory
        /// </summary>
        public string Fax
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (fax);
            }
        }

        private string fax;

        /// <summary>
        /// User's Email Address from Active Directory
        /// </summary>
        public string EmailAddress
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (emailAddress);
            }
        }

        private string emailAddress;

        /// <summary>
        /// User's Department from Active Directory
        /// </summary>
        public string Department
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (department);
            }
        }

        private string department;

        /// <summary>
        /// User's Company from Active Directory
        /// </summary>
        public string Company
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (company);
            }
        }

        private string company;

        /// <summary>
        /// User's Address from Active Directory
        /// </summary>
        public string Address
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (address);
            }
        }

        private string address;

        /// <summary>
        /// User's City from Active Directory
        /// </summary>
        public string City
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (city);
            }
        }

        private string city;

        /// <summary>
        /// User's State from Active Directory
        /// </summary>
        public string State
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (state);
            }
        }

        private string state;

        /// <summary>
        /// User's Postal Code from Active Directory
        /// </summary>
        public string ZipCode
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (postalCode);
            }
        }

        public string PostalCode
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (postalCode);
            }
        }

        private string postalCode;

        /// <summary>
        /// User's Country from Active Directory
        /// </summary>
        public string Country
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (country);
            }
        }

        private string country;

        /// <summary>
        /// Whether user is an internal employee
        /// </summary>
        public bool IsInternal
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                if (UpnExtension.Equals("kt"))
                    return (true);
                else
                    return (false);
            }
        }

        /// <summary>
        /// Whether user is from an external company
        /// </summary>
        public bool IsExternal
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                if (UpnExtension.Equals("kt-extern"))
                    return (true);
                else
                    return (false);
            }
        }

        /// <summary>
        /// Array of all Roles that the User is assigned
        /// </summary>
        public string[] Roles
        {
            get
            {
                string[] roles = null;

                // Add the list of AD groups to the list of roles
                if (ADGroups != null)
                    roles = (string[])ADGroups.Clone();

                // Add the list of Access Control roles to the list of roles
                if (ACRoles != null)
                {
                    if (roles != null)
                        ACRoles.CopyTo(roles, roles.Length);
                    else
                        roles = (string[])ACRoles.Clone();
                }

                // Return all the roles
                return (roles);
            }
        }

        /// <summary>
        /// Array of Active Directory Groups that the User is assigned
        /// </summary>
        public string[] ADGroups
        {
            get
            {
                if (!ADQueried) QueryActiveDirectory();
                return (adGroups);
            }
        }

        private string[] adGroups;

        /// <summary>
        /// Array of Access Control Roles that the User is assigned
        /// </summary>
        public string[] ACRoles
        {
            get
            {
                return (acRoles);
            }
        }

        private string[] acRoles;

        #endregion Public User Properties

        #region Private Properties

        /// <summary>
        /// Connection Information for Active Directory
        /// </summary>
        private ActiveDirectoryConnectionInfo adInfo;

        /// <summary>
        /// Whether Active Directory has been queried
        /// </summary>
        private bool ADQueried;

        #endregion Private Properties

        /// <summary>
        /// Create a new instance of the Business Portal Identity class
        /// </summary>
        /// <param name="identity">Identity object of current user</param>
        /// <param name="connectionInfo">Information for connecting to Active Directory</param>
        public AdInfo(String identity, ActiveDirectoryConnectionInfo connectionInfo)
        {
            SetActiveDirectoryConnectionInfo(connectionInfo);
            SetIdentity(identity);
        }

        /// <summary>
        /// Provide connection information for Active Directory
        /// </summary>
        /// <param name="connectionInfo">Information for connecting to Active Directory</param>
        public void SetActiveDirectoryConnectionInfo(ActiveDirectoryConnectionInfo connectionInfo)
        {
            adInfo = connectionInfo;
        }

        /// <summary>
        /// Set the User Identity Information
        /// </summary>
        /// <param name="identity">Identity object of current user</param>
        public void SetIdentity(String identity)
        {
            this.name = identity;//.Name;
                                 //this.isAuthenticated = identity.IsAuthenticated;
                                 //this.authenticationType = identity.AuthenticationType;

            if (Name.Contains("@"))
            {
                userId = Name.Substring(0, Name.IndexOf("@"));
                upnExtension = Name.Substring(Name.IndexOf("@") + 1);
            }
            else if (Name.Contains(@"\"))
            {
                userId = Name.Substring(Name.IndexOf(@"\") + 1);
                domain = Name.Substring(0, Name.IndexOf(@"\"));
            }
        }

        /// <summary>
        /// Query Active Directory and populate properties with AD values
        /// </summary>
        private void QueryActiveDirectory()
        {
            ActiveDirectory AD = new ActiveDirectory(adInfo);

            // Make sure that we do not try to query the data again
            ADQueried = true;

            if (AD.FindEntryByUserName(this.UserId))
            {
                // Populate the properties
                userPrincipalName = AD.GetUserPrincipalName();
                upnExtension = userPrincipalName.Substring(userPrincipalName.IndexOf("@") + 1);
                lastName = AD.GetSurName();
                firstName = AD.GetGivenName();
                displayName = AD.GetDisplayName();
                phone = AD.GetPhoneNumber();
                fax = AD.GetFaxNumber();
                emailAddress = AD.GetEmail();
                department = AD.GetDepartment();
                company = AD.GetCompany();
                address = AD.GetAddress();
                city = AD.GetCity();
                state = AD.GetState();
                postalCode = AD.GetPostalCode();
                country = AD.GetCountry();
                userId = AD.GetUserId();

                // Retrieve AD Groups
                // adGroups				= AD.GetGroups();
                //if (adGroups != null) Array.Sort(adGroups);
            }
        }
    }
}