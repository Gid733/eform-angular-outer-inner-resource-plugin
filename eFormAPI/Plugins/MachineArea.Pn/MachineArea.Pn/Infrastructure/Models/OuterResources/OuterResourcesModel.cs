﻿using System.Collections.Generic;

namespace MachineArea.Pn.Infrastructure.Models.OuterResources
{
    public class OuterResourcesModel
    {
        public int Total { get; set; }
        public List<OuterResourceModel> OuterResourceList { get; set; }
        public string Name { get; set; }
    }
}