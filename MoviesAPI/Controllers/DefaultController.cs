﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DefaultController : ControllerBase
    {
        [Route("/")]
        [Route("/swagger")]
        public RedirectResult Index()
        {
            return new RedirectResult("~/swagger");
        }

    }
}
