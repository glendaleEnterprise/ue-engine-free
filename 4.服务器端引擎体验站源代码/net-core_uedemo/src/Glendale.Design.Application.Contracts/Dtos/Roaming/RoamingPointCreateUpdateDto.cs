using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Roaming
{
    /// <summary>
    /// 第一/第三人称漫游记录
    /// </summary>
    public class RoamingPointCreateUpdateDto
    {
        //   {
        //    "Time":3,
        //    "Location":[
        //        593.2596435546875,
        //        -342.7739562988281,
        //        1111.8209228515625
        //    ],
        //    "Rotate":[
        //        0,
        //        328.0348205566406,
        //        271.278564453125
        //    ]
        //}
        public virtual int Time { get; set; }
        public virtual double[] Location { get; set; }
        public virtual double[] Rotate { get; set; }
    }
    
}