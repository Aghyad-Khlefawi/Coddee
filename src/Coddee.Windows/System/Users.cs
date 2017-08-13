// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Coddee.Windows
{
    public class Users
    {
        /// <summary>
        /// Returns the local user accounts on this machines
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetLocalUsers()
        {
            List<string> users = new List<string>();
            var path = $"WinNT://{Environment.MachineName},computer";
            using (var computerEntry = new DirectoryEntry(path))
                foreach (DirectoryEntry childEntry in computerEntry.Children)
                    if (childEntry.SchemaClassName == "User")
                    {
                        users.Add(childEntry.Name);
                    }
            return users;
        }

        /// <summary>
        /// Change a local user password
        /// </summary>
        /// <param name="username">The taget user</param>
        /// <param name="newPassword">The new pasword</param>
        public static void ChangeUserPassword(string username, string newPassword)
        {
            var path = $"WinNT://{Environment.MachineName},computer";
            DirectoryEntry localDirectory = new DirectoryEntry(path);
            DirectoryEntries users = localDirectory.Children;
            DirectoryEntry user = users.Find(username);
            user.Invoke("SetPassword", newPassword);
        }

        /// <summary>
        /// Creates a new local user on the machine
        /// Requires a reference to System.DirectoryServices.AccountManagement.dll
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="group">A gounp name to assigne the user to</param>
        /// <param name="displayName">The display name</param>
        /// <param name="description">Description</param>
        /// <param name="passwordExpires">Password expires</param>
        public static void CreateLocalWindowsAccount(string username, string password, string displayName, string description, bool passwordExpires, string group="Users")
        {
            PrincipalContext context = new PrincipalContext(ContextType.Machine);
            UserPrincipal user = new UserPrincipal(context);
            user.SetPassword(password);
            user.DisplayName = displayName;
            user.Name = username;
            user.Description = description;
            user.PasswordNeverExpires = passwordExpires;
            user.Save();
            GroupPrincipal groupPrincipal = GroupPrincipal.FindByIdentity(context, group);
            groupPrincipal.Members.Add(user);
            groupPrincipal.Save();
        }
    }

}
