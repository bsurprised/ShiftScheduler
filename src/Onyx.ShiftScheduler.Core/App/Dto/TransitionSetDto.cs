using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Newtonsoft.Json;
using Onyx.ShiftScheduler.Core.Common;

namespace Onyx.ShiftScheduler.Core.App.Dto
{
    public class TransitionSetDto : EntityDto
    {

        public string Name { get; set; }

        public static TransitionSetDto FromEntity(TransitionSet input)
        {
            if (input == null)
                return null;

            return new TransitionSetDto
            {
                Id = input.Id,
                Name = input.Name
            };
        }
    }
}
