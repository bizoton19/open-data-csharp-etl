﻿using OpenData.Shaper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenData.Shaper.Contracts
{
   public interface IIncidentRepository
    {
        IncidentReport Get(int IncidentReporId);
        void Add(IncidentReport incident);
        void Remove(IncidentReport  incident);
    }
}
