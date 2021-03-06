﻿using System.IO;
using System.Net;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotInitializr.Website.Server;
using DotInitializr.Website.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace DotInitializr.UnitTests
{
   internal class GeneratorControllerTest
   {
      [Test]
      [TestCase("https://github.com/dsuryd/DotInitializr", "DotInitializr.UnitTests\\TestTemplate")]
      public async Task GeneratorController_Get_ReturnsZippedProject(string sourceUrl, string directory, string branch = null)
      {
         var client = new WebApplicationFactory<Startup>().CreateClient();

         var metadata = new ProjectMetadata
         {
            ProjectName = "MyStarter",
            TemplateSourceType = "git",
            TemplateSourceUrl = sourceUrl,
            TemplateSourceDirectory = directory,
            TemplateSourceBranch = branch,
            Tags = new Dictionary<string, object>
            {
               { "projectName", "StarterApp" },
               { "namespace", "My.StarterApp" },
               { "ng", false },
               { "react", true }
            },
            FilesToExclude = "ClientApp{{ng}}/**"
         };
         var response = await client.PostAsJsonAsync("/api/generator", metadata);

         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

         var fileName = response.Content.Headers.ContentDisposition.FileName;
         var tempPath = Path.Combine(Path.GetTempPath(), nameof(DotInitializr), fileName);
         using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
         {
            await response.Content.CopyToAsync(fileStream);
         }

         File.Delete(tempPath);
      }
   }
}