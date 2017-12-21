// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Validation
{
    public enum ValidationType
    {
        Error,
        Warning
    }

    public interface IValidationRule
    {
        ValidationType ValidationType { get; }
        Validator Validator { get; }

        string FieldName { get; set; }
        bool Validate();
        string GetMessage();
    }
}
