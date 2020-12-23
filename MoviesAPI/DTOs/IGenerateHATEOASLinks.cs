using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.DTOs
{
    public interface IGenerateHATEOASLinks
    {
        void GenerateLinks(IUrlHelper urlHelper);
        ResourceCollection<T> GenerateLinksCollection<T>(List<T> dtos, IUrlHelper urlhelper);
    }
}
