// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Validation
{
    /// <summary>
    /// A validation delegate
    /// </summary>
    /// <param name="obj">The object that needs validation.</param>
    /// <returns>The validation result.</returns>
    public delegate bool Validator(object obj);


    /// <summary>
    /// A validation delegate
    /// </summary>
    /// <param name="obj">The object that needs validation.</param>
    /// <returns>The validation result.</returns>
    public delegate bool Validator<TTarget>(TTarget obj);
}