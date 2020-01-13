using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;

namespace bicen.Helpers
{
    public class Utilities
    {
        private static string _domainLDAP_HCM = WebConfigurationManager.AppSettings["LDAP_CONNECTION_HCM"];
        private static string _portLDAP_HCM = WebConfigurationManager.AppSettings["LDAP_PORT_HCM"];
        private static string _domainLDAP_HN = WebConfigurationManager.AppSettings["LDAP_CONNECTION_HN"];
        private static string _portLDAP_HN = WebConfigurationManager.AppSettings["LDAP_PORT_HN"];
        private static string _domainLDAP_CPC = WebConfigurationManager.AppSettings["LDAP_CONNECTION_CPC"];
        private static string _portLDAP_CPC = WebConfigurationManager.AppSettings["LDAP_PORT_CPC"];
        private static string _domainLDAP_NPC = WebConfigurationManager.AppSettings["LDAP_CONNECTION_NPC"];
        private static string _portLDAP_NPC = WebConfigurationManager.AppSettings["LDAP_PORT_CPC"];
        private static string _domainLDAP_SPC = WebConfigurationManager.AppSettings["LDAP_CONNECTION_SPC"];
        private static string _portLDAP_SPC = WebConfigurationManager.AppSettings["LDAP_PORT_SPC"];

        // Check UserLdap
        static public string IsAuthenticated(string username, string password, string department)
        {
            string sRetval = "";
            Char charRange = '@';
            string userLdap = "";
            string _dc = "";

            int startIndex = username.IndexOf(charRange);
            int endIndex = username.Length;
            int length = endIndex - startIndex;


            if (startIndex > 0)
            {
                //UserName Ldap
                userLdap = username.Substring(0, startIndex).Trim();

                //Domain dc =sungroup,dc =com,dc=vn
                string domain = username.Substring(startIndex, length).Trim();
                _dc = domain.Replace("@", "dc=").Replace(".", ",dc=");
            }
            else
            {
                //UserName not Ldap
                userLdap = username.Trim();
            }

            try
            {
                if (department == "P")
                {
                    LdapDirectoryIdentifier ldi = new LdapDirectoryIdentifier(_domainLDAP_HCM, Int32.Parse(_portLDAP_HCM));
                    // Create the new LDAP connection

                    LdapConnection ldapConnection = new LdapConnection(ldi);
                    ldapConnection.AuthType = AuthType.Basic;
                    ldapConnection.SessionOptions.ProtocolVersion = 3;
                    //NetworkCredential nc = new NetworkCredential(username, password,_domainLDAP); //correct password


                    NetworkCredential nc = new NetworkCredential(userLdap, password);


                    ldapConnection.Bind(nc);
                    Console.WriteLine("LdapConnection authentication success");
                    ldapConnection.Dispose();
                }

                if (department == "PE")
                {
                    LdapDirectoryIdentifier ldi = new LdapDirectoryIdentifier(_domainLDAP_HCM, Int32.Parse(_portLDAP_HCM));
                    // Create the new LDAP connection

                    LdapConnection ldapConnection = new LdapConnection(ldi);
                    ldapConnection.AuthType = AuthType.Basic;
                    ldapConnection.SessionOptions.ProtocolVersion = 3;
                    //NetworkCredential nc = new NetworkCredential(username, password,_domainLDAP); //correct password


                    NetworkCredential nc = new NetworkCredential(userLdap, password);


                    ldapConnection.Bind(nc);
                    Console.WriteLine("LdapConnection authentication success");
                    ldapConnection.Dispose();
                }
                if (department == "PA")
                {
                    LdapDirectoryIdentifier ldi = new LdapDirectoryIdentifier(_domainLDAP_HN, Int32.Parse(_portLDAP_HN));
                    // Create the new LDAP connection

                    LdapConnection ldapConnection = new LdapConnection(ldi);
                    ldapConnection.AuthType = AuthType.Basic;
                    ldapConnection.SessionOptions.ProtocolVersion = 3;
                    //NetworkCredential nc = new NetworkCredential(username, password,_domainLDAP); //correct password


                    NetworkCredential nc = new NetworkCredential(userLdap, password);


                    ldapConnection.Bind(nc);
                    Console.WriteLine("LdapConnection authentication success");
                    ldapConnection.Dispose();
                }
                if (department == "PB")
                {
                    LdapDirectoryIdentifier ldi = new LdapDirectoryIdentifier(_domainLDAP_CPC, Int32.Parse(_portLDAP_CPC));
                    // Create the new LDAP connection

                    LdapConnection ldapConnection = new LdapConnection(ldi);
                    ldapConnection.AuthType = AuthType.Basic;
                    ldapConnection.SessionOptions.ProtocolVersion = 3;
                    //NetworkCredential nc = new NetworkCredential(username, password,_domainLDAP); //correct password


                    NetworkCredential nc = new NetworkCredential(userLdap, password);


                    ldapConnection.Bind(nc);
                    Console.WriteLine("LdapConnection authentication success");
                    ldapConnection.Dispose();
                }
                if (department == "PC")
                {
                    LdapDirectoryIdentifier ldi = new LdapDirectoryIdentifier(_domainLDAP_NPC, Int32.Parse(_portLDAP_NPC));
                    // Create the new LDAP connection

                    LdapConnection ldapConnection = new LdapConnection(ldi);
                    ldapConnection.AuthType = AuthType.Basic;
                    ldapConnection.SessionOptions.ProtocolVersion = 3;
                    //NetworkCredential nc = new NetworkCredential(username, password,_domainLDAP); //correct password


                    NetworkCredential nc = new NetworkCredential(userLdap, password);


                    ldapConnection.Bind(nc);
                    Console.WriteLine("LdapConnection authentication success");
                    ldapConnection.Dispose();
                }
                if (department == "PD")
                {
                    LdapDirectoryIdentifier ldi = new LdapDirectoryIdentifier(_domainLDAP_SPC, Int32.Parse(_portLDAP_SPC));
                    // Create the new LDAP connection

                    LdapConnection ldapConnection = new LdapConnection(ldi);
                    ldapConnection.AuthType = AuthType.Basic;
                    ldapConnection.SessionOptions.ProtocolVersion = 3;
                    //NetworkCredential nc = new NetworkCredential(username, password,_domainLDAP); //correct password


                    NetworkCredential nc = new NetworkCredential(userLdap, password);


                    ldapConnection.Bind(nc);
                    Console.WriteLine("LdapConnection authentication success");
                    ldapConnection.Dispose();
                }

            }
            catch (LdapException e)
            {
                Console.WriteLine("\r\nUnable to login:\r\n\t" + e.Message);
                sRetval = e.Message;

            }
            catch (Exception e)
            {
                Console.WriteLine("\r\nUnexpected exception occured:\r\n\t" + e.GetType() + ":" + e.Message);
                sRetval = e.Message;

            }

            return sRetval;

        }
    }
}