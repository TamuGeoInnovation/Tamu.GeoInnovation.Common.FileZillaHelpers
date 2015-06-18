using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using USC.GISResearchLab.Common.Utils.XML;
using System.IO;
using System.Security.Cryptography;
using USC.GISResearchLab.Common.Security.Cryptography;
using USC.GISResearchLab.Common.Core.ShellCommands;

namespace USC.GISResearchLab.Common.Core.FTP
{
    public class FileZillaServerUtils
    {

        public static bool AddUser(string userName, string homeDirectory, string password)
        {
            bool ret = false;
            try
            {
                string configurationFile = @"C:\Program Files (x86)\Filezilla Server\FileZilla Server.xml";

                string contents = File.ReadAllText(configurationFile);

                XmlDocument xmlDocument = XMLUtils.CreateXmlDocumentFromXmlString(contents);
                //xmlDocument.LoadXml(configurationFile);

                XmlNode usersNode = xmlDocument.SelectSingleNode("//Users");
                XmlNode userNode = usersNode.SelectSingleNode("//User[@Name='" + userName + "']");

                if (userNode != null)
                {
                    XmlNode optionPassNode = usersNode.SelectSingleNode("//Option[@Name='Pass']");
                    if (optionPassNode != null)
                    {
                        optionPassNode.InnerText = CryptographyUtils.CalculateMD5Hash(password);
                    }
                }
                else
                {
                    userNode = xmlDocument.CreateElement("User");

                    XmlAttribute userNameAttr = xmlDocument.CreateAttribute("Name");
                    userNameAttr.Value = userName;
                    userNode.Attributes.Append(userNameAttr);


                    XmlNode optionPassNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionPassAttr = xmlDocument.CreateAttribute("Name");
                    optionPassAttr.Value = "Pass";
                    optionPassNode.Attributes.Append(optionPassAttr);
                    optionPassNode.InnerText = CryptographyUtils.CalculateMD5Hash(password);
                    userNode.AppendChild(optionPassNode);

                    XmlNode optionGroupNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionGroupAttr = xmlDocument.CreateAttribute("Name");
                    optionGroupAttr.Value = "Group";
                    optionGroupNode.Attributes.Append(optionGroupAttr);
                    optionGroupNode.InnerText = "";
                    userNode.AppendChild(optionGroupNode);

                    XmlNode optionByPassNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionByPassAttr = xmlDocument.CreateAttribute("Name");
                    optionByPassAttr.Value = "Bypass server userlimit";
                    optionByPassNode.Attributes.Append(optionByPassAttr);
                    optionByPassNode.InnerText = "0";
                    userNode.AppendChild(optionByPassNode);

                    XmlNode optionUserLimitNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionUserLimitAttr = xmlDocument.CreateAttribute("Name");
                    optionUserLimitAttr.Value = "User Limit";
                    optionUserLimitNode.Attributes.Append(optionUserLimitAttr);
                    optionUserLimitNode.InnerText = "0";
                    userNode.AppendChild(optionUserLimitNode);

                    XmlNode optionIPLimitNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionIPLimitAttr = xmlDocument.CreateAttribute("Name");
                    optionIPLimitAttr.Value = "IP Limit";
                    optionIPLimitNode.Attributes.Append(optionIPLimitAttr);
                    optionIPLimitNode.InnerText = "0";
                    userNode.AppendChild(optionIPLimitNode);

                    XmlNode optionEnabledNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionEnabledAttr = xmlDocument.CreateAttribute("Name");
                    optionEnabledAttr.Value = "Enabled";
                    optionEnabledNode.Attributes.Append(optionEnabledAttr);
                    optionEnabledNode.InnerText = "1";
                    userNode.AppendChild(optionEnabledNode);

                    XmlNode optionCommentsNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionCommentsAttr = xmlDocument.CreateAttribute("Name");
                    optionCommentsAttr.Value = "Comments";
                    optionCommentsNode.Attributes.Append(optionCommentsAttr);
                    optionCommentsNode.InnerText = "";
                    userNode.AppendChild(optionCommentsNode);

                    XmlNode optionForceSSLNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionForceSSLAttr = xmlDocument.CreateAttribute("Name");
                    optionForceSSLAttr.Value = "ForceSsl";
                    optionForceSSLNode.Attributes.Append(optionForceSSLAttr);
                    optionForceSSLNode.InnerText = "0";
                    userNode.AppendChild(optionForceSSLNode);

                    XmlNode ipFilterNode = xmlDocument.CreateElement("IPFilter");
                    XmlNode disallowedNode = xmlDocument.CreateElement("Disallowed");
                    XmlNode allowedNode = xmlDocument.CreateElement("Allowed");
                    ipFilterNode.AppendChild(disallowedNode);
                    ipFilterNode.AppendChild(allowedNode);
                    userNode.AppendChild(ipFilterNode);


                    XmlNode permissionsNode = xmlDocument.CreateElement("Permissions");

                    XmlNode permissionNode = xmlDocument.CreateElement("Permission");
                    XmlAttribute permissionDirAttr = xmlDocument.CreateAttribute("Dir");
                    permissionDirAttr.Value = homeDirectory;
                    permissionNode.Attributes.Append(permissionDirAttr);


                    XmlNode optionFileReadNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionFileReadAttr = xmlDocument.CreateAttribute("Name");
                    optionFileReadAttr.Value = "FileRead";
                    optionFileReadNode.Attributes.Append(optionFileReadAttr);
                    optionFileReadNode.InnerText = "1";
                    permissionNode.AppendChild(optionFileReadNode);

                    XmlNode optionFileWriteNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionFileWriteAttr = xmlDocument.CreateAttribute("Name");
                    optionFileWriteAttr.Value = "FileWrite";
                    optionFileWriteNode.Attributes.Append(optionFileWriteAttr);
                    optionFileWriteNode.InnerText = "1";
                    permissionNode.AppendChild(optionFileWriteNode);

                    XmlNode optionFileDeleteNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionFileDeleteAttr = xmlDocument.CreateAttribute("Name");
                    optionFileDeleteAttr.Value = "FileDelete";
                    optionFileDeleteNode.Attributes.Append(optionFileDeleteAttr);
                    optionFileDeleteNode.InnerText = "1";
                    permissionNode.AppendChild(optionFileDeleteNode);

                    XmlNode optionFileAppendNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionFileAppendAttr = xmlDocument.CreateAttribute("Name");
                    optionFileAppendAttr.Value = "FileAppend";
                    optionFileAppendNode.Attributes.Append(optionFileAppendAttr);
                    optionFileAppendNode.InnerText = "1";
                    permissionNode.AppendChild(optionFileAppendNode);

                    XmlNode optionDirCreateNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionDirCreateAttr = xmlDocument.CreateAttribute("Name");
                    optionDirCreateAttr.Value = "DirCreate";
                    optionDirCreateNode.Attributes.Append(optionDirCreateAttr);
                    optionDirCreateNode.InnerText = "0";
                    permissionNode.AppendChild(optionDirCreateNode);

                    XmlNode optionDirDeleteNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionDirDeleteAttr = xmlDocument.CreateAttribute("Name");
                    optionDirDeleteAttr.Value = "DirDelete";
                    optionDirDeleteNode.Attributes.Append(optionDirDeleteAttr);
                    optionDirDeleteNode.InnerText = "0";
                    permissionNode.AppendChild(optionDirDeleteNode);

                    XmlNode optionDirListNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionDirListAttr = xmlDocument.CreateAttribute("Name");
                    optionDirListAttr.Value = "DirList";
                    optionDirListNode.Attributes.Append(optionDirListAttr);
                    optionDirListNode.InnerText = "1";
                    permissionNode.AppendChild(optionDirListNode);

                    XmlNode optionDirSubdirsNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionDirSubdirsAttr = xmlDocument.CreateAttribute("Name");
                    optionDirSubdirsAttr.Value = "DirSubdirs";
                    optionDirSubdirsNode.Attributes.Append(optionDirSubdirsAttr);
                    optionDirSubdirsNode.InnerText = "1";
                    permissionNode.AppendChild(optionDirSubdirsNode);

                    XmlNode optionIsHomeNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionIsHomeAttr = xmlDocument.CreateAttribute("Name");
                    optionIsHomeAttr.Value = "IsHome";
                    optionIsHomeNode.Attributes.Append(optionIsHomeAttr);
                    optionIsHomeNode.InnerText = "1";
                    permissionNode.AppendChild(optionIsHomeNode);

                    XmlNode optionAutoCreateNode = xmlDocument.CreateElement("Option");
                    XmlAttribute optionAutoCreateAttr = xmlDocument.CreateAttribute("Name");
                    optionAutoCreateAttr.Value = "AutoCreate";
                    optionAutoCreateNode.Attributes.Append(optionAutoCreateAttr);
                    optionAutoCreateNode.InnerText = "0";
                    permissionNode.AppendChild(optionAutoCreateNode);

                    permissionsNode.AppendChild(permissionNode);
                    userNode.AppendChild(permissionsNode);


                    XmlNode speedLimitsNode = xmlDocument.CreateElement("SpeedLimits");
                    XmlAttribute speedLimitsDLTypeAttr = xmlDocument.CreateAttribute("DlType");
                    speedLimitsDLTypeAttr.Value = "0";
                    speedLimitsNode.Attributes.Append(speedLimitsDLTypeAttr);
                    XmlAttribute speedLimitsDlLimitAttr = xmlDocument.CreateAttribute("DlLimit");
                    speedLimitsDlLimitAttr.Value = "10";
                    speedLimitsNode.Attributes.Append(speedLimitsDlLimitAttr);
                    XmlAttribute speedLimitsServerDlLimitBypassAttr = xmlDocument.CreateAttribute("ServerDlLimitBypass");
                    speedLimitsServerDlLimitBypassAttr.Value = "0";
                    speedLimitsNode.Attributes.Append(speedLimitsServerDlLimitBypassAttr);
                    XmlAttribute speedLimitsUlTypeAttr = xmlDocument.CreateAttribute("UlType");
                    speedLimitsUlTypeAttr.Value = "0";
                    speedLimitsNode.Attributes.Append(speedLimitsUlTypeAttr);
                    XmlAttribute speedLimitsUlLimitAttr = xmlDocument.CreateAttribute("UlLimit");
                    speedLimitsUlLimitAttr.Value = "10";
                    speedLimitsNode.Attributes.Append(speedLimitsUlLimitAttr);
                    XmlAttribute speedLimitsServerUlLimitBypassAttr = xmlDocument.CreateAttribute("ServerUlLimitBypass");
                    speedLimitsServerUlLimitBypassAttr.Value = "0";
                    speedLimitsNode.Attributes.Append(speedLimitsServerUlLimitBypassAttr);

                    XmlNode speedLimitsDownloadNode = xmlDocument.CreateElement("Download");
                    XmlNode speedLimitsUploadNode = xmlDocument.CreateElement("Upload");

                    speedLimitsNode.AppendChild(speedLimitsDownloadNode);
                    speedLimitsNode.AppendChild(speedLimitsUploadNode);
                    userNode.AppendChild(speedLimitsNode);


                    usersNode.AppendChild(userNode);


                }

                xmlDocument.Save(configurationFile);

                ShellCommandUtils.ExecuteCommandAsync("\"C:\\Program Files (x86)\\FileZilla Server\\FileZilla server.exe\" /reload-config");
            }
            catch (Exception e)
            {
                throw new Exception("Error in AddUser: " + e.Message, e);
            }
            return ret;
        }

    }
}
