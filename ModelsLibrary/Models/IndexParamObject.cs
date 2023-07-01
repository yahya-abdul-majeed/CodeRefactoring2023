using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.Models
{
    public class IndexParamObject
    {
        public int? roomNo { get; set; } = 0;
        public int? buildingNo { get; set; } = 0;   
        public string? priority { get; set; } = null;   
        public string? state { get; set; } = null;

    }
}
