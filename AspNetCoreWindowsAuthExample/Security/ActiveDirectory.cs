using System;
using System.Collections;
using System.Configuration;
using System.DirectoryServices;
using System.Text;

namespace AspNetCoreWindowsAuthExample.Security
{
    //todo: This class should be updated to be the main class that returns a POCO, list of POCO, and single types.

    /// <summary>
    /// Contains all of the information necessary to connect to the Active Directory server
    /// </summary>
    /// <example>
    /// AdInfo id = new AdInfo(WindowsIdentity.GetCurrent(), new ActiveDirectoryConnectionInfo(GlobalCatalog, BasePath, UserName, Password));
    /// </example>
    public class ActiveDirectoryConnectionInfo
    {
        /// <summary>
        /// Active Directory Connection String
        /// </summary>
        public string ConnectionString { get { return (connectionString); } }

        private string connectionString;

        /// <summary>
        /// Active Directory Base Path
        /// </summary>
        public string BasePath { get { return (basePath); } }

        private string basePath;

        /// <summary>
        /// Active Directory User Name
        /// </summary>
        public string UserName { get { return (userName); } }

        private string userName;

        /// <summary>
        /// Active Directory Password
        /// </summary>
        public string Password { get { return (password); } }

        private string password;

        /// <summary>
        /// Create a new instance of the ActiveDirectoryConnectionInfo object
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="BasePath"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        public ActiveDirectoryConnectionInfo(string connectionString, string basePath, string userName, string password)
        {
            this.connectionString = connectionString;
            this.basePath = basePath;
            this.userName = userName;
            this.password = password;
        }
    }

    /// <summary>
    /// Connects and retrieves information from Active Directory
    /// </summary>
    public class ActiveDirectory
    {
        /// <summary>
        /// List of records returned from Active Directory
        /// </summary>
        public ArrayList Entries { get { return (entries); } }

        private ArrayList entries = new ArrayList();

        /// <summary>
        /// List of attribute names in Active Directory
        /// </summary>
        private string[] attributeList = new string[] { "cn", "userPrincipleName", "sAMAccountName", "givenName", "sn", "displayName", "department", "company", "streetAddress", "l", "st", "postalCode", "c", "co", "telephoneNumber", "facsimileTelephoneNumber", "mail", "memberOf", "member", "objectClass" };

        /// <summary>
        /// Specifies whether the LDAP server is available
        /// </summary>
        private ActiveDirectoryConnectionInfo AD;

