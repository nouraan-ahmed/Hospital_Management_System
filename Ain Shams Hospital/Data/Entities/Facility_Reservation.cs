﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ain_Shams_Hospital.Data.Entities
{
    [Keyless]
    public class Facility_Reservation
    {
        public Hospital_Facility Facility_Id { get; set; }
        public string Start_Hour { get; set; }
        public string End_Hour { get; set; }
        public Staff Staff_Id { get; set; }
        public Patient Patient_Id { get; set; }
    }
}
