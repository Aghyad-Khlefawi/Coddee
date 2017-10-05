// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;

namespace Coddee.WPF
{
    public class Resolution
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public string Title { get; set; }

        public static List<Resolution> CommonResolutions = new List<Resolution>
        {
           new Resolution { Height = double.NaN, Width = double.NaN, Title = "Full screen" },
           new Resolution { Height = 1080, Width = 1920, Title = "1080x1920"},
           new Resolution { Height = 992, Width = 1768, Title = "992x1768"},
           new Resolution { Height = 1050, Width = 1680, Title = "1050x1680"},
           new Resolution { Height = 1024, Width = 1600, Title = "1024x1600"},
           new Resolution { Height = 900, Width = 1600, Title = "900x1600"},
           new Resolution { Height = 900, Width = 1440, Title = "900x1440"},
           new Resolution { Height = 768, Width = 1366, Title = "768x1366"},
           new Resolution { Height = 768, Width = 1360, Title = "768x1360"},
           new Resolution { Height = 1024, Width = 1280, Title = "1024x1280"},
        };
    }
}