        /// <summary>
        /// Whether Active Directory is available
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return this.isAvailable;
            }
        }

        private bool isAvailable;

        /// <summary>
        /// used to flag upstream code of error condition.
        /// </summary>
        public bool Error { get { return this.error; } }

        private bool error;

        /// <summary>
        /// Creates an instance of the ActiveDirectory class
        /// </summary>
        /// <param name="ConnectionInfo">Information for connecting to Active Directory</param>
        public ActiveDirectory(ActiveDirectoryConnectionInfo connectionInfo)
        {
            AD = connectionInfo;
            isAvailable = true;
        }

        /// <summary>
        /// Creates an instance of the ActiveDirectory class
        /// </summary>
        public ActiveDirectory() { }

        /// <summary>
        /// Connects to the LDAP Server and gathers information for the specified UserName
        /// </summary>
        /// <param name="userName">UserName in the LDAP Server</param>
        /// <param name="hasWildCard">Specifies whether a wild card search will be performed</param>
        /// <returns>True if the user was found in the LDAP Repository</returns>
        public bool FindEntryByUserName(string userName, bool hasWildcard)
        {
            // Performing a wild card search requires no extra info
            string wildcard = "";

            // Limit search to exact UserName provided
            if (!hasWildcard)
                wildcard = "@";

            // Build the Query string, ie) Search Criteria
            string filter = "";

            // Check which Network the application is running on
            filter = "(&(objectclass=user)(userPrincipalName=" + userName + wildcard + "*))";

            // Search the LDAP Server
            if (FindEntries(filter) > 0)
                return (true);
            else
                return (false);
        }

        /// <summary>
        /// Connects to the LDAP Server and gathers information for the specified UserName
        /// </summary>
        /// <param name="UserName">UserName in the LDAP Server</param>
        /// <returns>True if the user was found in the LDAP Repository</returns>
        public bool FindEntryByUserName(string userName)
        {
            return (FindEntryByUserName(userName, false));
        }

        /// <summary>
        /// Connects to Active Directory and gathers information for the specified list of users
        /// </summary>
        /// <param name="users">List of user names in Active Directory</param>
        /// <returns># of Users found from the list of Users</returns>
        public int FindEntriesByUserNames(ArrayList users)
        {
            // Build the search filter, for one user it is just (!(userPrincipalName=[UserID]@*))
            // for mutliple users it is (!(userPrincipalName=User1@*)(userPrincipalName=User2@*))
            ArrayList filters = new ArrayList();
            string filter = "";
            int userCount = 0;
            foreach (string userName in users)
            {
                string addFilter = "";

                // Add each user to the search criteria
                addFilter += "(userPrincipalName=" + userName.Trim() + "@*)";

                if (userCount > 950)
                {
                    // Add the filter to the list of filters
                    filter = "(&(objectclass=user)(|" + filter + "))";
                    filters.Add(filter);
                    filter = "";
                    userCount = 0;
                }

                // Add the user to the list of users to search for (filter on)
                filter += addFilter;
                userCount++;
            }

            // Make sure that the last one gets added to the list
            filter = "(&(objectclass=user)(|" + filter + "))";
            filters.Add(filter);

            // Search Active Directory
            return (FindEntries(filters));
        }

        /// <summary>
        /// Connects to Active Directory and gathers information for the specified list of users
        /// </summary>
        /// <param name="users">List of user names in Active Directory</param>
        /// <returns># of Users found from the list of Users</returns>
        public int FindEntriesByComputerNames(ArrayList users)
        {
            // Build the search filter, for one user it is just (!(userPrincipalName=[UserID]@*))
            // for mutliple users it is (!(userPrincipalName=User1@*)(userPrincipalName=User2@*))
            ArrayList filters = new ArrayList();
            string filter = "";
            int userCount = 0;
            foreach (string userName in users)
            {
                string addFilter = "";

                // Add each user to the search criteria
                addFilter += "(sAMAccountName=" + userName + "$)";

                if (userCount > 950)
                {
                    // Add the filter to the list of filters
                    filter = "(&(objectclass=computer)(|" + filter + "))";
                    filters.Add(filter);
                    filter = "";
                    userCount = 0;
                }

                // Add the user to the list of users to search for (filter on)
                filter += addFilter;
                userCount++;
            }

            // Make sure that the last one gets added to the list
            filter = "(&(objectclass=computer)(|" + filter + "))";
            filters.Add(filter);

            // Search Active Directory
            return (FindEntries(filters));
        }

        /// <summary>
        /// Connects to Active Directory and gathers information for the specified list of users
        /// </summary>
        /// <param name="users">List of user names in Active Directory</param>
        /// <returns># of Users found from the list of Users</returns>
        public int FindEntriesByGroupNames(ArrayList users)
        {
            // Build the search filter, for one user it is just (!(userPrincipalName=[UserID]@*))
            // for mutliple users it is (!(userPrincipalName=User1@*)(userPrincipalName=User2@*))
            ArrayList filters = new ArrayList();
            string filter = "";
            int userCount = 0;
            foreach (string userName in users)
            {
                string addFilter = "";

                // Add each user to the search criteria
                addFilter += "(sAMAccountName=" + userName + ")";

                if (userCount > 950)
                {
                    // Add the filter to the list of filters
                    filter = "(&(objectclass=group)(|" + filter + "))";
                    filters.Add(filter);
                    filter = "";
                    userCount = 0;
                }

                // Add the user to the list of users to search for (filter on)
                filter += addFilter;
                userCount++;
            }

            // Make sure that the last one gets added to the list
            filter = "(&(objectclass=group)(|" + filter + "))";
            filters.Add(filter);

            // Search Active Directory
            return (FindEntries(filters));
        }

        /// <summary>
        /// Connects to Active Directory and gathers information for the specified list of users
        /// </summary>
        /// <param name="users">List of user names in Active Directory</param>
        /// <returns># of Users found from the list of Users</returns>
        public int FindEntriesByFolderNames(ArrayList users)
        {
            // Build the search filter, for one user it is just (!(userPrincipalName=[UserID]@*))
            // for mutliple users it is (!(userPrincipalName=User1@*)(userPrincipalName=User2@*))
            ArrayList filters = new ArrayList();
            string filter = "";
            int userCount = 0;
            foreach (string userName in users)
            {
                string addFilter = "";

                // Add each user to the search criteria
                addFilter += "(displayName=" + userName + ")";

                if (userCount > 950)
                {
                    // Add the filter to the list of filters
                    filter = "(&(objectclass=publicFolder)(|" + filter + "))";
                    filters.Add(filter);
                    filter = "";
                    userCount = 0;
                }

                // Add the user to the list of users to search for (filter on)
                filter += addFilter;
                userCount++;
            }

            // Make sure that the last one gets added to the list
            filter = "(&(objectclass=publicFolder)(|" + filter + "))";
            filters.Add(filter);

            // Search Active Directory
            return (FindEntries(filters));
        }

        /// <summary>
        /// Connects to Active Directory and searches on a specified search criteria
        /// </summary>
        /// <param name="filter">Criteria to search Active Directory</param>
        /// <returns># of Entries found in Active Directory</returns>
        public int FindEntries(string filter)
        {
            // Specifies whether a User was found when Active Directory was queried
            int Count = 0;

            // If the filter is not set, then running a query will return all records
            // since this is not the desired output, we ignore it when filter is an
            // empty string.
            if (!string.IsNullOrEmpty(filter))
            {
                // Do not even try to query the LDAP if it is not available
                if (this.IsAvailable)
                {
                    DirectoryEntry Ldap = null;
                    DirectorySearcher LdapSearch;

                    // Connect to the LDAP Server with the administrator's credentials
                    Ldap = new DirectoryEntry(AD.ConnectionString, "cn=" + AD.UserName + "," + AD.BasePath, AD.Password, AuthenticationTypes.ReadonlyServer);

                    // Execute the Search Query
                    try
                    {
                        LdapSearch = new DirectorySearcher(Ldap, filter, attributeList, SearchScope.Subtree);
                    }
                    catch (System.Exception ex)
                    {
                        throw new Exception("Filter:" + filter, ex);
                    }

                    // Retrieve the list of properties from the returned User
                    SearchResultCollection dsc = LdapSearch.FindAll();
                    foreach (SearchResult result in dsc)
                    {
                        entries.Add(result.GetDirectoryEntry());
                    }

                    // We will be returning the number of Entries returned from the search
                    Count = entries.Count;

                    // Close the connection to Ldap
                    Ldap.Close();
                }
            }

            // Return whether the User was found in LDAP query
            return (Count);
        }

        /// <summary>
        /// Connects to Active Directory and searches on a specified search criteria
        /// </summary>
        /// <param name="filters">List of Criteria to search Active Directory</param>
        /// <returns># of Entries found in Active Directory</returns>
        public int FindEntries(ArrayList filters)
        {
            // Specifies whether a User was found when the LDAP Server was queries
            int Count = 0;

            // Do not even try to query Active Directory if it is not available
            if (this.IsAvailable)
            {
                // This allows us to break up a really long query in to several shorter ones that the
                // Active Directory system can handle.
                foreach (string filter in filters)
                {
                    // If the filter is not set, then running a query will return all records
                    // since this is not the desired output, we ignore it when filter is an
                    // empty string.
                    if (!string.IsNullOrEmpty(filter))
                    {
                        DirectoryEntry Ldap = null;
                        DirectorySearcher LdapSearch;

                        Ldap = new DirectoryEntry(AD.ConnectionString, "cn=" + AD.UserName + "," + AD.BasePath, AD.Password, AuthenticationTypes.ReadonlyServer);

                        // Execute the Search Query
                        try
                        {
                            LdapSearch = new DirectorySearcher(Ldap, filter, attributeList, SearchScope.Subtree);
                            LdapSearch.SizeLimit = 20;
                        }
                        catch (System.Exception ex)
                        {
                            throw new Exception("Filter:" + filter, ex);
                        }

                        // Retrieve the list of properties from the returned User
                        SearchResultCollection dsc = LdapSearch.FindAll();

                        foreach (SearchResult result in dsc)
                        {
                            entries.Add(result.GetDirectoryEntry());
                        }

                        // We will be returning the number of Entries returned from the search
                        Count = entries.Count;

                        // Close the connection to Ldap
                        Ldap.Close();
                    }
                }
            }

            // Return whether the User was found in LDAP query
            return (Count);
        }

        /// <summary>
        /// Query Active Directory for specified filter and return a list of Email Address for the list of Entries
        /// </summary>
        /// <param name="filter">List of Criteria to search Active Directory</param>
        /// <returns>Semicolon seperated list of Email Addresses</returns>
        public string GetEmailAddresses(ArrayList filter)
        {
            StringBuilder emails = new StringBuilder();

            // Retrieve the list of
            if (FindEntries(filter) > 0)
            {
                foreach (DirectoryEntry e in entries)
                {
                    emails.Append(string.Format("{0} <{1}>;", GetProperty("givenName", e) + " " + GetProperty("sn", e), GetProperty("mail", e)));
                }
            }

            // Return the list of email address
            return (emails.ToString());
        }

        /// <summary>
        /// Checks if the User's attributes contains the specified Property
        /// </summary>
        /// <param name="PropertyName">Name of property to check</param>
        /// <returns>True, if the User has the property defined</returns>
        public bool HasProperty(string propertyName)
        {
            // Retrieve the 1st LdapEntry object
            DirectoryEntry entry = null;
            if (entries.Count > 0)
                entry = (DirectoryEntry)entries[0];
            else
                throw new Exception("no entry retrieved yet");

            // Retrieve the Property value from the LdapEntry object
            return (HasProperty(propertyName, entry));
        }

        /// <summary>
        /// Checks if the User's attributes contains the specified Property
        /// </summary>
        /// <param name="propertyName">Name of property to check</param>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>True, if the User has the property defined</returns>
        public static bool HasProperty(string propertyName, DirectoryEntry entry)
        {
            // Return the property's value
            return (entry.Properties.Contains(propertyName));
        }

        /// <summary>
        /// Retrieves the specified Property from the User's attributes
        /// </summary>
        /// <param name="PropertyName">Name of property to retrieve</param>
        /// <returns>The value for the specified property name</returns>
        public string GetProperty(string propertyName)
        {
            // Retrieve the 1st LdapEntry object
            DirectoryEntry entry = null;
            if (entries.Count > 0)
                entry = (DirectoryEntry)entries[0];
            else
                throw new Exception("no entry retrieved yet");

            // Retrieve the Property value from the LdapEntry object
            return (GetProperty(propertyName, entry));
        }

        /// <summary>
        /// Retrieves the specified Property from the User's attributes
        /// </summary>
        /// <param name="PropertyName">Name of property to retrieve</param>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>The value for the specified property name</returns>
        public static string GetProperty(string propertyName, DirectoryEntry entry)
        {
            // Return the property's value
            if (entry.Properties[propertyName] != null)
            {
                PropertyValueCollection properties = entry.Properties[propertyName];

                if (properties.Count == 1)
                    return (properties.Value.ToString());
                else
                    return (string.Empty);
            }

            // Return an empty string if the value was not found
            else
                return (string.Empty);
        }

        /// <summary>
        /// Retrieves the specified Property Set from the User's attributes
        /// The Property Set is a string array of properties
        /// </summary>
        /// <param name="PropertyName">Name of property to retrieve</param>
        /// <returns>The value for the specified property name</returns>
        public string[] GetPropertySet(string propertyName)
        {
            // Retrieve the 1st LdapEntry object
            DirectoryEntry entry = null;
            if (entries.Count > 0)
                entry = (DirectoryEntry)entries[0];
            else
                throw new Exception("no entry retrieved yet");

            // Retrieve the Property value from the LdapEntry object
            return (GetPropertySet(propertyName, entry));
        }

        /// <summary>
        /// Retrieves the specified Property Set from the User's attributes
        /// The Property Set is a string array of properties
        /// </summary>
        /// <param name="PropertyName">Name of property to retrieve</param>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>The value for the specified property name</returns>
        public static string[] GetPropertySet(string propertyName, DirectoryEntry entry)
        {
            // Return the property's value
            if (entry.Properties[propertyName] != null && entry.Properties.Contains(propertyName))
            {
                // Create the string Array that will be returned
                string[] prop;

                // The Property will only be an Array if there is more than one value
                if (entry.Properties[propertyName].Value is Array)
                {
                    Array propertySet = (Array)entry.Properties[propertyName].Value;
                    prop = new string[propertySet.Length];
                    for (int i = 0; i < propertySet.Length; i++)
                    {
                        prop[i] = propertySet.GetValue(i).ToString();
                    }
                }
                else
                {
                    prop = new string[1];
                    prop[0] = entry.Properties[propertyName].Value.ToString();
                }

                // Return the Array of Strings
                return (prop);
            }

            // Return an empty string if the value was not found
            else
            {
                return (null);
            }
        }

        #region Get Properties from Active Directory

        /// <summary>
        ///  Get the property givenName from Active Directory
        /// </summary>
        /// <returns>Value of the givenName property</returns>
        public string GetGivenName()
        {
            return GetProperty("givenName");
        }

        /// <summary>
        /// Get the property givenName from Active Directory
        /// </summary>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>Value of the givenName property</returns>
        public static string GetGivenName(DirectoryEntry entry)
        {
            return GetProperty("givenName", entry);
        }

        /// <summary>
        ///  Get the property surName from Active Directory
        /// </summary>
        /// <returns>Value of the surName property</returns>
        public string GetSurName()
        {
            return GetProperty("sn");
        }

        /// <summary>
        /// Get the property SurName from Active Directory
        /// </summary>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>Value of the surName property</returns>
        public static string GetSurName(DirectoryEntry entry)
        {
            return GetProperty("sn", entry);
        }

        /// <summary>
        ///  Get the DisplayName property from Active Directory
        /// </summary>
        /// <returns>Value of the DisplayName property</returns>
        public string GetDisplayName()
        {
            return GetProperty("displayName");
        }

        /// <summary>
        /// Get the DisplayName property from Active Directory
        /// </summary>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>Value of the DisplayName property</returns>
        public static string GetDisplayName(DirectoryEntry entry)
        {
            return GetProperty("displayName", entry);
        }

        /// <summary>
        ///  Get the FaxNumber property from Active Directory
        /// </summary>
        /// <returns>Value of the FaxNumber property</returns>
        public string GetFaxNumber()
        {
            return GetProperty("facsimileTelephoneNumber");
        }

        /// <summary>
        /// Get the FaxNumber property from Active Directory
        /// </summary>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>Value of the FaxNumber property</returns>
        public static string GetFaxNumber(DirectoryEntry entry)
        {
            return GetProperty("facsimileTelephoneNumber", entry);
        }

        /// <summary>
        ///  Get the PhoneNumber property from Active Directory
        /// </summary>
        /// <returns>Value of the PhoneNumber property</returns>
        public string GetPhoneNumber()
        {
            return GetProperty("telephoneNumber");
        }

        /// <summary>
        /// Get the PhoneNumber property from Active Directory
        /// </summary>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>Value of the PhoneNumber property</returns>
        public static string GetPhoneNumber(DirectoryEntry entry)
        {
            return GetProperty("telephoneNumber", entry);
        }

        /// <summary>
        ///  Get the property mail from Active Directory
        /// </summary>
        /// <returns>Value of the e-mail property</returns>
        public string GetEmail()
        {
            return GetProperty("mail");
        }

        /// <summary>
        /// Get the property e-mail from Active Directory
        /// </summary>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>Value of the Mail property</returns>
        public static string GetEmail(DirectoryEntry entry)
        {
            return GetProperty("mail", entry);
        }

        /// <summary>
        ///  Get the company property from Active Directory
        /// </summary>
        /// <returns>Value of the company property</returns>
        public string GetCompany()
        {
            return GetProperty("company");
        }

        /// <summary>
        /// Get the company property from Active Directory
        /// </summary>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>Value of the company property</returns>
        public static string GetCompany(DirectoryEntry entry)
        {
            return GetProperty("company", entry);
        }

        /// <summary>
        ///  Get the department property from Active Directory
        /// </summary>
        /// <returns>Value of the department property</returns>
        public string GetDepartment()
        {
            return GetProperty("department");
        }

        /// <summary>
        /// Get the department property from Active Directory
        /// </summary>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>Value of the department property</returns>
        public static string GetDepartment(DirectoryEntry entry)
        {
            return GetProperty("department", entry);
        }

        /// <summary>
        ///  Get the Address property from Active Directory
        /// </summary>
        /// <returns>Value of the Address property</returns>
        public string GetAddress()
        {
            return GetProperty("streetAddress");
        }

        /// <summary>
        /// Get the Address property from Active Directory
        /// </summary>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>Value of the Address property</returns>
        public static string GetAddress(DirectoryEntry entry)
        {
            return GetProperty("streetAddress", entry);
        }

        /// <summary>
        ///  Get the city property from Active Directory
        /// </summary>
        /// <returns>Value of the city property</returns>
        public string GetCity()
        {
            return GetProperty("l");
        }

        /// <summary>
        /// Get the city property from Active Directory
        /// </summary>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>Value of the city property</returns>
        public static string GetCity(DirectoryEntry entry)
        {
            return GetProperty("l", entry);
        }

        /// <summary>
        ///  Get the state property from Active Directory
        /// </summary>
        /// <returns>Value of the state property</returns>
        public string GetState()
        {
            return GetProperty("st");
        }

        /// <summary>
        /// Get the state property from Active Directory
        /// </summary>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>Value of the state property</returns>
        public static string GetState(DirectoryEntry entry)
        {
            return GetProperty("st", entry);
        }

        /// <summary>
        ///  Get the PostalCode property from Active Directory
        /// </summary>
        /// <returns>Value of the PostalCode property</returns>
        public string GetPostalCode()
        {
            return GetProperty("postalCode");
        }

        /// <summary>
        /// Get the PostalCode property from Active Directory
        /// </summary>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>Value of the PostalCode property</returns>
        public static string GetPostalCode(DirectoryEntry entry)
        {
            return GetProperty("postalCode", entry);
        }

        /// <summary>
        ///  Get the country property from Active Directory
        /// </summary>
        /// <returns>Value of the country property</returns>
        public string GetCountry()
        {
            return GetProperty("c");
        }

        /// <summary>
        /// Get the country property from Active Directory
        /// </summary>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>Value of the country property</returns>
        public static string GetCountry(DirectoryEntry entry)
        {
            return GetProperty("c", entry);
        }

        /// <summary>
        /// Get the simplified user id from Active Directory
        /// </summary>
        /// <returns>Value of the simplified user id</returns>
        public string GetUserId()
        {
            return GetProperty("sAMAccountName");
        }

        /// <summary>
        /// Get the simplified user id from Active Directory
        /// </summary>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>Value of the simplified user id</returns>
        public static string GetUserId(DirectoryEntry entry)
        {
            return GetProperty("sAMAccountName", entry);
        }

        /// <summary>
        /// Get the property userPrincipalName from Active Directory
        /// </summary>
        /// <returns>Value of the userPrincipalName property</returns>
        public string GetUserPrincipalName()
        {
            return GetProperty("userPrincipalName");
        }

        /// <summary>
        /// Get the property userPrincipalName from Active Directory
        /// </summary>
        /// <param name="entry">Specific user entry record to check</param>
        /// <returns>Value of the userPrincipalName property</returns>
        public static string GetUserPrincipalName(DirectoryEntry entry)
        {
            return GetProperty("userPrincipalName", entry);
        }

        /// <summary>
        ///  Get the property set  groups from Active Directory
        /// </summary>
        /// <returns>Value of the groups property</returns>
        public string[] GetGroups()
        {
            string[] groups = GetPropertySet("memberOf");

            // Remove the additional information that AD applies and return just the group name
            for (int i = 0; i < groups.Length; i++)
                groups[i] = groups[i].Substring(3, groups[i].IndexOf(",") - 3);

            return (groups);
        }

        #endregion Get Properties from Active Directory

        /// <summary>
        /// Retrieves the email address of the Requester
        /// </summary>
        /// <returns>List of Email Addresses</returns>
        public static string GetRequesterEmail(string userName)
        {
            // Use this to build a string to query against the LDAP server
            ArrayList filters = new ArrayList();
            // Check which Network the application is running on
            filters.Add("(&(objectclass=user)(userPrincipalName=" + userName + "@*))");

            // Get the Email Addresses from LDAP based on the filter
            return (GetEmailsFromLdap(filters));
        }

        /// <summary>
        /// Query LDAP for specified filter and return a list of Email Address for the list of Entries
        /// </summary>
        /// <param name="filter">Search Criteria</param>
        /// <returns>List of Email Addresses</returns>
        public static string GetEmailsFromLdap(ArrayList filter)
        {
            string emails = "";

            // We need to access LDAP to get the email addresses
            ActiveDirectory Ldap = new ActiveDirectory();

            // Retrieve the list of
            emails = Ldap.GetEmailAddresses(filter);

            return (emails);
        }

        /// <summary>
        /// Query LDAP for specified filter and return a list of Email Address for the list of Entries
        /// </summary>
        /// <param name="filter">Search Criteria</param>
        /// <returns>List of Email Addresses</returns>
        public string GetEmailsAddressesByUserNames(ArrayList filter)
        {
            // Retrieve the list of
            StringBuilder emails = new StringBuilder();

            // Retrieve the list of
            if (FindEntriesByUserNames(filter) > 0)
            {
                foreach (DirectoryEntry e in Entries)
                {
                    emails.Append(string.Format("{0} <{1}>;", GetProperty("givenName", e) + " " + GetProperty("sn", e), GetProperty("mail", e)));
                }
            }

            // Return the list of email address
            return (emails.ToString());
        }
    }
}