﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work_Track_Server_V2
{
    public class Worked_time : ICLON
    {
        public string PC_ID;
        public string Worked_Time;
        public string Worked_Date;
        public int rebootcount;
        public Worked_time()
        {
            rebootcount = 0;
        }
        public object Clone()
        {
            return new Worked_time { PC_ID = this.PC_ID, Worked_Time = this.Worked_Time, Worked_Date = this.Worked_Date, rebootcount = this.rebootcount };
        }

    }
}
