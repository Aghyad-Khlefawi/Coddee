// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;

namespace Coddee.Validation
{
    public class ValidationResult
    {
        public ValidationResult()
        {
            Errors = new List<string>();
            Warnings = new List<string>();
        }

        public bool IsValid => !HasErrors;
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }

        public bool IsValidWithoutWarrnings => !HasErrors && !HasWarnings;
        public bool HasErrors => Errors == null || Errors.Count > 0;
        public bool HasWarnings => Warnings == null || Warnings.Count > 0;

        public void Append(ValidationResult result)
        {
            Errors.AddRange(result.Errors);
            Warnings.AddRange(result.Warnings);
        }
    }
}
