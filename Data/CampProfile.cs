using AutoMapper;
using CoreCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Data
{
    public class CampProfile:Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>().
                ForMember(c=>c.Venue,o=>o.MapFrom(b=>b.Location.VenueName)).ForMember(o=>o.Address1,
                c=>c.MapFrom(b=>b.Location.Address1)
               ).ReverseMap() ;
            CreateMap<Talk, TalkModel>().ReverseMap().ForMember(t=>t.Camp,cnfg=>cnfg.Ignore())
                .ForMember(t=>t.Speaker,cnfg=>cnfg.Ignore());
            CreateMap<Speaker, SpeakerModel>().ReverseMap();
        }
    }
}
