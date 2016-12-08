﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bigdata.filereader.Model
{
   public interface INeissReportRepository
    {
        NeissReport Get(int CpscCaseNumber);
        void Add(NeissReport report);
        void Remove(NeissReport report);
    }
}