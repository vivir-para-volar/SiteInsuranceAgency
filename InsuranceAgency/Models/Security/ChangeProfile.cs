﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InsuranceAgency.Models.Security
{
    public class ChangeProfile
    {
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Bio { get; set; }
    }
}