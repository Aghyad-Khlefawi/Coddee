// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Services
{
    public interface IGlobalEventsService
    {
        TResult GetEvent<TResult>() where TResult : class, IGlobalEvent, new();
    }
}