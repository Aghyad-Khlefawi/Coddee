// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Validation
{
    public delegate bool Validator(object obj);
    public delegate bool Validator<TTarget>(TTarget obj);
}