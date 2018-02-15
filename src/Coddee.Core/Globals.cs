// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee
{
    /// <summary>
    /// A global variable that contains the current logged in user username
    /// </summary>
    public class UsernameGlobalVariable : GlobalVarialbe<string> { }

    /// <summary>
    /// A global variable that contains the current application name.
    /// </summary>
    public class ApplicationNameGlobalVariable : GlobalVarialbe<string> { }


    /// <summary>
    /// A global variable that container a value that indicates whether the application is using a default
    /// of a custom shell
    /// </summary>
    public class UsingDefaultShellGlobalVariable : GlobalVarialbe<bool> { }

   
}