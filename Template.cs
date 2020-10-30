using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Templating;

namespace DotNet.Cli.Flubu
{
    public class Template : IFlubuTemplate
    {
        public void ConfigureTemplate(IFlubuTemplateBuilder templateBuilder)
        {
            templateBuilder.AddReplacementToken(new TemplateReplacmentToken()
            {
                Token = "{{SolutionFileName}}",
                Description = "Enter relative path to solution filename:",
            });
        }
    }
}
