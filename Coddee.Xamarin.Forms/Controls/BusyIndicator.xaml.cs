﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Coddee.Xamarin.Forms
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BusyIndicator : ContentView
	{
		public BusyIndicator ()
		{
			InitializeComponent ();
		}
	}
}