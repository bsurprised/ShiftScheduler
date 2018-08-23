using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Newtonsoft.Json;
using Onyx.ShiftScheduler.Core.Common;
using Onyx.ShiftScheduler.Core.Interfaces;

namespace Onyx.ShiftScheduler.Core.App
{
    public class TransitionSet : Entity, IPassivable
    {
        public TransitionSet()
        {
            IsActive = true;
        }

        [Required]
        [StringLength(ObjectAttributeConsts.NameLength100, MinimumLength = ObjectAttributeConsts.StringMinLength3)]
        public string Name { get; set; }

        [Required]
        public string RuleSetString { get; set; }

        [NotMapped]
        public RuleSet RuleSet { get; set; }

        public bool IsActive { get; set; }

        public static TransitionSet FromRuleSet(string name, RuleSet ruleSet)
        {
            return new TransitionSet()
            {
                Name = name,
                RuleSet = ruleSet,
                RuleSetString = JsonConvert.SerializeObject(ruleSet)
            };
        }

        public static TransitionSet FromRuleSetString(string name, string ruleSetString)
        {
            return new TransitionSet()
            {
                Name = name,
                RuleSet = JsonConvert.DeserializeObject<RuleSet>(ruleSetString),
                RuleSetString = ruleSetString
            };
        }
    }
}
