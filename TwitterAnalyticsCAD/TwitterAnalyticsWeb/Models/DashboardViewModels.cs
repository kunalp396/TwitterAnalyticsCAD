using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TwitterAnalyticsWeb.Models
{
    public class FilterCriteria
    {
        [Required]
        [Display(Name = "Topics")]
        [DataType(DataType.Text)]
        public string Topics { get; set; }

        
        [Display(Name = "Duration")]       
        public List<SelectListItem> Duration { get; set; }

        [Display(Name = "Time Zone")]
        public List<SelectListItem> TimeZone { get; set; }
    }
}