﻿using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace SMSService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new SMS() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
