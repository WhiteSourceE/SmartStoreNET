using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartStore.Web.Models.ReflectedXSS
{
    public class DummyMessage
    {
        [Display(Name = "Name")]
        public string Name { get; set; }
        [AllowHtml]
        public string Description { get; set; }

    }
}